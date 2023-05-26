using System.Net;
using System.Net.Sockets;
using System.Text;
    
namespace P2PChat;

public class Client
{
    private string Ip { get; }

    private int Port { get; }

    private readonly HashSet<string> _usersInChat;
    
    private Socket _socket;
    
    private readonly IPEndPoint _serverEndPoint; 

    private bool _isListening;

    private readonly Action<string> _wpfPrint;
    
    private Thread _listener;


    public Client(string ip, int port, Action<string> wpfPrint)
    {
        Ip = ip;
        Port = port;
        _isListening = false;
        _wpfPrint = wpfPrint;

        _usersInChat = new HashSet<string>();
            
        _serverEndPoint = new IPEndPoint(IPAddress.Parse(ServerInfo.Ip), ServerInfo.Port);
        
        var endPoint = new IPEndPoint(IPAddress.Parse(Ip), Port);
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _socket.Bind(endPoint);

        _listener = null!;
    }

    private void SendMessage(string message, EndPoint endPoint)
    {
        try
        {
            _socket.SendTo(Encoding.UTF8.GetBytes(message), endPoint);
        }
        catch (Exception _)
        {
            // ignored
        }
    }
    
    public bool CreateChat()
    {
        SendMessage($"create${Ip}:{Port}", _serverEndPoint);

        var bytes = new byte[1024];
        var bytesRec = _socket.Receive(bytes);
        var data = Encoding.UTF8.GetString(bytes, 0, bytesRec);

        return data switch
        {
            "success" => true,
            _ => false
        };
    }

    public bool JoinChat(string ip, int port)
    {
        SendMessage($"join${Ip}:{Port}${ip}:{port}", _serverEndPoint);
        
        var bytes = new byte[1024];
        var bytesRec = _socket.Receive(bytes);
        var data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            
        if (data == "error")
        {
            return false;
        }

        var usersInChatFromServer = data.Split('$');
        foreach (var user in usersInChatFromServer)
        {
            _usersInChat.Add(user);
            
            var ipPort = user.Split(':');
            var userIp = ipPort[0];
            var userPort = int.Parse(ipPort[1]);
            var endPoint = new IPEndPoint(IPAddress.Parse(userIp), userPort);
            
            SendMessage($"add${Ip}:{Port}", endPoint);
        }
        
        return true;
    }

    public void ListenMessages()
    {
        _listener = new Thread(Listen);
        _listener.Start();
    }
    
    private void Listen()
    {
        _isListening = true;
        // Listening...
        while (_isListening)
        {
            var data = "";
            var bytes = new byte[1024];
            int bytesRec;
            // New response from some client!
            try
            {
                bytesRec = _socket.Receive(bytes);
            }
            catch (Exception _)
            {
                continue;
            }

            data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

            var dataArray = data.Split("$");
            
            switch (dataArray[0])
            {
                // New user in chat where this user presents.
                case "add":
                    _usersInChat.Add(dataArray[1]);
                    _wpfPrint($"Welcome, [{dataArray[1]}]!"); 
                    break;
                // Other user in chat (where this user presents) leaved.
                case "remove":
                    _usersInChat.Remove(dataArray[1]);
                    _wpfPrint ($"Bye, [{dataArray[1]}]...");
                    break;
                // Message from other user.
                case "message":
                    _wpfPrint ($"[{dataArray[1]}]: " +dataArray[2]);
                    break;
                // It is time to stop working.
                case "_":
                    return;
            }
        }
    }

    public void DisconnectFromChat()
    {
        SendMessage($"disconnect${Ip}:{Port}", _serverEndPoint);
        
        // var bytes = new byte[1024];
        // var bytesRec = _socket.Receive(bytes);
        
        foreach (var endPoint in from user in _usersInChat select user.Split(':') 
                 into ipPort let userIp = ipPort[0] let userPort = int.Parse(ipPort[1]) 
                 select new IPEndPoint(IPAddress.Parse(userIp), userPort))
        {
            SendMessage($"remove${Ip}:{Port}", endPoint);
        }
        _usersInChat.Clear();
        Dispose();
    }

    public void SendMessage(string message)
    {
        _wpfPrint(message);

        foreach (var endPoint in from user in _usersInChat select user.Split(':') 
                 into ipPort let userIp = ipPort[0] let userPort = int.Parse(ipPort[1]) 
                 select new IPEndPoint(IPAddress.Parse(userIp), userPort))
        {
            SendMessage($"message${Ip}:{Port}${message}", endPoint);
        }
    }
    private void Dispose()
    {
        _isListening = false;
        
        // We need to tell thread that there is time to stop working (posible internal eternal waiting response without it [line 118]).
        var endPoint = new IPEndPoint(IPAddress.Parse(Ip), Port);
        var endListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        endListenSocket.Connect(endPoint);
        endListenSocket.Send("_"u8.ToArray());

        _listener.Join();
        
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
        _socket = null!;
        
        _usersInChat.Clear();
    }
}

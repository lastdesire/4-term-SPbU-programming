using System.Net;
using System.Net.Sockets;
using System.Text;

namespace P2PChat;

public class Server : IDisposable
{
    private Socket _socket;

    private bool _isListening;

    // One set represents one chat where the strings have the form "UserIP:UserPort".
    private List<HashSet<string>> _usersInChat;

    private Thread _listener;

    public Server()
    {
        var endPoint = new IPEndPoint(IPAddress.Parse(ServerInfo.Ip), ServerInfo.Port);
        
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        
        // Attached the socket to the IP address and port.
        _socket.Bind(endPoint);

        _isListening = false;
        _usersInChat = new List<HashSet<string>>();

        _listener = new Thread(Listen);
        _listener.Start();
    }

    private void Listen()
    {
        // Listening...
        _isListening = true;
        while (_isListening)
        {
            // Waiting... 
            Console.WriteLine("Waiting...");
            var data = "";
            
            var bytes = new byte[1024];
            EndPoint clientEndPoint = new IPEndPoint(0,0);
            
            // New client!
            var bytesRec = _socket.ReceiveFrom(bytes, ref clientEndPoint);

            data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
            
            Console.WriteLine("Data from new client:" + data);

            // Data looks like "UserIP:UserPort$Method" where Method is possible to be "join", "create", "disconnect".
            var arrayData = data.Split('$');
            var userData = arrayData[1];
            var userMethod = arrayData[0].ToLower();

            var reply = "error";
            var flagContains = false;

            switch (userMethod)
            {
                case "create":
                    Console.WriteLine("Trying to create the chat...");
                    if (_usersInChat.Any(hashset => hashset.Contains(userData)))
                    {
                        flagContains = true;
                    }
                    if (!flagContains)
                    {
                        Console.WriteLine("Success!");
                        
                        reply = "success";
                        var hashset = new HashSet<string> { userData };
                        _usersInChat.Add(hashset);
                    }
                    else
                    {
                        Console.WriteLine("Nope!");
                    }
                    break;
                
                case "join":
                    Console.WriteLine("Trying to join the chat...");
                    var joinData = arrayData[2];
                    HashSet<string> savedHashset = null!;
                    foreach (var hashset in _usersInChat.Where(hashset => hashset.Contains(joinData)))
                    {
                        // A user with such an ID and port is already a server for any chat.
                        flagContains = true;
                        savedHashset = hashset;
                        break;
                    }

                    if (flagContains)
                    {
                        Console.WriteLine("Success!");
                        
                        reply = savedHashset.Aggregate("", (current, item) => current + item + "$");
                        savedHashset.Add(userData);
                        reply = reply.Length == 0 ? reply : reply.Remove(reply.Length - 1, 1); // Remove last "$"
                    }
                    else
                    {
                        Console.WriteLine("Nope!");
                    }
                    break;
                
                case "disconnect":
                    Console.WriteLine("Trying to disconnect user from the chat...");
                    foreach (var hashset in _usersInChat.Where(hashset => hashset.Contains(userData)))
                    {
                        // A user with such an ID and port is already a server for any chat.
                        hashset.Remove(userData);
                        break;
                    }
                    reply = "success";
                    break;
                case "_":
                    return;
            }
            Console.WriteLine("REPLY: " + reply);
            
            var msg = Encoding.UTF8.GetBytes(reply);
            _socket.SendTo(msg, clientEndPoint);
            Console.WriteLine();
        }
        
    }

    public void Dispose()
    {
        _isListening = false;

        var endPoint = new IPEndPoint(IPAddress.Parse(ServerInfo.Ip), ServerInfo.Port);
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

using System;
using System.Net;
using System.Windows;
using P2PChat;

namespace P2PChatUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private P2PChatWindow _chatWindow;
        private Client _client;
        
        public MainWindow()
        {
            _chatWindow = null!;
            InitializeComponent();
        }

        private static uint ToUint32(IPAddress ipAddress)
        {
            var bytes = ipAddress.GetAddressBytes();

            return (uint)(bytes[0] << 24) |
                   (uint)(bytes[1] << 16) |
                   (uint)(bytes[2] << 8) |
                   (uint)(bytes[3]);
        }

        private static int CompareIpAddresses(IPAddress x, IPAddress y)
        {
            var int1 = ToUint32(x);
            var int2 = ToUint32(y);
            if (int1 == int2)
                return 0;
            if (int1 > int2)
                return 1;
            return -1;
        }

        private bool CheckIpAndPort(string ip, string port)
        {
            int correctPort;
            try
            {
                IPAddress.Parse(ip);
                correctPort = int.Parse(port);
            }
            catch (Exception _)
            {
                ErrorLabel.Content = "This IP or port is bad...";
                return false;
            }

            if (CompareIpAddresses(IPAddress.Parse("127.0.0.1"), IPAddress.Parse(ip)) == 1 ||
                CompareIpAddresses(IPAddress.Parse("127.255.255.255"), IPAddress.Parse(ip)) == -1)
            {
                ErrorLabel.Content = "This IP is bad...";
                return false;
            }

            if (correctPort is >= 3000 and <= 65535) return true;
                ErrorLabel.Content = "This port is bad...";
                return false;
            }

        private void CreateButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!CheckIpAndPort(UserIp.Text, UserPort.Text))
            {
                return;
            }

            _chatWindow = new P2PChatWindow();
            try
            {
                _client = new Client(UserIp.Text, int.Parse(UserPort.Text), _chatWindow.PrintMessage);
            }
            catch (Exception _)
            {
                ErrorLabel.Content = "Can't create chat using this IP and port...";
                return;
            }

            var result = _client.CreateChat();
            if (result)
            {
                _chatWindow.Show();
                _chatWindow.Client = _client;
                _client.ListenMessages();
                Close();
            }
            else
            {
                ErrorLabel.Content = "Can't create chat using this IP and port...";
            }
        }

        private void JoinButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!CheckIpAndPort(UserIp.Text, UserPort.Text) || !CheckIpAndPort(OtherUserIp.Text, OtherUserPort.Text))
            {
                return;
            }
            _chatWindow = new P2PChatWindow();
            try
            {
                _client = new Client(UserIp.Text, int.Parse(UserPort.Text), _chatWindow.PrintMessage);
            }
            catch (Exception _)
            {
                ErrorLabel.Content = "Can't join chat using this IP and port...";
                return;
            }
            var result = _client.JoinChat(OtherUserIp.Text, int.Parse(OtherUserPort.Text));
            if (result)
            {
                _chatWindow.Show();
                _chatWindow.Client = _client;
                _client.ListenMessages();
                Close();
            }
            else
            {
                ErrorLabel.Content = "Can't join this chat using other IP and other port (maybe it does not exist?)...";
            }
        }
    }
}

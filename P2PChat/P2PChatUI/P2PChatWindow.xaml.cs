using System;
using System.ComponentModel;
using System.Windows;
using P2PChat;

namespace P2PChatUI;

public partial class P2PChatWindow
{
    internal Client Client;

    public P2PChatWindow()
    {
        InitializeComponent();
    }

    public void PrintMessage(string s)
    {
        // https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
        ChatTextBox.Dispatcher.Invoke(() =>
        {
            ChatTextBox.Text += Environment.NewLine + s;
        });
    }

    private void SendMessage(object sender, RoutedEventArgs routedEventArgs)
    {
        Client.SendMessage(ChatInputTextBox.Text);
    }

    private void Disconnect(object? sender, CancelEventArgs cancelEventArgs)
    {
        try
        {
            Client.DisconnectFromChat();
        }
        catch (Exception _)
        {
            return;
        }
    }
}

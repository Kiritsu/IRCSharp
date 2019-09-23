using System;
using System.Threading;
using IRCSharp;
using IRCSharp.EventArgs;

namespace IRCShdarp.Example
{
    public class Program
    {
        public static void Main()
        {
            var client = new IRCClient(new IRCConfiguration
            {
                Username = "kiritsu___",
                Hostname = "irc.chat.twitch.tv",
                Port = 6667,
                Password = "zzz"
            });

            client.Ready += Client_Ready;
            client.MessageReceived += Client_MessageReceived;

            client.Connect();

            Thread.Sleep(-1);
        }

        private static void Client_MessageReceived(MessageReceivedEventArgs e)
        {
            Console.WriteLine($"{e.Channel.Name} - {e.User.Username}: {e.Message}");
        }

        private static void Client_Ready(ReadyEventArgs e)
        {
            e.Client.Send("JOIN #sardoche");
        }
    }
}

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
                Hostname = "irc.onlinegamesnet.net",
                Port = 6667
            });

            client.Ready += Client_Ready;
            client.UnhandledDataReceived += ClientOnUnhandledDataReceived;

            client.Connect();

            Thread.Sleep(-1);
        }

        private static void ClientOnUnhandledDataReceived(string obj)
        {
            Console.WriteLine($"[UNHANDLED]: {obj}");
        }
        private static void Client_Ready(ReadyEventArgs e)
        {
            e.Client.Send("JOIN #jsp");
        }
    }
}

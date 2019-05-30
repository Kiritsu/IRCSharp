using System.Threading;
using IRCSharp.EventArgs;

namespace IRCSharp.Example
{
    public class Program
    {
        public static void Main()
        {
            var client = new IRCClient(new IRCConfiguration
            {
                Username = "KiriTest",
                Hostname = "irc.onlinegamesnet.net",
                Port = 6667,
                Identd = "KiriTest",
                RealName = "Bim bam boom!"
            });

            client.Ready += Client_Ready;
            client.UserJoined += Client_UserJoined;

            client.Connect();

            Thread.Sleep(-1);
        }

        private static void Client_UserJoined(UserJoinedEventArgs e)
        {
            if (e.User == e.CurrentUser)
            {
                e.Channel.SendMessage("Bonjour. Je voudrais un café au lait, s'il vous plait.");
            }
        }

        private static void Client_Ready(ReadyEventArgs e)
        {
            e.Client.Send("JOIN #JsP");
        }
    }
}

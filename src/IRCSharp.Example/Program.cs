using System.Threading;
using System.Threading.Tasks;
using IRCSharp.EventArgs;
using IRCSharp.Qmmands;
using Qmmands;

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

            client.AddCommandService("!");

            client.Connect();

            Thread.Sleep(-1);
        }

        private static void Client_Ready(ReadyEventArgs e)
        {
            e.Client.Send("JOIN #JsP");
        }
    }

    public class FunModule : IRCModuleBase
    {
        [Command("ping")]
        public Task Ping()
        {
            Respond($"{Context.Author.Username}: Pong!");

            return Task.CompletedTask;
        }
    }
}

using System.Threading.Tasks;

namespace IRCSharp.Example
{
    public class Program
    {
        public static async Task Main()
        {
            var client = new IRCClient(new IRCConfiguration
            {
                Hostname = "irc.freenode.net",
                Nickname = "AllanOnFreenode"
            });

            await client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}

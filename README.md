# IRCSharp

IRCSharp is a library written in C# for IRC (Internal Relay Chat)

## Requirements:
- .NET Core 3.0

## Installation:
Compile from source:

1. Clone the repository: `git clone https://github.com/Kiritsu/IRCSharp`
2. Open `IRCSharp.sln`
3. Compile it with your favorite IDE (Visual Studio 2019 is prefered)
4. Get the dlls and add them to your project.

## Contributing:
If you want to contribute to the library which is not fully implemented yet, feel free to `Fork`, `Commit` and then `Pull Request`, or open new issues.

## Project used:
- Qmmands is an open source command framework and can be used with this project to create an IRC Bot: https://github.com/Quahu/Qmmands/

## Example:
This example uses `IRCSharp` and `IRCSharp.Qmmands`
```cs
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
```

When the bot starts and connect to your favorite IRC network, type `!ping`. It should respond with something similar to this:
`[02:00] <KiriTest> Kiritsu_: Pong!`

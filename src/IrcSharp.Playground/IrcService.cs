using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IrcSharp.Playground;

public class IrcService(IrcClient client, ILogger<IrcService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        client.OnReady += (_, token) 
            => client.SendRawMessageAsync("JOIN :#testallan", token);

        client.OnRawMessageReceived += (messageBytes, _) =>
        {
            logger.LogInformation("Received: {Message}", Encoding.UTF8.GetString(messageBytes.Span));
            return Task.CompletedTask;
        };

        client.OnRplISupportReceived += (args, _) =>
        {
            logger.LogInformation("Server capabilities: {Capabilities} - Prefix: {Prefix}",
                string.Join(", ", args.Capabilities), args.Origin?.Prefix);

            return Task.CompletedTask;
        };
        
        await client.ConnectAsync();
    }
}
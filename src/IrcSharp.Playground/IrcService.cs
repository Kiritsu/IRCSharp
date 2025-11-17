using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IrcSharp.Playground;

public class IrcService(IrcClient client, ILogger<IrcService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        client.OnReady += async (_, token) =>
        {
            await client.SendRawMessageAsync("JOIN :#testallan", token);
        };

        client.OnPing += (_, token) =>
        {
            logger.LogInformation("Ping received");
            return Task.CompletedTask;
        };

        client.OnUnknownMessage += (args, token) =>
        {
            logger.LogInformation("Received: {Message}", args.Message);
            return Task.CompletedTask;
        };
        
        await client.ConnectAsync();
    }
}
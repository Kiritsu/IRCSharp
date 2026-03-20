using IrcSharp.Events;
using IrcSharp.Internal;
using IrcSharp.Internal.Extensions;
using EventArgs = IrcSharp.Events.EventArgs;

namespace IrcSharp;

public sealed partial class IrcClient
{
    private Task HandleUnknownMessageAsync(RawIrcMessage message, CancellationToken cancellationToken)
    {
        return InvokeHandlerAsync(OnUnknownMessage, UnknownMessageEventArgs.Create(
            message.ToIrcMessage(), message.ToString()), cancellationToken);
    }
    
    private async Task HandlePingAsync(RawIrcMessage message, CancellationToken cancellationToken)
    {
        var pingToken = message.GetPingToken();
        await this.PongAsync(pingToken, cancellationToken).ConfigureAwait(false);
        
        await InvokeHandlerAsync(OnPing, PingEventArgs.Create(
            message.ToIrcMessage(), pingToken), 
            cancellationToken).ConfigureAwait(false);
    }
    
    private Task HandleRplWelcomeAsync(RawIrcMessage message, CancellationToken cancellationToken)
    {
        IsConnected = true;
        
        return InvokeHandlerAsync(OnWelcome, GenericEventArgs.Create(
            message.ToIrcMessage()), cancellationToken);
    }
    
    private Task HandleRplISupportAsync(RawIrcMessage message, CancellationToken cancellationToken)
    {
        IsConnected = true;
        
        var capabilities = new Dictionary<string, string?>();
        foreach (var rawCapability in message.EnumerateParameters())
        {
            var (param1, param2) = rawCapability.ParseParameter();
            capabilities.Add(param1, param2);
        }
        
        if (_parseServerCapabilities)
        {
            Capabilities ??= new IrcServerCapabilities();
            Capabilities.Append(capabilities);
        }
        
        return InvokeHandlerAsync(OnRplISupportReceived, RplIsupportEventArgs.Create(
            message.ToIrcMessage(), capabilities), cancellationToken);
    }
    
    private async Task HandleCapAsync(RawIrcMessage message, CancellationToken cancellationToken)
    {
        // CAP message format: CAP <client> <subcommand> [params]
        // The subcommand (LS, ACK, NAK, ...) is the second space-separated token in params, not in the command field.
        var paramsEnumerator = message.EnumerateParameters();
        ReadOnlyMemory<byte> subcommand = default;
        var paramIndex = 0;
        foreach (var param in paramsEnumerator)
        {
            if (paramIndex == 1)
            {
                subcommand = param;
                break;
            }
            paramIndex++;
        }

        if (subcommand.Span.SequenceEqual("LS"u8))
        {
            var serverCapabilities = new List<string>();

            var trailingSpaceEnumerator = new SeparatedByEnumerator(message.GetTrailing()[1..].ToArray(), ' ');
            foreach (var capability in trailingSpaceEnumerator)
            {
                var capabilityStr = capability.AsUtf8String();
                serverCapabilities.Add(capabilityStr);
            }
            
            if (_capabilityNegotiationVersion != null && _allowedCapabilities != null)
            {
                foreach (var capability in serverCapabilities)
                {
                    if (_allowedCapabilities.Contains(capability))
                    {
                        await SendRawMessageAsync($"CAP REQ :{capability}", cancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }
        
        await InvokeHandlerAsync(OnCapReceived, GenericEventArgs.Create(message.ToIrcMessage()), 
            cancellationToken).ConfigureAwait(false);
    }
    
    private Task InvokeHandlerAsync<TEventArgs>(
        Func<TEventArgs, CancellationToken, Task>? handler,
        TEventArgs args,
        CancellationToken cancellationToken) where TEventArgs : EventArgs
    {
        if (handler is null)
        {
            return Task.CompletedTask;
        }
        
        var invokeList = handler.GetInvocationList();
        var tasks = new Task[invokeList.Length];
        for (var index = 0; index < invokeList.Length; index++)
        {
            var @delegate = (Func<TEventArgs, CancellationToken, Task>)invokeList[index];
            tasks[index] = Task.Run(async () =>
            {
                try
                {
                    await @delegate(args, cancellationToken).ConfigureAwait(false);
                }
#pragma warning disable CA1031
                catch (Exception ex)
#pragma warning restore CA1031
                {
                    _logger.LogErrorWhileInvokingHandler(ex);
                }
            }, cancellationToken);
        }
        
        return Task.WhenAll(tasks);
    }
}
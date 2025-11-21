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
            _includeHighLevelMessage ? message.ToIrcMessage() : null, 
            message.ToString()), cancellationToken);
    }
    
    private Task HandlePingAsync(RawIrcMessage message, CancellationToken cancellationToken)
    {
        return InvokeHandlerAsync(OnPing, PingEventArgs.Create(
            _includeHighLevelMessage ? message.ToIrcMessage() : null, 
            message.GetPingToken()), cancellationToken);
    }
    
    private Task HandleRplWelcomeAsync(RawIrcMessage message, CancellationToken cancellationToken)
    {
        IsConnected = true;
        return InvokeHandlerAsync(OnReady, EmptyEventArgs.Instance, cancellationToken);
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
        
        return InvokeHandlerAsync(OnRplISupportReceived, ServerCapabilityEventArgs.Create(
            _includeHighLevelMessage ? message.ToIrcMessage() : null, 
            capabilities), cancellationToken);
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
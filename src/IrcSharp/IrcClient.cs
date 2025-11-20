using System.Buffers;
using System.Collections.Frozen;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using IrcSharp.Events;
using IrcSharp.Internal;
using IrcSharp.Internal.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IrcSharp;

/// <summary>
/// Represents an IRC client.
/// </summary>
public sealed partial class IrcClient : IAsyncDisposable
{
    private TcpClient? _tcpClient;
    private SslStream? _sslStream;
    private IrcMessageReader? _messageReader;
    private IrcMessageWriter? _messageWriter;
    private CancellationTokenSource? _cancellationTokenSource;
    
    private readonly bool _waitForHandlersBeforeNextMessage;
    private readonly bool _ignoreUnknownMessages;
    private readonly int _maximumMessageSize;

    private readonly string _host;
    private readonly int _port;
    private readonly bool _useSsl;
    private readonly bool _useSslWithNoValidation;
    private readonly string _username;
    private readonly string? _password;
    private readonly string? _realname;
    private readonly string? _identd;

    private readonly bool _parseServerCapabilities;
    
    private delegate Task HandlerDelegate(RawIrcMessage context, CancellationToken cancellationToken);
    private readonly FrozenDictionary<CommandKey, HandlerDelegate> _commandHandlers;
    
    private readonly ILogger<IrcClient> _logger;
    
#pragma warning disable CA1003
    /// <summary>
    /// Triggered when any message is received.
    /// </summary>
    /// <remarks>
    /// This event works independently from other events. When subscribed to, the received message is copied
    /// to its own buffer and the value for <see cref="_waitForHandlersBeforeNextMessage"/> is ignored.
    /// </remarks>
    public event Func<ReadOnlyMemory<byte>, CancellationToken, Task>? OnRawMessageReceived;
    
    /// <summary>
    /// Triggered when an unknown message is received.
    /// </summary>
    /// <remarks>
    /// Requires <see cref="IrcOptions.IgnoreUnknownMessages"/> to be set to false.
    /// </remarks>
    public event Func<UnknownMessageEventArgs, CancellationToken, Task>? OnUnknownMessage;
    
    /// <summary>
    /// Triggered when a PING message is received.
    /// </summary>
    /// <remarks>
    /// The <see cref="IrcClient"/> is automatically handling PONG responses.
    /// </remarks>
    public event Func<PingEventArgs, CancellationToken, Task>? OnPing;
    
    /// <summary>
    /// Triggered when a RPL_WELCOME (001) message is received.
    /// </summary>
    public event Func<EmptyEventArgs, CancellationToken, Task>? OnReady;
    
    /// <summary>
    /// Triggered when a RPL_ISUPPORT (005) message is received.
    /// </summary>
    public event Func<ServerCapabilityEventArgs, CancellationToken, Task>? OnRplISupportReceived;
#pragma warning restore CA1003

    /// <summary>
    /// Indicates whether the client is connected to the IRC Server.
    /// </summary>
    public bool IsConnected { get; private set; }

    /// <summary>
    /// Gets the server capabilities. Null if <see cref="IrcOptions.ParseServerCapabilities"/> is set to false.
    /// </summary>
    public IrcServerCapabilities? Capabilities { get; private set; }

    public IrcClient(ILogger<IrcClient> logger, IOptions<IrcOptions> options)
    {
        _logger = logger;
        
        _host = options.Value.Host;
        _port = options.Value.Port;
        _useSsl = options.Value.UseSsl;
        _useSslWithNoValidation = options.Value.UseSslWithNoValidation;
        _realname = options.Value.Realname;
        _identd = options.Value.Identd;
        _username = options.Value.Username;
        _password = options.Value.Password;
        
        _waitForHandlersBeforeNextMessage = options.Value.WaitForHandlersBeforeNextMessage;
        _ignoreUnknownMessages = options.Value.IgnoreUnknownMessages;
        _maximumMessageSize = options.Value.MaximumMessageSize;

        _parseServerCapabilities = options.Value.ParseServerCapabilities;
        
        _commandHandlers = new Dictionary<CommandKey, HandlerDelegate>
        {
            [CommandKey.FromString("PING")] = HandlePingAsync,
            [CommandKey.FromString("001")] = HandleRplWelcomeAsync,
            [CommandKey.FromString("005")] = HandleRplISupportAsync
        }.ToFrozenDictionary();
    }

    public async Task ConnectAsync()
    {
        if (_cancellationTokenSource != null)
        {
            throw new InvalidOperationException("The client is already connected or hasn't been stopped properly.");
        }
     
        await ResetAsync().ConfigureAwait(false);
        SubscribeInternalHandlers();
        
        _cancellationTokenSource = new CancellationTokenSource();
        _tcpClient = new TcpClient();

        await _tcpClient.ConnectAsync(_host, _port).ConfigureAwait(false);
        
        Stream? stream;
        if (_useSsl)
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                _cancellationTokenSource.Token, 
                timeoutCts.Token);
            
            stream = _sslStream = new SslStream(_tcpClient.GetStream(), false);
            await _sslStream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
            {
                TargetHost = _host,
                RemoteCertificateValidationCallback = ValidateServerCertificate
            }, linkedCts.Token).ConfigureAwait(false);
        }
        else
        {
            stream = _tcpClient.GetStream();
        }
        
        _messageReader = new IrcMessageReader(stream);
        _messageWriter = new IrcMessageWriter(stream);
        
        _ = Task.Run(() => ReceiveMessagesAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);

        if (!string.IsNullOrWhiteSpace(_password))
        {
            await this.PassAsync(_password, _cancellationTokenSource.Token).ConfigureAwait(false);
        }

        await this.NickAsync(_username, _cancellationTokenSource.Token).ConfigureAwait(false);
        await this.UserAsync(_identd ?? _username, _realname ?? Consts.DefaultRealname, _cancellationTokenSource.Token).ConfigureAwait(false);
    }

    // make an actual implem
    private bool ValidateServerCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
    {
        if (_useSslWithNoValidation)
        {
            return true;
        }
        
        if (sslPolicyErrors == SslPolicyErrors.None)
        {
            return true;
        }

        return false;
    }

    // todo: look for https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/performance/interpolated-string-handler
    public async Task SendRawMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        if (_messageWriter == null)
        {
            throw new InvalidOperationException("The client is not connected or ready.");
        }
        
        await _messageWriter.WriteAsync(message, _maximumMessageSize, cancellationToken).ConfigureAwait(false);
    }
    
    public async Task SendRawMessageAsync(ReadOnlyMemory<char> message, CancellationToken cancellationToken = default)
    {
        if (_messageWriter == null)
        {
            throw new InvalidOperationException("The client is not connected or ready.");
        }
        
        await _messageWriter.WriteAsync(message, _maximumMessageSize, cancellationToken).ConfigureAwait(false);
    }

    public async Task DisconnectAsync()
    {
        await ResetAsync().ConfigureAwait(false);
    }
    
    public ValueTask DisposeAsync()
    {
        return ResetAsync();
    }

    private async ValueTask ResetAsync()
    {
        IsConnected = false;
        UnsubscribeInternalHandlers();
        
        if (_sslStream != null)
        {
            await _sslStream.DisposeAsync().ConfigureAwait(false);
        }
        
        _tcpClient?.Dispose();
        _tcpClient = null;
        
        if (_messageReader != null)
        {
            await _messageReader.DisposeAsync().ConfigureAwait(false);
            _messageReader = null;
        }

        _messageWriter?.Dispose();
        _messageWriter = null;
        
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }
    
    private async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
    {
        if (_messageReader == null || _messageWriter == null)
        {
            throw new InvalidOperationException("The client is not connected or ready");
        }
        
        await foreach (var message in _messageReader.ReadMessagesAsync(_maximumMessageSize, cancellationToken).ConfigureAwait(false))
        {
            if (OnRawMessageReceived != null)
            {
                var messageSpan = message.AsSpan();
                var buffer = ArrayPool<byte>.Shared.Rent(messageSpan.Length);
                messageSpan.CopyTo(buffer);

                var anyMessageHandlerTask = OnRawMessageReceived(buffer.AsMemory(0, messageSpan.Length), cancellationToken)
                    .ConfigureAwait(false);
                
                _ = Task.Run(async () =>
                {
                    await anyMessageHandlerTask;
                    ArrayPool<byte>.Shared.Return(buffer);
                }, cancellationToken);
            }
            
            var key = new CommandKey(message.GetCommand());
            if (!_commandHandlers.TryGetValue(key, out var handler))
            {
                if (_ignoreUnknownMessages || OnUnknownMessage == null)
                {
                    IrcMessagePool.Return(message);
                    continue;
                }
                
                handler = HandleUnknownMessageAsync;
            }

            var handlerTask = handler(message, cancellationToken).ConfigureAwait(false);
            if (_waitForHandlersBeforeNextMessage)
            {
                await handlerTask;
                IrcMessagePool.Return(message);
            }
            else
            {
                _ = Task.Run(async () =>
                {
                    await handlerTask;
                    IrcMessagePool.Return(message);
                }, cancellationToken);
            }
        }
    }
    
    private void SubscribeInternalHandlers()
    {
        OnPing += PongAsync;

        if (_parseServerCapabilities)
        {
            OnRplISupportReceived += OnOnRplISupportReceived;
        }
    }

    private void UnsubscribeInternalHandlers()
    {
        OnPing -= PongAsync;
        
        if (_parseServerCapabilities)
        {
            OnRplISupportReceived -= OnOnRplISupportReceived;
        }
    }
    
    private Task PongAsync(PingEventArgs args, CancellationToken cancellationToken)
    {
        return this.PongAsync(args.TrailingValue, cancellationToken);
    }
    
    private Task OnOnRplISupportReceived(ServerCapabilityEventArgs args, CancellationToken cancellationToken)
    {
        Capabilities ??= new IrcServerCapabilities();
        Capabilities.Append(args.Capabilities);
        
        return Task.CompletedTask;
    }
}
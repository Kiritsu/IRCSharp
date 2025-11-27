namespace IrcSharp;

/// <summary>
/// Represents the different options that can be used to configure the IRC client.
/// </summary>
public class IrcOptions
{
    /// <summary>
    /// Gets or sets the host to connect to.
    /// </summary>
    public required string Host { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use SSL.
    /// </summary>
    public bool UseSsl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to accept any certificate.
    /// </summary>
    public bool UseSslWithNoValidation { get; set; }

    /// <summary>
    /// Gets or sets the port to connect to.
    /// </summary>
    public required int Port { get; set; }

    /// <summary>
    /// Gets or sets the identd to use.
    /// </summary>
    public string? Identd { get; set; }
    
    /// <summary>
    /// Gets or sets the realname to use.
    /// </summary>
    public string? Realname { get; set; }

    /// <summary>
    /// Gets or sets the username to use.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the password to use.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to wait for handlers to be called before processing the next message that is received by the client.
    /// </summary>
    public bool WaitForHandlersBeforeNextMessage { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to ignore unknown messages.
    /// </summary>
    /// <remarks>
    /// Ignoring unknown messages means that it will not parse the message and will not call the <see cref="IrcClient.OnUnknownMessage"/> handler. 
    /// </remarks>
    public bool IgnoreUnknownMessages { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum send message size.
    /// </summary>
    /// <remarks>
    /// The default value is <see cref="IrcSharpConsts.StandardMaximumSendMessageSize"/>. It can be overriden if the remote IRC server support longer messages.
    /// </remarks>
    public int MaximumSendMessageSize { get; set; } = IrcSharpConsts.StandardMaximumSendMessageSize;
    
    /// <summary>
    /// Gets or sets the maximum receive message size.
    /// </summary>
    /// <remarks>
    /// The default value is <see cref="IrcSharpConsts.StandardMaximumReceiveMessageSize"/>. It can be overriden if the remote IRC server support longer messages.
    /// </remarks>
    public int MaximumReceiveMessageSize { get; set; } = IrcSharpConsts.StandardMaximumReceiveMessageSize;

    /// <summary>
    /// Gets or sets whether to parse the server capabilities.
    /// </summary>
    public bool ParseServerCapabilities { get; set; }

    /// <summary>
    /// Gets or sets whether to include the high level <see cref="IrcMessage"/> in the events"/>.
    /// </summary>
    public bool IncludeHighLevelMessage { get; set; }
}
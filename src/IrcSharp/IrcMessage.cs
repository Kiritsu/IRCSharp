using System.Collections.Immutable;

namespace IrcSharp;

public sealed class IrcMessage
{
    public ImmutableArray<string> Tags { get; init; } = [];
    
    public string Prefix { get; init; } = string.Empty;

    public string Command { get; init; } = string.Empty;

    public ImmutableArray<string> Parameters { get; init; } = [];

    public string? Trailing { get; init; }
    
    internal IrcMessage() {}
}
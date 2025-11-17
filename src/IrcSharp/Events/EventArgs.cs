namespace IrcSharp.Events;

public abstract class EventArgs;

public sealed class EmptyEventArgs : EventArgs
{
    public static readonly EmptyEventArgs Instance = new();
}

public sealed class PingEventArgs : EventArgs
{
    public required string? TrailingValue { get; init; }
    
    public static PingEventArgs Create(string? trailingValue)
    {
        return new PingEventArgs { TrailingValue = trailingValue };
    }
}

public sealed class UnknownMessageEventArgs : EventArgs
{
    public required string Message { get; init; }
    
    public static UnknownMessageEventArgs Create(string message)
    {
        return new UnknownMessageEventArgs { Message = message };
    }
}
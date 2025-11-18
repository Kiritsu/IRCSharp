using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using IrcSharp.Internal;

namespace IrcSharp.Benchmark;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class IrcMessageBenchmarks
{
    private RawIrcMessage _simpleMessage = null!;
    private RawIrcMessage _withPrefixAndParams = null!;
    private RawIrcMessage _completeMessage = null!;
    private RawIrcMessage _longCompleteMessage = null!;
    
    // For high-volume tests
    private string[] _messagePool = null!;
    private const int MessagePoolSize = 100_000;

    // Test messages
    private const string SimpleMessage = "PING";
    private const string WithPrefixAndParams = ":server.com PRIVMSG #channel";
    private const string CompleteMessage = "@badge=1;color=#FF0000 :nick!user@host PRIVMSG #channel :Hello world!";
    private const string LongCompleteMessage = "@badge-info=subscriber/12;badges=subscriber/12;color=#FF0000;display-name=TestUser;emotes=;flags=;id=12345678-1234-1234-1234-123456789012;mod=0;room-id=123456789;subscriber=1;tmi-sent-ts=1234567890123;turbo=0;user-id=87654321;user-type= :testuser!testuser@testuser.tmi.twitch.tv PRIVMSG #channelname :This is a longer message with more content to benchmark performance with realistic data";

    [GlobalSetup]
    public void Setup()
    {
        _simpleMessage = CreateMessage(SimpleMessage);
        _withPrefixAndParams = CreateMessage(WithPrefixAndParams);
        _completeMessage = CreateMessage(CompleteMessage);
        _longCompleteMessage = CreateMessage(LongCompleteMessage);
        
        // Setup message pool for high-volume tests
        SetupMessagePool();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        IrcMessagePool.Return(_simpleMessage);
        IrcMessagePool.Return(_withPrefixAndParams);
        IrcMessagePool.Return(_completeMessage);
        IrcMessagePool.Return(_longCompleteMessage);
    }

    private void SetupMessagePool()
    {
        _messagePool = new string[MessagePoolSize];
        
        // Create a realistic mix of IRC messages
        var templates = new[]
        {
            "PING :server{0}.example.com",
            ":server{0}.example.com PONG server{0}.example.com :LAG123456789",
            ":user{0}!user{0}@user{0}.tmi.twitch.tv JOIN #channel{1}",
            ":user{0}!user{0}@user{0}.tmi.twitch.tv PART #channel{1}",
            "@badge-info=;badges=broadcaster/1 :user{0}!user{0}@user{0}.tmi.twitch.tv PRIVMSG #channel{1} :Hello everyone!",
            "@badge-info=subscriber/12;badges=subscriber/12;color=#FF0000 :user{0}!user{0}@user{0}.tmi.twitch.tv PRIVMSG #channel{1} :This is message number {2}",
            ":user{0}!user{0}@user{0}.tmi.twitch.tv PRIVMSG #channel{1} :Simple message {2}",
            "@emotes=25:0-4;color=#0000FF :user{0}!user{0}@user{0}.tmi.twitch.tv PRIVMSG #channel{1} :Kappa test message {2}",
            ":server{0}.example.com 353 user{0} = #channel{1} :user1 user2 user3 user4 user5",
            ":server{0}.example.com 366 user{0} #channel{1} :End of /NAMES list",
            ":user{0}!user{0}@user{0}.tmi.twitch.tv MODE #channel{1} +o user{2}",
            ":user{0}!user{0}@user{0}.tmi.twitch.tv KICK #channel{1} user{2} :Violation of rules",
            "@msg-id=sub :tmi.twitch.tv USERNOTICE #channel{1} :user{0} just subscribed!",
            ":user{0}!user{0}@host.example.com QUIT :Leaving server",
            "@room-id=12345;target-user-id=67890 :tmi.twitch.tv CLEARCHAT #channel{1} :user{0}",
        };

        for (int i = 0; i < MessagePoolSize; i++)
        {
            var template = templates[i % templates.Length];
            _messagePool[i] = string.Format(template, i % 1000, i % 50, i);
        }
    }

    private static RawIrcMessage CreateMessage(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        var sequence = new ReadOnlySequence<byte>(bytes);
        return IrcMessagePool.Rent(sequence);
    }

    #region Constructor Benchmarks

    [Benchmark]
    public void Constructor_SimpleMessage()
    {
        var bytes = Encoding.UTF8.GetBytes(SimpleMessage);
        var sequence = new ReadOnlySequence<byte>(bytes);
        var message = IrcMessagePool.Rent(sequence);
        IrcMessagePool.Return(message);
    }

    [Benchmark]
    public void Constructor_CompleteMessage()
    {
        var bytes = Encoding.UTF8.GetBytes(CompleteMessage);
        var sequence = new ReadOnlySequence<byte>(bytes);
        var message = IrcMessagePool.Rent(sequence);
        IrcMessagePool.Return(message);
    }

    [Benchmark]
    public void Constructor_LongMessage()
    {
        var bytes = Encoding.UTF8.GetBytes(LongCompleteMessage);
        var sequence = new ReadOnlySequence<byte>(bytes);
        var message = IrcMessagePool.Rent(sequence);
        IrcMessagePool.Return(message);
    }

    #endregion

    #region AsSpan Benchmarks

    [Benchmark]
    public ReadOnlySpan<byte> AsSpan_Simple()
    {
        return _simpleMessage.AsSpan();
    }

    [Benchmark]
    public ReadOnlySpan<byte> AsSpan_Complete()
    {
        return _completeMessage.AsSpan();
    }

    [Benchmark]
    public ReadOnlySpan<byte> AsSpan_Long()
    {
        return _longCompleteMessage.AsSpan();
    }

    #endregion

    #region GetTags Benchmarks

    [Benchmark]
    public ReadOnlySpan<byte> GetTags_NoTags()
    {
        return _simpleMessage.GetTags();
    }

    [Benchmark]
    public ReadOnlySpan<byte> GetTags_WithTags()
    {
        return _completeMessage.GetTags();
    }

    [Benchmark]
    public ReadOnlySpan<byte> GetTags_LongTags()
    {
        return _longCompleteMessage.GetTags();
    }

    #endregion

    #region GetPrefix Benchmarks

    [Benchmark]
    public ReadOnlySpan<byte> GetPrefix_NoPrefix()
    {
        return _simpleMessage.GetPrefix();
    }

    [Benchmark]
    public ReadOnlySpan<byte> GetPrefix_WithPrefix()
    {
        return _withPrefixAndParams.GetPrefix();
    }

    [Benchmark]
    public ReadOnlySpan<byte> GetPrefix_CompleteMessage()
    {
        return _completeMessage.GetPrefix();
    }

    [Benchmark]
    public ReadOnlySpan<byte> GetPrefix_LongMessage()
    {
        return _longCompleteMessage.GetPrefix();
    }

    #endregion

    #region GetCommand Benchmarks

    [Benchmark]
    public ReadOnlySpan<byte> GetCommand_Simple()
    {
        return _simpleMessage.GetCommand();
    }

    [Benchmark]
    public ReadOnlySpan<byte> GetCommand_WithPrefix()
    {
        return _withPrefixAndParams.GetCommand();
    }

    [Benchmark]
    public ReadOnlySpan<byte> GetCommand_Complete()
    {
        return _completeMessage.GetCommand();
    }

    [Benchmark]
    public ReadOnlySpan<byte> GetCommand_Long()
    {
        return _longCompleteMessage.GetCommand();
    }

    #endregion

    #region GetParams Benchmarks

    [Benchmark]
    public ReadOnlySpan<byte> GetParams_NoParams()
    {
        return _simpleMessage.GetParams();
    }

    [Benchmark]
    public ReadOnlySpan<byte> GetParams_WithParams()
    {
        return _withPrefixAndParams.GetParams();
    }

    [Benchmark]
    public ReadOnlySpan<byte> GetParams_Complete()
    {
        return _completeMessage.GetParams();
    }

    [Benchmark]
    public ReadOnlySpan<byte> GetParams_Long()
    {
        return _longCompleteMessage.GetParams();
    }

    #endregion

    #region GetTrailing Benchmarks

    [Benchmark]
    public ReadOnlySpan<byte> GetTrailing_NoTrailing()
    {
        return _simpleMessage.GetTrailing();
    }

    [Benchmark]
    public ReadOnlySpan<byte> GetTrailing_WithTrailing()
    {
        return _completeMessage.GetTrailing();
    }

    [Benchmark]
    public ReadOnlySpan<byte> GetTrailing_Long()
    {
        return _longCompleteMessage.GetTrailing();
    }

    #endregion

    #region Full Parse Benchmarks

    [Benchmark]
    public void FullParse_Simple()
    {
        _ = _simpleMessage.GetTags();
        _ = _simpleMessage.GetPrefix();
        _ = _simpleMessage.GetCommand();
        _ = _simpleMessage.GetParams();
        _ = _simpleMessage.GetTrailing();
    }

    [Benchmark]
    public void FullParse_Complete()
    {
        _ = _completeMessage.GetTags();
        _ = _completeMessage.GetPrefix();
        _ = _completeMessage.GetCommand();
        _ = _completeMessage.GetParams();
        _ = _completeMessage.GetTrailing();
    }

    [Benchmark]
    public void FullParse_Long()
    {
        _ = _longCompleteMessage.GetTags();
        _ = _longCompleteMessage.GetPrefix();
        _ = _longCompleteMessage.GetCommand();
        _ = _longCompleteMessage.GetParams();
        _ = _longCompleteMessage.GetTrailing();
    }

    #endregion

    #region Complete Lifecycle Benchmarks

    [Benchmark]
    public void CompleteLifecycle_Simple()
    {
        var bytes = Encoding.UTF8.GetBytes(SimpleMessage);
        var sequence = new ReadOnlySequence<byte>(bytes);
        var message = IrcMessagePool.Rent(sequence);
        _ = message.GetCommand();
        IrcMessagePool.Return(message);
    }

    [Benchmark]
    public void CompleteLifecycle_Complete()
    {
        var bytes = Encoding.UTF8.GetBytes(CompleteMessage);
        var sequence = new ReadOnlySequence<byte>(bytes);
        var message = IrcMessagePool.Rent(sequence);
        _ = message.GetTags();
        _ = message.GetPrefix();
        _ = message.GetCommand();
        _ = message.GetParams();
        _ = message.GetTrailing();
        IrcMessagePool.Return(message);
    }

    [Benchmark]
    public void CompleteLifecycle_Long()
    {
        var bytes = Encoding.UTF8.GetBytes(LongCompleteMessage);
        var sequence = new ReadOnlySequence<byte>(bytes);
        var message = IrcMessagePool.Rent(sequence);
        _ = message.GetTags();
        _ = message.GetPrefix();
        _ = message.GetCommand();
        _ = message.GetParams();
        _ = message.GetTrailing();
        IrcMessagePool.Return(message);
    }

    #endregion

    #region High Volume Benchmarks

    [Benchmark]
    [Arguments(100_000)]
    public long HighVolume_ParseMessages(int messageCount)
    {
        long totalBytes = 0;
        
        for (int i = 0; i < messageCount; i++)
        {
            var messageText = _messagePool[i % MessagePoolSize];
            var bytes = Encoding.UTF8.GetBytes(messageText);
            var sequence = new ReadOnlySequence<byte>(bytes);
            
            var message = IrcMessagePool.Rent(sequence);
            
            // Parse all components (realistic usage)
            _ = message.GetTags();
            _ = message.GetPrefix();
            _ = message.GetCommand();
            _ = message.GetParams();
            _ = message.GetTrailing();
            
            totalBytes += bytes.Length;
            
            IrcMessagePool.Return(message);
        }
        
        return totalBytes;
    }

    [Benchmark]
    [Arguments(100_000)]
    public long HighVolume_ParseCommandOnly(int messageCount)
    {
        long totalBytes = 0;
        
        for (int i = 0; i < messageCount; i++)
        {
            var messageText = _messagePool[i % MessagePoolSize];
            var bytes = Encoding.UTF8.GetBytes(messageText);
            var sequence = new ReadOnlySequence<byte>(bytes);
            
            var message = IrcMessagePool.Rent(sequence);
            
            // Only parse command (minimal parsing)
            _ = message.GetCommand();
            
            totalBytes += bytes.Length;
            
            IrcMessagePool.Return(message);
        }
        
        return totalBytes;
    }

    [Benchmark]
    [Arguments(250_000)]
    public long HighVolume_250k_Messages(int messageCount)
    {
        long totalBytes = 0;
        
        for (int i = 0; i < messageCount; i++)
        {
            var messageText = _messagePool[i % MessagePoolSize];
            var bytes = Encoding.UTF8.GetBytes(messageText);
            var sequence = new ReadOnlySequence<byte>(bytes);
            
            var message = IrcMessagePool.Rent(sequence);
            
            _ = message.GetTags();
            _ = message.GetPrefix();
            _ = message.GetCommand();
            _ = message.GetParams();
            _ = message.GetTrailing();
            
            totalBytes += bytes.Length;
            
            IrcMessagePool.Return(message);
        }
        
        return totalBytes;
    }

    [Benchmark]
    [Arguments(500_000)]
    public long HighVolume_500k_Messages(int messageCount)
    {
        long totalBytes = 0;
        
        for (int i = 0; i < messageCount; i++)
        {
            var messageText = _messagePool[i % MessagePoolSize];
            var bytes = Encoding.UTF8.GetBytes(messageText);
            var sequence = new ReadOnlySequence<byte>(bytes);
            
            var message = IrcMessagePool.Rent(sequence);
            
            _ = message.GetTags();
            _ = message.GetPrefix();
            _ = message.GetCommand();
            _ = message.GetParams();
            _ = message.GetTrailing();
            
            totalBytes += bytes.Length;
            
            IrcMessagePool.Return(message);
        }
        
        return totalBytes;
    }

    [Benchmark]
    [Arguments(100_000)]
    public long HighVolume_ReuseBytes(int messageCount)
    {
        long totalBytes = 0;
        
        // Pre-allocate byte arrays to reduce encoding overhead
        var preEncodedMessages = new byte[MessagePoolSize][];
        for (int i = 0; i < MessagePoolSize; i++)
        {
            preEncodedMessages[i] = Encoding.UTF8.GetBytes(_messagePool[i]);
        }
        
        for (int i = 0; i < messageCount; i++)
        {
            var bytes = preEncodedMessages[i % MessagePoolSize];
            var sequence = new ReadOnlySequence<byte>(bytes);
            
            var message = IrcMessagePool.Rent(sequence);
            
            _ = message.GetTags();
            _ = message.GetPrefix();
            _ = message.GetCommand();
            _ = message.GetParams();
            _ = message.GetTrailing();
            
            totalBytes += bytes.Length;
            
            IrcMessagePool.Return(message);
        }
        
        return totalBytes;
    }

    #endregion
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<IrcMessageBenchmarks>();
    }
}
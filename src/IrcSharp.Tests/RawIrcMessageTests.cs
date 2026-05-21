using System.Buffers;
using System.Text;
using IrcSharp.Internal;

namespace IrcSharp.Tests;

public sealed class RawIrcMessageTests : IDisposable
{
    private readonly List<RawIrcMessage> _messages = [];

    public void Dispose()
    {
        foreach (var message in _messages)
        {
            IrcMessagePool.Return(message);
        }
        
        _messages.Clear();
    }

    private RawIrcMessage CreateMessage(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        var sequence = new ReadOnlySequence<byte>(bytes);
        var message = IrcMessagePool.Rent(sequence);
        _messages.Add(message);
        return message;
    }

    private static string SpanToString(ReadOnlySpan<byte> span)
    {
        return Encoding.UTF8.GetString(span);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidSequence_CopiesDataCorrectly()
    {
        // Arrange
        var text = "PING :server";

        // Act
        var message = CreateMessage(text);

        // Assert
        Assert.Equal(text, SpanToString(message.AsSpan()));
    }

    #endregion

    #region GetTags Tests

    [Fact]
    public void GetTags_WithValidTags_ReturnsTagsWithoutAtSymbol()
    {
        // Arrange
        var message = CreateMessage("@key1=value1;key2=value2 COMMAND");

        // Act
        var tags = message.GetTags();

        // Assert
        Assert.Equal("key1=value1;key2=value2", SpanToString(tags));
    }

    [Fact]
    public void GetTags_WithoutTags_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage("COMMAND");

        // Act
        var tags = message.GetTags();

        // Assert
        Assert.True(tags.IsEmpty);
    }

    [Fact]
    public void GetTags_WithPrefixOnly_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage(":prefix COMMAND");

        // Act
        var tags = message.GetTags();

        // Assert
        Assert.True(tags.IsEmpty);
    }

    [Fact]
    public void GetTags_WithTagsAndPrefix_ReturnsOnlyTags()
    {
        // Arrange
        var message = CreateMessage("@tag1=val1 :prefix COMMAND");

        // Act
        var tags = message.GetTags();

        // Assert
        Assert.Equal("tag1=val1", SpanToString(tags));
    }

    #endregion

    #region GetPrefix Tests

    [Fact]
    public void GetPrefix_WithValidPrefix_ReturnsPrefixWithoutColon()
    {
        // Arrange
        var message = CreateMessage(":nick!user@host COMMAND");

        // Act
        var prefix = message.GetPrefix();

        // Assert
        Assert.Equal("nick!user@host", SpanToString(prefix));
    }

    [Fact]
    public void GetPrefix_WithoutPrefix_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage("COMMAND");

        // Act
        var prefix = message.GetPrefix();

        // Assert
        Assert.True(prefix.IsEmpty);
    }

    [Fact]
    public void GetPrefix_WithTagsAndPrefix_ReturnsOnlyPrefix()
    {
        // Arrange
        var message = CreateMessage("@tag1=val1 :nick!user@host COMMAND");

        // Act
        var prefix = message.GetPrefix();

        // Assert
        Assert.Equal("nick!user@host", SpanToString(prefix));
    }

    [Fact]
    public void GetPrefix_WithOnlyTags_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage("@tags COMMAND");

        // Act
        var prefix = message.GetPrefix();

        // Assert
        Assert.True(prefix.IsEmpty);
    }

    [Fact]
    public void GetPrefix_AfterTags_WithoutPrefixMarker_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage("@tags COMMAND");

        // Act
        var prefix = message.GetPrefix();

        // Assert
        Assert.True(prefix.IsEmpty);
    }

    #endregion

    #region GetCommand Tests

    [Fact]
    public void GetCommand_WithOnlyCommand_ReturnsCommand()
    {
        // Arrange
        var message = CreateMessage("PRIVMSG");

        // Act
        var command = message.GetCommand();

        // Assert
        Assert.Equal("PRIVMSG", SpanToString(command));
    }

    [Fact]
    public void GetCommand_WithCommandAndParams_ReturnsCommand()
    {
        // Arrange
        var message = CreateMessage("PRIVMSG #channel");

        // Act
        var command = message.GetCommand();

        // Assert
        Assert.Equal("PRIVMSG", SpanToString(command));
    }

    [Fact]
    public void GetCommand_WithPrefixAndCommand_ReturnsCommand()
    {
        // Arrange
        var message = CreateMessage(":prefix PRIVMSG #channel");

        // Act
        var command = message.GetCommand();

        // Assert
        Assert.Equal("PRIVMSG", SpanToString(command));
    }

    [Fact]
    public void GetCommand_WithTagsPrefixAndCommand_ReturnsCommand()
    {
        // Arrange
        var message = CreateMessage("@tags :prefix PRIVMSG #channel");

        // Act
        var command = message.GetCommand();

        // Assert
        Assert.Equal("PRIVMSG", SpanToString(command));
    }

    [Fact]
    public void GetCommand_WithOnlyTagsAndCommand_ReturnsCommand()
    {
        // Arrange
        var message = CreateMessage("@tags PRIVMSG #channel");

        // Act
        var command = message.GetCommand();

        // Assert
        Assert.Equal("PRIVMSG", SpanToString(command));
    }

    [Fact]
    public void GetCommand_WithEmptyMessage_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage("");

        // Act
        var command = message.GetCommand();

        // Assert
        Assert.True(command.IsEmpty);
    }

    [Fact]
    public void GetCommand_NumericCommand_ReturnsCorrectly()
    {
        // Arrange
        var message = CreateMessage(":server 001 nick :Welcome");

        // Act
        var command = message.GetCommand();

        // Assert
        Assert.Equal("001", SpanToString(command));
    }

    #endregion
    
    #region GetParams Tests

    [Fact]
    public void GetParams_WithCommandAndParams_ReturnsParams()
    {
        // Arrange
        var message = CreateMessage("PRIVMSG #channel");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.Equal("#channel", SpanToString(parameters));
    }

    [Fact]
    public void GetParams_WithCommandParamsAndTrailing_ReturnsParamsOnly()
    {
        // Arrange
        var message = CreateMessage("PRIVMSG #channel :Hello world");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.Equal("#channel", SpanToString(parameters));
    }

    [Fact]
    public void GetParams_WithPrefixCommandAndParams_ReturnsParams()
    {
        // Arrange
        var message = CreateMessage(":server PRIVMSG #channel");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.Equal("#channel", SpanToString(parameters));
    }

    [Fact]
    public void GetParams_WithTagsPrefixCommandAndParams_ReturnsParams()
    {
        // Arrange
        var message = CreateMessage("@tag1=val1 :server PRIVMSG #channel");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.Equal("#channel", SpanToString(parameters));
    }

    [Fact]
    public void GetParams_CompleteMessage_ReturnsParams()
    {
        // Arrange
        var message = CreateMessage("@tag1=val1 :nick!user@host PRIVMSG #channel :Hello");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.Equal("#channel", SpanToString(parameters));
    }

    [Fact]
    public void GetParams_WithOnlyCommand_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage("PING");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.True(parameters.IsEmpty);
    }

    [Fact]
    public void GetParams_WithCommandAndOnlyTrailing_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage("PING :server.com");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.True(parameters.IsEmpty);
    }

    [Fact]
    public void GetParams_WithMultipleParams_ReturnsAllParams()
    {
        // Arrange
        var message = CreateMessage("MODE #channel +o user");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.Equal("#channel +o user", SpanToString(parameters));
    }

    [Fact]
    public void GetParams_WithMultipleParamsAndTrailing_ReturnsParamsOnly()
    {
        // Arrange
        var message = CreateMessage("MODE #channel +o user :trailing text");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.Equal("#channel +o user", SpanToString(parameters));
    }

    [Fact]
    public void GetParams_NumericReplyWithParams_ReturnsParams()
    {
        // Arrange
        var message = CreateMessage(":server 001 nick :Welcome message");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.Equal("nick", SpanToString(parameters));
    }

    [Fact]
    public void GetParams_WithEmptyMessage_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage("");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.True(parameters.IsEmpty);
    }

    [Fact]
    public void GetParams_WithOnlyTags_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage("@tag1=val1");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.True(parameters.IsEmpty);
    }

    [Fact]
    public void GetParams_WithOnlyPrefix_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage(":server");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.True(parameters.IsEmpty);
    }

    [Fact]
    public void GetParams_JoinCommand_ReturnsChannel()
    {
        // Arrange
        var message = CreateMessage(":nick!user@host JOIN #channel");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.Equal("#channel", SpanToString(parameters));
    }

    [Fact]
    public void GetParams_KickCommand_ReturnsChannelAndUser()
    {
        // Arrange
        var message = CreateMessage(":op!user@host KICK #channel baduser :Reason for kick");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.Equal("#channel baduser", SpanToString(parameters));
    }
    
    [Fact]
    public void GetParams_WithOnlyTagsAndCommand_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage("@tags PING");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.True(parameters.IsEmpty);
    }

    [Fact]
    public void GetParams_LongParameterList_ReturnsAllParams()
    {
        // Arrange
        var message = CreateMessage("COMMAND p1 p2 p3 p4 p5 p6 p7 p8 p9 p10 p11 p12 p13 p14");

        // Act
        var parameters = message.GetParams();

        // Assert
        Assert.Equal("p1 p2 p3 p4 p5 p6 p7 p8 p9 p10 p11 p12 p13 p14", SpanToString(parameters));
    }

    #endregion
    
    #region GetTrailing Tests

    [Fact]
    public void GetTrailing_WithCommandAndTrailing_ReturnsTrailingWithoutColon()
    {
        // Arrange
        var message = CreateMessage("PRIVMSG :Hello world");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.Equal("Hello world", SpanToString(trailing));
    }

    [Fact]
    public void GetTrailing_WithCommandParamsAndTrailing_ReturnsTrailingWithoutColon()
    {
        // Arrange
        var message = CreateMessage("PRIVMSG #channel :Hello world");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.Equal("Hello world", SpanToString(trailing));
    }

    [Fact]
    public void GetTrailing_WithPrefixCommandParamsAndTrailing_ReturnsTrailing()
    {
        // Arrange
        var message = CreateMessage(":nick!user@host PRIVMSG #channel :Hello world");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.Equal("Hello world", SpanToString(trailing));
    }

    [Fact]
    public void GetTrailing_CompleteMessage_ReturnsTrailing()
    {
        // Arrange
        var message = CreateMessage("@tag1=val1 :nick!user@host PRIVMSG #channel :Hello world");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.Equal("Hello world", SpanToString(trailing));
    }

    [Fact]
    public void GetTrailing_WithOnlyCommand_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage("PING");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.True(trailing.IsEmpty);
    }

    [Fact]
    public void GetTrailing_WithCommandAndParams_NoTrailing_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage("PRIVMSG #channel");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.True(trailing.IsEmpty);
    }

    [Fact]
    public void GetTrailing_WithEmptyMessage_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage("");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.True(trailing.IsEmpty);
    }

    [Fact]
    public void GetTrailing_TrailingWithSpaces_PreservesSpaces()
    {
        // Arrange
        var message = CreateMessage("PRIVMSG #channel :Hello   world   test");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.Equal("Hello   world   test", SpanToString(trailing));
    }

    [Fact]
    public void GetTrailing_TrailingWithColons_PreservesColons()
    {
        // Arrange
        var message = CreateMessage("PRIVMSG #channel :Time is 12:30:45");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.Equal("Time is 12:30:45", SpanToString(trailing));
    }
    
    [Fact]
    public void GetTrailing_NumericReply_ReturnsTrailing()
    {
        // Arrange
        var message = CreateMessage(":server 001 nick :Welcome to the IRC Network");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.Equal("Welcome to the IRC Network", SpanToString(trailing));
    }

    [Fact]
    public void GetTrailing_EmptyTrailing_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage("PRIVMSG #channel :");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.True(trailing.IsEmpty);
    }

    [Fact]
    public void GetTrailing_WithOnlyTagsAndCommand_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage("@tags COMMAND");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.True(trailing.IsEmpty);
    }

    [Fact]
    public void GetTrailing_WithOnlyPrefixAndCommand_ReturnsEmpty()
    {
        // Arrange
        var message = CreateMessage(":prefix COMMAND");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.True(trailing.IsEmpty);
    }

    [Fact]
    public void GetTrailing_WithSpecialCharacters_PreservesCharacters()
    {
        // Arrange
        var message = CreateMessage("PRIVMSG #channel :Hello! @user #hashtag $money & more");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.Equal("Hello! @user #hashtag $money & more", SpanToString(trailing));
    }

    [Fact]
    public void GetTrailing_WithUnicodeCharacters_PreservesUnicode()
    {
        // Arrange
        var message = CreateMessage("PRIVMSG #channel :Hello 👋 世界 🌍");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.Equal("Hello 👋 世界 🌍", SpanToString(trailing));
    }

    [Fact]
    public void GetTrailing_LongTrailingMessage_ReturnsFullMessage()
    {
        // Arrange
        var longText = new string('a', 400);
        var message = CreateMessage($"PRIVMSG #channel :{longText}");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.Equal(longText, SpanToString(trailing));
    }

    [Fact]
    public void GetTrailing_WithLeadingSpaceInTrailing_PreservesLeadingSpace()
    {
        // Arrange
        var message = CreateMessage("PRIVMSG #channel : Leading space");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.Equal(" Leading space", SpanToString(trailing));
    }

    [Fact]
    public void GetTrailing_WithTrailingSpaceInTrailing_PreservesTrailingSpace()
    {
        // Arrange
        var message = CreateMessage("PRIVMSG #channel :Trailing space ");

        // Act
        var trailing = message.GetTrailing();

        // Assert
        Assert.Equal("Trailing space ", SpanToString(trailing));
    }

    #endregion
    
    #region Integration Tests
    
    [Fact]
    public void GetAllComponents_CompleteMessage_AllComponentsParsedCorrectly()
    {
        // Arrange
        var message = CreateMessage("@badge=1;color=#FF0000 :nick!user@host PRIVMSG #channel :Hello world!");

        // Act
        var tags = message.GetTags();
        var prefix = message.GetPrefix();
        var command = message.GetCommand();
        var parameters = message.GetParams();
        var trailing = message.GetTrailing();

        // Assert
        Assert.Equal("badge=1;color=#FF0000", SpanToString(tags));
        Assert.Equal("nick!user@host", SpanToString(prefix));
        Assert.Equal("PRIVMSG", SpanToString(command));
        Assert.Equal("#channel", SpanToString(parameters));
        Assert.Equal("Hello world!", SpanToString(trailing));
    }

    [Fact]
    public void GetAllComponents_MinimalMessage_OnlyTrailingPresent()
    {
        // Arrange
        var message = CreateMessage("COMMAND :trailing text");

        // Act
        var tags = message.GetTags();
        var prefix = message.GetPrefix();
        var command = message.GetCommand();
        var parameters = message.GetParams();
        var trailing = message.GetTrailing();

        // Assert
        Assert.True(tags.IsEmpty);
        Assert.True(prefix.IsEmpty);
        Assert.Equal("COMMAND", SpanToString(command));
        Assert.True(parameters.IsEmpty);
        Assert.Equal("trailing text", SpanToString(trailing));
    }

    #endregion
}
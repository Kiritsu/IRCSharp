using System.Text;
using IrcSharp.Internal.Extensions;

namespace IrcSharp.Tests;

public class SeparatedByEnumeratorTests
{
    [Fact]
    public void MoveNext_EmptyArray_ReturnsFalse()
    {
        var enumerator = new SeparatedByEnumerator([], ' ');

        bool result = enumerator.MoveNext();

        Assert.False(result);
        Assert.Equal(ReadOnlyMemory<byte>.Empty, enumerator.Current);
    }

    [Fact]
    public void MoveNext_SingleParameter_ReturnsCorrectValue()
    {
        byte[] data = "param1"u8.ToArray();
        var enumerator = new SeparatedByEnumerator(data, ' ');

        Assert.True(enumerator.MoveNext());
        Assert.Equal("param1", Encoding.UTF8.GetString(enumerator.Current.Span));
        
        Assert.False(enumerator.MoveNext());
        Assert.Equal(ReadOnlyMemory<byte>.Empty, enumerator.Current);
    }

    [Fact]
    public void MoveNext_MultipleParameters_ReturnsAllValues()
    {
        byte[] data = "param1 param2 param3"u8.ToArray();
        var enumerator = new SeparatedByEnumerator(data, ' ');
        var results = new List<string>();

        foreach (var item in enumerator)
        {
            results.Add(Encoding.UTF8.GetString(item.Span));
        }

        Assert.Equal(new[] { "param1", "param2", "param3" }, results);
    }

    [Theory]
    [InlineData("   param1 param2", new[] { "param1", "param2" })] // leading
    [InlineData("param1 param2   ", new[] { "param1", "param2" })]  // trailing
    [InlineData("param1    param2     param3", new[] { "param1", "param2", "param3" })] // between
    [InlineData("  param1  param2 param3   param4   ", new [] { "param1", "param2", "param3", "param4" })]
    public void MoveNext_VariousSeparatorScenarios_HandlesCorrectly(string input, string[] expected)
    {
        byte[] data = Encoding.UTF8.GetBytes(input);
        var enumerator = new SeparatedByEnumerator(data, ' ');
        var results = new List<string>();

        foreach (var item in enumerator)
        {
            results.Add(Encoding.UTF8.GetString(item.Span));
        }

        Assert.Equal(expected, results);
    }
    
    [Fact]
    public void MoveNext_OnlySpaces_ReturnsNoElements()
    {
        byte[] data = "     "u8.ToArray();
        var enumerator = new SeparatedByEnumerator(data, ' ');
        
        bool result = enumerator.MoveNext();

        Assert.False(result);
        Assert.Equal(ReadOnlyMemory<byte>.Empty, enumerator.Current);
    }

    [Theory]
    [InlineData(',', "param1,param2,param3", new[] { "param1", "param2", "param3" })]
    [InlineData('|', "param1|param2|param3", new[] { "param1", "param2", "param3" })]
    [InlineData('\t', "param1\tparam2\tparam3", new[] { "param1", "param2", "param3" })]
    public void MoveNext_DifferentSeparators_ParsesCorrectly(char separator, string input, string[] expected)
    {
        byte[] data = Encoding.UTF8.GetBytes(input);
        var enumerator = new SeparatedByEnumerator(data, separator);
        var results = new List<string>();

        while (enumerator.MoveNext())
        {
            results.Add(Encoding.UTF8.GetString(enumerator.Current.Span));
        }

        Assert.Equal(expected, results);
    }
}
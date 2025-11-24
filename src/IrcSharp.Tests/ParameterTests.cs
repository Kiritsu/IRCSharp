using System.Text;
using IrcSharp.Internal.Extensions;

namespace IrcSharp.Tests;

public class ParameterTests
{
    [Fact]
    public void ParseParameter_AllKnownCases()
    {
        var testCases = new[]
        {
            ("param1=x", "param1", "x"),
            ("param2", "param2", null),
            ("-param", "param", null),
            ("param4=3,5,1,5", "param4", "3,5,1,5")
        };

        foreach (var (input, expectedKey, expectedValue) in testCases)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var memory = new ReadOnlyMemory<byte>(bytes);

            var result = memory.ParseParameter();

            Assert.Equal(expectedKey, result.param1);
            Assert.Equal(expectedValue, result.param2);
        }
    }
}
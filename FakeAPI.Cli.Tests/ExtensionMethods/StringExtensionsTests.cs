using FluentAssertions;
using Raccoon.Ninja.FakeAPI.Cli.ExtensionMethods;

namespace FakeAPI.Cli.Tests.ExtensionMethods;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    public void IsJson_NullOrEmptyText_ReturnsFalse(string text)
    {
        text.IsJson().Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetValidTestJson))]
    public void IsJson_JsonAsText_ReturnsTrue(string jsonAsText)
    {
        jsonAsText.IsJson().Should().BeTrue();
    }
    public static IEnumerable<object[]> GetValidTestJson()
    {
        yield return new object[] { "{\"prop\": 42}" };
        yield return new object[] { "\r\n\r\n{\"prop\": 42}" };
        yield return new object[] { "\r{\"prop\": 42}" };
        yield return new object[] { "\r\n{\"prop\": 42}" };
        yield return new object[] { "[{\"prop\": 42}, {\"prop\": 42}]" };
        yield return new object[] { "\r\n\r\n[{\"prop\": 42}, {\"prop\": 42}]" };
        yield return new object[] { "\r[{\"prop\": 42}, {\"prop\": 42}]" };
        yield return new object[] { "\r\n[{\"prop\": 42}, {\"prop\": 42}]" };
        yield return new object[] { "{\"prop\": 42}" };
        yield return new object[]
        {
            @"
{
   ""prop"": 42
}
"
        };
        yield return new object[]
        {
            @"{
   ""prop"": 42
}
"
        };
        yield return new object[]
        {
            @"
[{
   ""prop"": 42
},
{
   ""prop"": 42
},
{
   ""prop"": 42
}]
"
        };
        yield return new object[]
        {
            @"[{
   ""prop"": 42
},
{
   ""prop"": 42
}]"
        };
    }
}
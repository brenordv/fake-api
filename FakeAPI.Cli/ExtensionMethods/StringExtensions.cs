namespace Raccoon.Ninja.FakeAPI.Cli.ExtensionMethods;

public static class StringExtensions
{
    public static bool IsJson(this string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        var textLine = (text.Length > 100 ? text[..100] : text)
            .Trim()
            .Replace("\n", "")
            .Replace("\t", "")
            .Replace("\r", "")
            .Replace(" ", "");


        return textLine.StartsWith("{") || textLine.StartsWith("[");
    }
}
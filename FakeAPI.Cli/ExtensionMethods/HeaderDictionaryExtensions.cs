namespace Raccoon.Ninja.FakeAPI.Cli.ExtensionMethods;

public static class HeaderDictionaryExtensions
{
    public static string Stringify(this IHeaderDictionary headers)
    {
        var result = new List<string>();
        foreach (var (header, value) in headers)
        {
            result.Add($"{header}: {value}");
        }

        return string.Join("\n", result);
    }
}
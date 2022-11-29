using System.Diagnostics.CodeAnalysis;
using Raccoon.Ninja.FakeAPI.Cli.Enums;

namespace Raccoon.Ninja.FakeAPI.Cli.ValueObjects;

[ExcludeFromCodeCoverage]
public record CliOptions(int StatusCode, IList<string> Files, FileReturnOption FileOption, int Port, string Host)
{
    public bool HasReturnContent => Files != null && Files.Any();
    public string Url => $"{Host}:{Port}";
}
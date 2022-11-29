using System.Diagnostics.CodeAnalysis;
using Raccoon.Ninja.FakeAPI.Cli.Interfaces;

namespace Raccoon.Ninja.FakeAPI.Cli.Handlers;

[ExcludeFromCodeCoverage]
public class FileHandler: IFileHandler
{
    public bool Exists(string filename)
    {
        return File.Exists(filename);
    }
}
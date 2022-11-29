using System.Text;
using Raccoon.Ninja.FakeAPI.Cli.Enums;

namespace Raccoon.Ninja.FakeAPI.Cli.Handlers;

public static class ResponseFromFileHandler
{
    private static int _filesIndex;
    public static (string filename, string content) GetNextFile(IList<string> files, FileReturnOption option)
    {
        var file = option == FileReturnOption.Fixed ? files[0] : files[_filesIndex++ % files.Count];
        var content = File.ReadAllText(file, Encoding.UTF8);
        return (file, content);
    }
}
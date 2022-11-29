using Raccoon.Ninja.FakeAPI.Cli.Enums;
using Raccoon.Ninja.FakeAPI.Cli.Interfaces;
using Raccoon.Ninja.FakeAPI.Cli.ValueObjects;

namespace Raccoon.Ninja.FakeAPI.Cli.Handlers;

public class CliArgsHandler
{
    private int _statusCode = 200;
    private const int Port = 5000;
    private const string HostUrl = "https://localhost";
    private readonly IFileHandler _fileHandler;
    
    public CliArgsHandler(IFileHandler fileHandler)
    {
        _fileHandler = fileHandler;
    }
    
    public CliArgsHandler SetDefaultStatusCodeReturn(int statusCode)
    {
        _statusCode = statusCode;
        return this;
    }

    public CliOptions Parse(string[] args)
    {
        if (args == null || !args.Any())
            return GetDefaultOptions();
        
        var files = GetFilesOption(args);
        var fileReturnOption = GetFileReturnOption(args);
        var hostUrl = GetHostUrl(args);
        var port = GetPortNumber(args);
        var statusCode = GetStatusCode(args);
        
        return new CliOptions(
            statusCode,
            files,
            fileReturnOption,
            port,
            hostUrl
        );
    }

    private int GetStatusCode(string[] args)
    {
        if (args == null)
            return _statusCode;

        var statusCodeText = args
            .FirstOrDefault(arg => arg.Contains("-statusCode=", StringComparison.InvariantCultureIgnoreCase));

        var statusCode = ExtractValue(statusCodeText, _statusCode);
        return statusCode;
    }

    private int GetPortNumber(string[] args)
    {
        if (args == null)
            return Port;

        var portText = args
            .FirstOrDefault(arg => arg.Contains("-port=", StringComparison.InvariantCultureIgnoreCase));

        var port = ExtractValue(portText, Port);
        return port;
    }

    private string GetHostUrl(string[] args)
    {
        if (args == null)
            return HostUrl;

        var hostText = args
            .FirstOrDefault(arg => arg.Contains("-host=", StringComparison.InvariantCultureIgnoreCase));

        var host = ExtractValue(hostText, HostUrl);
        return !host.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) 
            ? HostUrl 
            : host;
    }

    private CliOptions GetDefaultOptions()
    {
        return new CliOptions(_statusCode, new List<string>(), FileReturnOption.None, Port, HostUrl);
    }

    private static FileReturnOption GetFileReturnOption(IEnumerable<string> args)
    {
        if (args == null)
            return FileReturnOption.None;

        var fileOptionText = args
            .FirstOrDefault(arg => arg.Contains("-fileOption=", StringComparison.InvariantCultureIgnoreCase));

        var fileReturnOption = ExtractValue(fileOptionText, FileReturnOption.None);
        return fileReturnOption;
    }

    private IList<string> GetFilesOption(IEnumerable<string> args)
    {
        var files = new List<string>();

        if (args == null)
            return files;

        foreach (var fileOption in args
                     .Where(arg => arg.Contains("-file=", StringComparison.InvariantCultureIgnoreCase)))
        {
            var file = ExtractValue<string>(fileOption, null);

            if (string.IsNullOrWhiteSpace(file))
                continue;

            if (!_fileHandler.Exists(file))
            {
                Console.WriteLine($"File '{file}' does not exist or cannot be accessed. Skipping this...");
                continue;
            }

            files.Add(file);
        }

        return files; 
    }

    private static T ExtractValue<T>(string text, T defaultValue, string divider = "=")
    {
        if (string.IsNullOrWhiteSpace(text))
            return defaultValue;

        var parts = text.Split(divider);
        if (parts.Length != 2)
            return defaultValue;

        var value = parts[1];
        try
        {
            if (typeof(T).BaseType == typeof(Enum))
                return (T)Enum.Parse(typeof(T), value, true);
                
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (FormatException)
        {
            return defaultValue;
        }
        catch (InvalidCastException)
        {
            return defaultValue;
        }
    }
}
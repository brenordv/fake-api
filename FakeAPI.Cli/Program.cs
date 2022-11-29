using System.Text;
using Raccoon.Ninja.FakeAPI.Cli.ExtensionMethods;
using Raccoon.Ninja.FakeAPI.Cli.Handlers;

var builder = WebApplication.CreateBuilder(Array.Empty<string>());
var app = builder.Build();

var options = new CliArgsHandler(new FileHandler())
    .Parse(args);

app.Use(async (context, next) =>
{
    //Kind of a hack, just to get by.
    if (string.IsNullOrWhiteSpace(context.Request.Path))
        await next(context);

    Console.WriteLine("---------------------------------------------------------------------------------------------");
    Console.WriteLine($"[{context.Request.Method}] Received At: {DateTime.Now}");
    Console.WriteLine("---------------------------------------------");
    Console.WriteLine($"[{context.Request.Method}] Endpoint: {context.Request.Path}");
    Console.WriteLine($"[{context.Request.Method}] Headers:");
    Console.WriteLine(context.Request.Headers.Stringify());
    Console.WriteLine($"[{context.Request.Method}] QueryString: {string.Join(", ", context.Request.QueryString)}");
    Console.WriteLine($"[{context.Request.Method}] Body: {await context.Request.GetBody()}");

    var isOption = context.Request.Method == "OPTIONS";
    
    context.Response.StatusCode = options.StatusCode;
    
    if (options.HasReturnContent && !isOption)
    {
        var (filename, content) = ResponseFromFileHandler.GetNextFile(options.Files, options.FileOption);
        Console.WriteLine(
            $"Returning file '{filename}' with content of size '{content?.Length}' chars for a '{context.Request.Method}' request");
        context.Response.ContentType = content.IsJson()
            ? "application/json;charset=UTF-8"
            : "text/plain;charset=UTF-8";
        await context.Response.WriteAsync(content, Encoding.UTF8);
        return;
    }
    
    Console.WriteLine(
            $"Returning an empty response with Status Code '{context.Response.StatusCode}' for a '{context.Request.Method}' request");

    await context.Response.CompleteAsync();
});

app.UseRouting();
app.MapGet("/", () =>
{
    Console.WriteLine("=======This will never be reached=======");
    return "Hello World";
});

app.Run(options.Url);
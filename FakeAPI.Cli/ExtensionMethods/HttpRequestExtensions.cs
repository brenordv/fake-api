namespace Raccoon.Ninja.FakeAPI.Cli.ExtensionMethods;

public static class HttpRequestExtensions
{
    public static async Task<object> GetBody(this HttpRequest request)
    {
        try
        {
            var result = await request.ReadFromJsonAsync<object>();
            return result;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
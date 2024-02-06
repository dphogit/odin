namespace Odin.Api.Config;

public static class WebHostEnvironmentConstants
{
    public const string Testing = "Testing";
}

public static class WebHostEnvironmentExtensions
{
    public static bool IsTesting(this IWebHostEnvironment env)
    {
        return env.IsEnvironment(WebHostEnvironmentConstants.Testing);
    }
}

public static class WebHostBuilderExtensions
{
    public static IWebHostBuilder UseTestingEnvironment(this IWebHostBuilder builder)
    {
        builder.UseEnvironment(WebHostEnvironmentConstants.Testing);
        return builder;
    }
}

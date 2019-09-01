using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Serializing
{
    public static class SetupSerializers
    {
        public static IWebHostBuilder ConfigureSerializer(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<JsonSerializer>();
            });

            return webHostBuilder;
        }
    }
}
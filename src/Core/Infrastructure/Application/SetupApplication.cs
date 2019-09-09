namespace Core.Infrastructure.Application
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class SetupApplication
    {
        public static IWebHostBuilder ConfigureApplication(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<IHostedService, ApplicationService>();
            });
            return webHostBuilder;
        }
    }
}
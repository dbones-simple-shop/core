namespace Core.Infrastructure.HealthChecks
{
    using MassTransit.HealthCheck;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    public static class SetupHealthChecks
    {
        public static IWebHostBuilder ConfigureHeathChecks(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureServices(services =>
            {
                //we are assuming that all services will have web api and rabbit for exposed points.
                services.AddHealthChecks()
                    .AddCheck("WebApi", () => HealthCheckResult.Healthy("WebApi"))
                    .AddCheck<MassTransitHealthCheck>("Bus");
                
            });
            return webHostBuilder;
        }

        public static IApplicationBuilder ConfigureApplicationHealthCheck(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/hc");

            return app;
        }
    }


   
}
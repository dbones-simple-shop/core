using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Swagger
{
    public static class SetupSwagger
    {
        public static IWebHostBuilder ConfigureSwagger(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureServices(services =>
            {
                services.AddSwaggerGen(options =>
                {
                    options.DescribeAllEnumsAsStrings();
                    options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                    {
                        Title = "HTTP API",
                        Version = "v1",
                        Description = "",
                        TermsOfService = ""
                    });

                });
            });
            
            return webHostBuilder;
        }

        public static IApplicationBuilder ConfigureApplicationSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                });
            return app;
        }
    }
}
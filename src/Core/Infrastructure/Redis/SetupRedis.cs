using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.Infrastructure.Redis
{
    public static class SetupRedis
    {
        public static IWebHostBuilder ConfigureRedis(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureServices(services =>
            {

                services.AddSingleton<ConnectionMultiplexer>(provider =>
                {
                    var dbConfig = new RedisConfiguration();
                    var conf = provider.GetService<IConfiguration>();
                    conf.GetSection("Redis").Bind(dbConfig);
                    
                    return ConnectionMultiplexer.Connect(dbConfig.ConnectionString);
                });

                services.AddScoped<IDatabase>(provider =>
                {
                    var redis = provider.GetService<ConnectionMultiplexer>();
                    return redis.GetDatabase();
                });
            });

            return webHostBuilder;
        }
       
    }
}
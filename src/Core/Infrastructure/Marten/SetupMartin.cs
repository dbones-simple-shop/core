using Marten;
using Marten.Schema;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Marten
{
    public static class SetupMartin 
    {
        public static IWebHostBuilder ConfigureMartin(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<IDocumentStore>(s =>
                {
                    var dbConfig = new DatabaseConfiguration();
                    var conf = s.GetService<IConfiguration>();
                    conf.GetSection("Database").Bind(dbConfig);

                    var logger = s.GetService<ILogger<Core.Infrastructure.Marten.DatabaseConfiguration>>();

                    return DocumentStore.For(_ =>
                    {
                        _.Connection(dbConfig.ConnectionString);
                        //_.DatabaseSchemaName = dbConfig.Name;
                        
                        _.CreateDatabasesForTenants(c =>
                        {
                            // Specify a db to which to connect in case database needs to be created.
                            // If not specified, defaults to 'postgres' on the connection for a tenant.
                            //c.MaintenanceDatabase(cstring);
                            c.ForTenant()
                                .CheckAgainstPgDatabase()
                                .WithOwner("application")
                                .WithEncoding("UTF-8")
                                .ConnectionLimit(-1)
                                .OnDatabaseCreated(__ =>
                                {
                                    logger.LogInformation($"created {__.Database}");
                                    //dbCreated = true;
                                });
                        });

                        _.DefaultIdStrategy = (mapping, storeOptions) => new StringIdGeneration();
                        _.AutoCreateSchemaObjects = AutoCreate.All;
                    });
                });

                services.AddScoped(s => s.GetService<IDocumentStore>().DirtyTrackedSession());
            });

            return webHostBuilder;
        }
    }
}
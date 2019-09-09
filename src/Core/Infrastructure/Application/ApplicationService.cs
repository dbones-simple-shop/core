namespace Core.Infrastructure.Application
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class ApplicationService : IHostedService
    {
        private readonly ILogger<ApplicationService> _logger;

        public ApplicationService(ILogger<ApplicationService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var assembly = Assembly.GetEntryAssembly();
                var version = assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
                var name = assembly?.FullName;
                _logger.LogInformation($"starting {name} - {version}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "issue getting version");
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
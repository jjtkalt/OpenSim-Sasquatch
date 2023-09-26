using log4net.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace OpenSim.Server.MoneyServer
{
    public sealed class MoneyService : IHostedService
    {
        private readonly Task _completedTask = Task.CompletedTask;
        private readonly ILogger<MoneyService> _logger;

        public MoneyService(ILogger<MoneyService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{Service} is running.", nameof(MoneyServer));


            MoneyServerBase app = new MoneyServerBase();
            app.Startup();
            app.Work();
            
            return _completedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{Service} is stopping.", nameof(MoneyServer));

            return _completedTask;
        }
    }
}

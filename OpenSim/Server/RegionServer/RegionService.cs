using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenSim.Server.RegionServer
{
    public sealed class RegionService : IHostedService
    {
        private readonly Task _completedTask = Task.CompletedTask;
        private readonly ILogger<RegionService> _logger;

        private int m_res;

        public RegionService(ILogger<RegionService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{Service} is running.", nameof(RegionServer));

            string[] args = Environment.GetCommandLineArgs();

            // Not the way we ultimately want to do this...  
            // Reach into the OpenSim Namespace and run the main there passing in args.
            Application.Main(args);

            return _completedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{Service} is stopping.", nameof(RegionServer));

            // Nothing to do here
            // OpenSimServer.Shutdown(m_res);

            return _completedTask;
        }
    }
}

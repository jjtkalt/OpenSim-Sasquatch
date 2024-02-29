using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OpenSim.Server.GridServer
{
    public sealed class GridService : IHostedService
    {
        private readonly Task _completedTask = Task.CompletedTask;

        private readonly ILogger<GridService> _logger;
        private readonly IConfiguration _configuration;
        private readonly GridServer _openSimServer;

        public GridService(
            IConfiguration configuration, 
            ILogger<GridService> logger,
            GridServer openSimServer
            )
        {
            _configuration = configuration;
            _logger = logger;
            _openSimServer = openSimServer;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{Service} is running.", nameof(Server.GridServer));

            _openSimServer.Startup();
            _openSimServer.Work();
            
            return _completedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            int m_res = 0;

            _logger.LogInformation("{Service} is stopping.", nameof(Server.GridServer));

            _openSimServer.Shutdown(m_res);

            return _completedTask;
        }
    }
}

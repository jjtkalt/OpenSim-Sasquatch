using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OpenSim.Server.RobustServer
{
    public sealed class RobustService : IHostedService
    {
        private readonly Task _completedTask = Task.CompletedTask;

        private readonly ILogger<RobustService> _logger;
        private readonly IConfiguration _configuration;
        private readonly RobustServer _openSimServer;

        private int m_res;

        public RobustService(
            IConfiguration configuration, 
            ILogger<RobustService> logger,
            RobustServer openSimServer
            )
        {
            _configuration = configuration;
            _logger = logger;
            _openSimServer = openSimServer;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{Service} is running.", nameof(Server.RobustServer));

            _openSimServer.Startup();
            _openSimServer.Work();
            
            return _completedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{Service} is stopping.", nameof(Server.RobustServer));

            _openSimServer.Shutdown(m_res);

            return _completedTask;
        }
    }
}

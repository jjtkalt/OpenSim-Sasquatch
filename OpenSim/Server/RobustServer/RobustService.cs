using log4net.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace OpenSim.Server.RobustServer
{
    public sealed class RobustService : IHostedService
    {
        private readonly Task _completedTask = Task.CompletedTask;
        private readonly ILogger<RobustService> _logger;

        private int m_res;

        public RobustService(ILogger<RobustService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{Service} is running.", nameof(RobustServer));

            m_res = OpenSimServer.Startup();

            return _completedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{Service} is stopping.", nameof(RobustServer));

            OpenSimServer.Shutdown(m_res);

            return _completedTask;
        }
    }
}

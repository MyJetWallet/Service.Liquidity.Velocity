using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.Liquidity.Velocity.Services;

namespace Service.Liquidity.Velocity
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly VelocityCalcBackgroundService _velocityCalcBackgroundService;

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime, 
            ILogger<ApplicationLifetimeManager> logger, 
            VelocityCalcBackgroundService velocityCalcBackgroundService)
            : base(appLifetime)
        {
            _logger = logger;
            _velocityCalcBackgroundService = velocityCalcBackgroundService;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _velocityCalcBackgroundService.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _velocityCalcBackgroundService.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}

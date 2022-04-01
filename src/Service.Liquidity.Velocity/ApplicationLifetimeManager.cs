using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.Liquidity.Velocity.Jobs;

namespace Service.Liquidity.Velocity
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly LiquidityVelocityCalcBackgroundService _liquidityVelocityCalcBackgroundService;
        private readonly MarkupVelocityCalcBackgroundService _markupVelocityCalcBackgroundService;

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime, 
            ILogger<ApplicationLifetimeManager> logger, 
            LiquidityVelocityCalcBackgroundService liquidityVelocityCalcBackgroundService, 
            MarkupVelocityCalcBackgroundService markupVelocityCalcBackgroundService)
            : base(appLifetime)
        {
            _logger = logger;
            _liquidityVelocityCalcBackgroundService = liquidityVelocityCalcBackgroundService;
            _markupVelocityCalcBackgroundService = markupVelocityCalcBackgroundService;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _liquidityVelocityCalcBackgroundService.Start();
            _markupVelocityCalcBackgroundService.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _liquidityVelocityCalcBackgroundService.Stop();
            _markupVelocityCalcBackgroundService.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}

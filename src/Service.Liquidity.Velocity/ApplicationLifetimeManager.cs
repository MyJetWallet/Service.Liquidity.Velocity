using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyNoSqlServer.Abstractions;
using Service.Liquidity.Velocity.Jobs;

namespace Service.Liquidity.Velocity
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly LiquidityVelocityCalcBackgroundService _liquidityVelocityCalcBackgroundService;
        private readonly MarkupVelocityCalcBackgroundService _markupVelocityCalcBackgroundService;
        private readonly MyNoSqlClientLifeTime _myNoSqlCientLifeTime;

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime, 
            ILogger<ApplicationLifetimeManager> logger, 
            LiquidityVelocityCalcBackgroundService liquidityVelocityCalcBackgroundService, 
            MarkupVelocityCalcBackgroundService markupVelocityCalcBackgroundService, 
            MyNoSqlClientLifeTime myNoSqlCientLifeTime)
            : base(appLifetime)
        {
            _logger = logger;
            _liquidityVelocityCalcBackgroundService = liquidityVelocityCalcBackgroundService;
            _markupVelocityCalcBackgroundService = markupVelocityCalcBackgroundService;
            _myNoSqlCientLifeTime = myNoSqlCientLifeTime;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _myNoSqlCientLifeTime.Start();
            _liquidityVelocityCalcBackgroundService.Start();
            _markupVelocityCalcBackgroundService.Start();
            
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _liquidityVelocityCalcBackgroundService.Stop();
            _markupVelocityCalcBackgroundService.Stop();
            _myNoSqlCientLifeTime.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}

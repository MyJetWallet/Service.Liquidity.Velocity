using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain;
using MyJetWallet.Sdk.Service.Tools;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Grpc;
using Service.Liquidity.TradingPortfolio.Grpc;
using Service.Liquidity.TradingPortfolio.Grpc.Models;
using Service.Liquidity.Velocity.Domain.Models.NoSql;
using Service.Liquidity.Velocity.Domain.Utils;
using SimpleTrading.Abstraction.Candles;
using SimpleTrading.CandlesHistory.Grpc;
using SimpleTrading.CandlesHistory.Grpc.Contracts;

namespace Service.Liquidity.Velocity.Jobs
{
    public class LiquidityVelocityCalcBackgroundService
    {
        private readonly ILogger<LiquidityVelocityCalcBackgroundService> _logger;
        private readonly ISimpleTradingCandlesHistoryGrpc _candlesHistory;
        private readonly ISpotInstrumentsDictionaryService _instrumentService;
        private readonly MyTaskTimer _operationsTimer;
        private readonly IMyNoSqlServerDataWriter<VelocityNoSql> _myNoSqlVelocityWriter;
        private readonly IManualInputService _manualInputService;
#if DEBUG
        private const int TimerSpanSec = 30;
#else
        private const int TimerSpanSec = 3600;
#endif
        public LiquidityVelocityCalcBackgroundService(
            ILogger<LiquidityVelocityCalcBackgroundService> logger,
            ISimpleTradingCandlesHistoryGrpc candlesHistory,
            ISpotInstrumentsDictionaryService instrumentService,
            IMyNoSqlServerDataWriter<VelocityNoSql> myNoSqlVelocityWriter,
            IManualInputService manualInputService)
        {
            _logger = logger;
            _candlesHistory = candlesHistory;
            _instrumentService = instrumentService;
            _myNoSqlVelocityWriter = myNoSqlVelocityWriter;
            _manualInputService = manualInputService;
            _operationsTimer = new MyTaskTimer(nameof(LiquidityVelocityCalcBackgroundService),
                TimeSpan.FromSeconds(TimerSpanSec), logger, Process);
        }

        public void Start()
        {
            _operationsTimer.Start();
        }

        public void Stop()
        {
            _operationsTimer.Stop();
        }

        private async Task Process()
        {
            var instrumentResponse = await _instrumentService.GetAllSpotInstrumentsAsync();
            var spotInstruments = instrumentResponse.SpotInstruments
                .Where(e => e.BaseAsset == "USD" || e.QuoteAsset == "USD")
                .ToList();

            foreach (var item in spotInstruments)
            {
                var symbol = item.Symbol;
                var candleType = CandleType.Day;
                var current = DateTime.UtcNow;
                var from = CalendarUtils.TwoWeeksBefore(current);
                var to = CalendarUtils.OneDayBefore(current);
                //var coef = item.QuoteAsset == "USD" ? 1 : -1;  
                var asset = item.QuoteAsset == "USD" ? item.BaseAsset : item.QuoteAsset;

                var candles = (await _candlesHistory.GetCandlesHistoryAsync(new GetCandlesHistoryGrpcRequestContract
                    {
                        Bid = true,
                        Instrument = symbol,
                        CandleType = candleType,
                        From = from,
                        To = to
                    }))
                    .OrderByDescending(e => e.DateTime)
                    .ToList();

                var lowOpenSum = 0.0;
                var highOpenSum = 0.0;

                var velocity = VelocityNoSql.Create(item.BrokerId, new Domain.Models.Velocity
                {
                    Asset = asset,
                    LowOpenAverage = 0.0m,
                    HighOpenAverage = 0.0m,
                    CalcDate = DateTime.UtcNow
                });

                if (candles.Count == 0)
                {
                    await _myNoSqlVelocityWriter.InsertOrReplaceAsync(velocity);
                    continue;
                }

                // Calc Mid and Usd
                foreach (var candle in candles)
                {
                    if (candle.Open == 0.0)
                        continue;

                    lowOpenSum += candle.Low / candle.Open;
                    highOpenSum += candle.High / candle.Open;
                }

                var lowOpenAverage = Convert.ToDecimal(lowOpenSum / candles.Count);
                var highOpenAverage = Convert.ToDecimal(highOpenSum / candles.Count);

                velocity.Velocity.LowOpenAverage = (lowOpenAverage - 1m) * 100m;
                velocity.Velocity.HighOpenAverage = (highOpenAverage - 1m) * 100;
                await _myNoSqlVelocityWriter.InsertOrReplaceAsync(velocity);

                var response = await _manualInputService.SetVelocityAsync(new SetVelocityRequest
                {
                    Broker = DomainConstants.DefaultBroker,
                    Asset = asset,
                    User = "liquidity.velocity.service",
                    VelocityLowOpen = velocity.Velocity.LowOpenAverage,
                    VelocityHighOpen = velocity.Velocity.HighOpenAverage
                });
            }
        }
    }
}
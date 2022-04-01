using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service.Tools;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Grpc;
using Service.Liquidity.TradingPortfolio.Grpc;
using Service.Liquidity.Velocity.Domain.Models;
using Service.Liquidity.Velocity.Domain.Models.NoSql;
using Service.Liquidity.Velocity.Domain.Utils;
using SimpleTrading.Abstraction.Candles;
using SimpleTrading.CandlesHistory.Grpc;
using SimpleTrading.CandlesHistory.Grpc.Contracts;

namespace Service.Liquidity.Velocity.Jobs
{
    public class MarkupVelocityCalcBackgroundService
    {
        private readonly ILogger<MarkupVelocityCalcBackgroundService> _logger;
        private readonly ISimpleTradingCandlesHistoryGrpc _candlesHistory;
        private readonly ISpotInstrumentsDictionaryService _instrumentService;
        private readonly MyTaskTimer _operationsTimer;
        private readonly IMyNoSqlServerDataWriter<MarkupVelocityNoSql> _myNoSqlVelocityWriter;
#if DEBUG
        private const int TimerSpanSec = 30;
        private const uint PeriodMin = 200;
#else
        private const int TimerSpanSec = 60;
        private const uint PeriodMin = 200;
#endif
        public MarkupVelocityCalcBackgroundService(
            ILogger<MarkupVelocityCalcBackgroundService> logger,
            ISimpleTradingCandlesHistoryGrpc candlesHistory,
            ISpotInstrumentsDictionaryService instrumentService,
            IMyNoSqlServerDataWriter<MarkupVelocityNoSql> myNoSqlVelocityWriter)
        {
            _logger = logger;
            _candlesHistory = candlesHistory;
            _instrumentService = instrumentService;
            _myNoSqlVelocityWriter = myNoSqlVelocityWriter;
            _operationsTimer = new MyTaskTimer(nameof(MarkupVelocityCalcBackgroundService),
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
                var period = PeriodMin;
                var candleType = CandleType.Minute;
                var current = DateTime.UtcNow;
                var from = CalendarUtils.CountOfMinutesBefore(current, period);
                var to = CalendarUtils.OneMinuteBefore(current);
                var symbol = item.Symbol;
                var asset = item.QuoteAsset == "USD" ? item.BaseAsset : item.QuoteAsset;

                var candles = (await _candlesHistory.GetCandlesHistoryAsync(
                        new GetCandlesHistoryGrpcRequestContract
                    {
                        Bid = true,
                        Instrument = symbol,
                        CandleType = candleType,
                        From = from,
                        To = to
                    }))
                    .OrderBy(e => e.DateTime)
                    .ToList();

                var calculatedItem = MarkupVelocityNoSql.Create(item.BrokerId, CreateDefaultItem(asset));
                var velocityItem = calculatedItem.Velocity;
                var first = candles.FirstOrDefault();
                var last = candles.LastOrDefault();
                velocityItem.CanTrust = (candles.Count != 0 && candles.Count == period);
                velocityItem.Velocity = (Convert.ToDecimal(last?.Close) - Convert.ToDecimal(first?.Open)) / 100;
                await _myNoSqlVelocityWriter.InsertOrReplaceAsync(calculatedItem);
            }
        }

        private MarkupVelocity CreateDefaultItem(string asset)
        {
            return new Domain.Models.MarkupVelocity
            {
                Asset = asset,
                Velocity = 0,
                Period = 0,
                CalcDateTime = DateTime.UtcNow,
                CanTrust = false
            };
        }

    }
}
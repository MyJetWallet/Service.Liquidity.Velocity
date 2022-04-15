using System;
using System.Collections.Generic;
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
        private readonly IMyNoSqlServerDataReader<MarkupVelocitySettingsNoSql> _myNoSqlVelocitySettingsReader;

#if DEBUG
        private const int TimerSpanSec = 30;
        private const uint DefaultPeriodMin = 200;
#else
        private const int TimerSpanSec = 60;
        private const uint DefaultPeriodMin = 200;
#endif
        public MarkupVelocityCalcBackgroundService(
            ILogger<MarkupVelocityCalcBackgroundService> logger,
            ISimpleTradingCandlesHistoryGrpc candlesHistory,
            ISpotInstrumentsDictionaryService instrumentService,
            IMyNoSqlServerDataWriter<MarkupVelocityNoSql> myNoSqlVelocityWriter,
            IMyNoSqlServerDataReader<MarkupVelocitySettingsNoSql> myNoSqlVelocitySettingsReader)
        {
            _logger = logger;
            _candlesHistory = candlesHistory;
            _instrumentService = instrumentService;
            _myNoSqlVelocityWriter = myNoSqlVelocityWriter;
            _myNoSqlVelocitySettingsReader = myNoSqlVelocitySettingsReader;
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
            try
            {
                var instrumentResponse = await _instrumentService.GetAllSpotInstrumentsAsync();
                var spotInstruments = instrumentResponse.SpotInstruments
                    .Where(e => e.BaseAsset == "USD" || e.QuoteAsset == "USD")
                    .ToList();

                foreach (var item in spotInstruments)
                {
                    var asset = item.QuoteAsset == "USD" ? item.BaseAsset : item.QuoteAsset;
                    var candleType = CandleType.Minute;
                    var current = DateTime.UtcNow;
                    var symbol = item.Symbol;

                    var assetSettings = _myNoSqlVelocitySettingsReader.Get(item.BrokerId, asset);
                    var period = assetSettings == null ? DefaultPeriodMin : assetSettings.Settings.Period;
                    var from = CalendarUtils.CountOfMinutesBefore(current, period);
                    var to = CalendarUtils.OneMinuteBefore(current);

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

                    var first = candles.FirstOrDefault();
                    var last = candles.LastOrDefault();
                    var velocity = (Convert.ToDecimal(last?.Close) - Convert.ToDecimal(first?.Open)) / 100;
                    var canTrust = (candles.Count != 0 && candles.Count == period);
                    var velocityItem = CreateMyNoSqlItem(asset, velocity, period, DateTime.UtcNow, canTrust);

                    await _myNoSqlVelocityWriter.InsertOrReplaceAsync(MarkupVelocityNoSql.Create(item.BrokerId, velocityItem));
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message);
            }
        }

        private MarkupVelocity CreateMyNoSqlItem(string asset, decimal velocity, uint period,
            DateTime date, bool canTrust)
        {
            return new Domain.Models.MarkupVelocity
            {
                Asset = asset,
                Velocity = velocity,
                Period = period,
                CalcDateTime = date,
                CanTrust = canTrust
            };
        }

    }
}
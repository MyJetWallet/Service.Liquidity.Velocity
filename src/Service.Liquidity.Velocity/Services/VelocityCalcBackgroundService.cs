using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service.Tools;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Grpc;
using Service.Liquidity.Velocity.Domain.Models;
using Service.Liquidity.Velocity.Domain.Models.NoSql;
using Service.Liquidity.Velocity.Domain.Utils;
using SimpleTrading.Abstraction.Candles;
using SimpleTrading.CandlesHistory.Grpc;
using SimpleTrading.CandlesHistory.Grpc.Contracts;

namespace Service.Liquidity.Velocity.Services
{
    public class VelocityCalcBackgroundService
    {
        private readonly ILogger<VelocityCalcBackgroundService> _logger;
        private readonly ISimpleTradingCandlesHistoryGrpc _candlesHistory;
        private readonly ISpotInstrumentsDictionaryService _instrumentService;
        private readonly MyTaskTimer _operationsTimer;
        private readonly IMyNoSqlServerDataWriter<VelocityNoSql> _myNoSqlVelocityWriter;
        private const int TimerSpan60Sec = 60;
        
        
        public VelocityCalcBackgroundService(
            ILogger<VelocityCalcBackgroundService> logger, 
            ISimpleTradingCandlesHistoryGrpc candlesHistory, 
            ISpotInstrumentsDictionaryService instrumentService, 
            IMyNoSqlServerDataWriter<VelocityNoSql> myNoSqlVelocityWriter)
        {
            _logger = logger;
            _candlesHistory = candlesHistory;
            _instrumentService = instrumentService;
            _myNoSqlVelocityWriter = myNoSqlVelocityWriter;
            _operationsTimer = new MyTaskTimer(nameof(VelocityCalcBackgroundService), 
                TimeSpan.FromSeconds(TimerSpan60Sec), logger, Process);

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
                var coef = item.QuoteAsset == "USD" ? 1 : -1;  
                var asset = item.QuoteAsset == "USD" ? item.BaseAsset : item.QuoteAsset;  
                
                // Get history
                var data = await _candlesHistory.GetCandlesHistoryAsync(new GetCandlesHistoryGrpcRequestContract()
                {
                    Bid = true,
                    Instrument = symbol,
                    CandleType = candleType,
                    From = from,
                    To = to
                });

                var candels = data.OrderByDescending(e => e.DateTime).ToList();
                var lowOpenSum = 0.0;
                var highOpenSum = 0.0;

                var velocity = VelocityNoSql.Create(new Domain.Models.Velocity
                {
                    Asset = asset,
                    LowOpenAverage = 0.0m,
                    HighOpenAverage = 0.0m,
                    CalcDate = DateTime.UtcNow
                });

                if (candels.Count == 0)
                {
                    await _myNoSqlVelocityWriter.InsertOrReplaceAsync(velocity);
                    continue;
                }
            
                // Calc Mid and Usd
                foreach (var candle in candels)
                {
                    if(candle.Open == 0.0)
                        continue;
                    
                    lowOpenSum += candle.Low / candle.Open;
                    highOpenSum += candle.High / candle.Open;
                }
                
                var lowOpenAverage = Convert.ToDecimal(Math.Pow(lowOpenSum / candels.Count, coef));
                var highOpenAverage = Convert.ToDecimal(Math.Pow(highOpenSum / candels.Count, coef));

                velocity.Velocity.LowOpenAverage = lowOpenAverage;
                velocity.Velocity.HighOpenAverage = highOpenAverage;
                await _myNoSqlVelocityWriter.InsertOrReplaceAsync(velocity);
            }
        }
    }
}

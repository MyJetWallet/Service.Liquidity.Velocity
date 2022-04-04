using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core.Logging;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.Liquidity.Velocity.Domain.Models;
using Service.Liquidity.Velocity.Domain.Models.NoSql;
using Service.Liquidity.Velocity.Grpc;
using Service.Liquidity.Velocity.Grpc.Models;

namespace Service.Liquidity.Velocity.Services
{
    public class MarkupVelocityService : IMarkupVelocityService
    {
        private readonly ILogger<MarkupVelocityService> _logger;
        private IMyNoSqlServerDataWriter<MarkupVelocitySettingsNoSql> _markupWriter;
        private IMyNoSqlServerDataWriter<MarkupVelocityNoSql> _velocityReader;
        private const int BulkRecordRequest = 100;
        public MarkupVelocityService(ILogger<MarkupVelocityService> logger, 
            IMyNoSqlServerDataWriter<MarkupVelocitySettingsNoSql> markupWriter,
            IMyNoSqlServerDataWriter<MarkupVelocityNoSql> velocityReader)
        {
            _logger = logger;
            _markupWriter = markupWriter;
            _velocityReader = velocityReader;
        }
        public async Task<SetHistoryDepthResponse> SetPeriodAsync(SetHistoryDepthRequest historyDepthRequest)
        {
            try
            {
                var item = new MarkupVelocitySettings
                {
                    Asset = historyDepthRequest.Asset,
                    Period = historyDepthRequest.Period
                };
                var itemNoSql = MarkupVelocitySettingsNoSql
                    .Create(historyDepthRequest.BrokerId, item);

                await _markupWriter.InsertOrReplaceAsync(itemNoSql);

                return new SetHistoryDepthResponse
                {
                    IsError = false,
                    ErrorMessage = null
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new SetHistoryDepthResponse
                {
                    IsError = true,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<GetVelocityResponse> GetAllVelocitiesAsync(GetVelocityRequest historyDepthRequest)
        {
            try
            {
                var items = await _velocityReader.GetAsync(historyDepthRequest.BrokerId);
                items ??= new List<MarkupVelocityNoSql>();
                return new GetVelocityResponse
                {
                    Items = items.Select(i => i.Velocity).ToList(),
                    IsError = false,
                    ErrorMessage = "",
                };
            }
            catch (Exception e)
            {
                    Console.WriteLine(e);
                    return new GetVelocityResponse
                    {
                        Items = null,
                        IsError = true,
                        ErrorMessage = e.Message,
                    };
            }
        }
    }
}

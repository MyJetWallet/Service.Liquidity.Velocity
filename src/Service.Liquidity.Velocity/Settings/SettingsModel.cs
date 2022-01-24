﻿using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.Liquidity.Velocity.Settings
{
    public class SettingsModel
    {
        [YamlProperty("LiquidityVelocity.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("LiquidityVelocity.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("LiquidityVelocity.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("LiquidityVelocity.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }        
        
        [YamlProperty("LiquidityVelocity.CandlesServiceGrpcUrl")]
        public string CandlesServiceGrpcUrl { get; set; }    
        
        [YamlProperty("LiquidityVelocity.AssetDictionaryGrpcServiceUrl")]
        public string AssetDictionaryGrpcServiceUrl { get; set; }  
        

    }
}

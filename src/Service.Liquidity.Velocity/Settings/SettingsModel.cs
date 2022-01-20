using MyJetWallet.Sdk.Service;
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
    }
}

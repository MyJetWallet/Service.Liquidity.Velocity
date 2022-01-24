using Autofac;
using JetBrains.Annotations;
using Microsoft.CSharp.RuntimeBinder;
using MyJetWallet.Sdk.Grpc;
using MyJetWallet.Sdk.NoSql;
using Service.AssetsDictionary.Client.Grpc;
using Service.AssetsDictionary.Grpc;
using Service.Liquidity.TradingPortfolio.Client;
using SimpleTrading.CandlesHistory.Grpc;


namespace Service.Liquidity.Velocity.Modules
{
    public class ClientsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // var myNoSqlClient = builder.CreateNoSqlClient(
            //     Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));
            
            var assetDictionaryFactory = new AssetsDictionaryClientFactory(Program.Settings.AssetDictionaryGrpcServiceUrl);
            builder
                .RegisterInstance(assetDictionaryFactory.GetSpotInstrumentsDictionaryService())
                .As<ISpotInstrumentsDictionaryService>()
                .SingleInstance();
            
            var factory = new SimpleTradingCandlesHistoryClientFactory(Program.Settings.CandlesServiceGrpcUrl);
            builder.RegisterInstance(factory.GetSimpleTradingCandlesHistoryService()).As<ISimpleTradingCandlesHistoryGrpc>().SingleInstance();

            builder.RegisterLiquidityTradingPortfolioClient(Program.Settings.LiquidityTradingPortfolioServiceUrl);
        }
    }

    [UsedImplicitly]
    public class SimpleTradingCandlesHistoryClientFactory: MyGrpcClientFactory
    {
        public SimpleTradingCandlesHistoryClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }
        public ISimpleTradingCandlesHistoryGrpc GetSimpleTradingCandlesHistoryService() => CreateGrpcService<ISimpleTradingCandlesHistoryGrpc>();
    }
}
using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;

namespace Service.Liquidity.Velocity.Client
{
    [UsedImplicitly]
    public class LiquidityVelocityClientFactory: MyGrpcClientFactory
    {
        public LiquidityVelocityClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        //public IHelloService GetHelloService() => CreateGrpcService<IHelloService>();
    }
}

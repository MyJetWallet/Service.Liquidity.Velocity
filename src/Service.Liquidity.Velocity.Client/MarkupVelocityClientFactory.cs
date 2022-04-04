using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.Liquidity.Velocity.Grpc;

namespace Service.Liquidity.Velocity.Client
{
    [UsedImplicitly]
    public class MarkupVelocityClientFactory : MyGrpcClientFactory
    {
        public MarkupVelocityClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IMarkupVelocityService GetManualInputService() => CreateGrpcService<IMarkupVelocityService>();
    }
}

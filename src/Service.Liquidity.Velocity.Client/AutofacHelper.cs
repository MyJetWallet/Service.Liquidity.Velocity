using Autofac;

// ReSharper disable UnusedMember.Global

namespace Service.Liquidity.Velocity.Client
{
    public static class AutofacHelper
    {
        public static void RegisterLiquidityVelocityClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new LiquidityVelocityClientFactory(grpcServiceUrl);
        }
    }
}

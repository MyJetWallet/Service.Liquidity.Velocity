using Autofac;
using Service.Liquidity.Velocity.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.Liquidity.Velocity.Client
{
    public static class AutofacHelper
    {
        public static void RegisterLiquidityVelocityClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new LiquidityVelocityClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetHelloService()).As<IHelloService>().SingleInstance();
        }
    }
}

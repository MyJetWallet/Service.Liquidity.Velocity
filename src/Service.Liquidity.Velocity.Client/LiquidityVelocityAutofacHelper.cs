using Autofac;
using MyJetWallet.Sdk.NoSql;
using MyNoSqlServer.DataReader;
using Service.Liquidity.Velocity.Domain.Models.NoSql;

// ReSharper disable UnusedMember.Global

namespace Service.Liquidity.Velocity.Client
{
    public static class LiquidityVelocityAutofacHelper
    {
        /// <summary>
        /// Register interface ILiquidityVelocityClient
        /// </summary>
        public static void RegisterLiquidityVelocityClient(this ContainerBuilder builder,
            MyNoSqlTcpClient myNoSqlcClient)
        {
            builder.RegisterMyNoSqlReader<VelocityNoSql>(myNoSqlcClient, VelocityNoSql.TableName);
            builder
                .RegisterType<LiquidityVelocityClient>()
                .As<ILiquidityVelocityClient>()
                .AutoActivate()
                .SingleInstance();
        }

        /// <summary>
        /// Register interface IMarkupVelocityClient
        /// </summary>
        public static void RegisterMarkupVelocityClient(this ContainerBuilder builder,
            MyNoSqlTcpClient myNoSqlcClient)
        {
            builder.RegisterMyNoSqlReader<MarkupVelocityNoSql>(myNoSqlcClient, MarkupVelocityNoSql.TableName);
            builder
                .RegisterType<MarkupVelocityClient>()
                .As<IMarkupVelocityClient>()
                .AutoActivate()
                .SingleInstance();
        }
    }
    

}

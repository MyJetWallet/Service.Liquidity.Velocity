using System;
using Autofac;
using MyJetWallet.Sdk.NoSql;
using MyNoSqlServer.DataReader;
using Service.Liquidity.Velocity.Domain.Models.NoSql;
using Service.Liquidity.Velocity.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.Liquidity.Velocity.Client
{
    public static class AutofacHelper
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

        /// <summary>
        /// Register interface IMarkupVelocityClient
        /// </summary>
        public static void RegisterMarkupVelocitySettingsClient(this ContainerBuilder builder,
            MyNoSqlTcpClient myNoSqlcClient)
        {
            builder.RegisterMyNoSqlReader<MarkupVelocitySettingsNoSql>(myNoSqlcClient, MarkupVelocitySettingsNoSql.TableName);
            builder
                .RegisterType<MarkupVelocitySettingsClient>()
                .As<IMarkupVelocitySettingsClient>()
                .AutoActivate()
                .SingleInstance();
        }

        public static void RegisterMarkupVelocityService(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new MarkupVelocityClientFactory(grpcServiceUrl);
            builder.RegisterInstance(factory.GetManualInputService()).As<IMarkupVelocityService>().SingleInstance();
        }
        
        [Obsolete("Please use RegisterMarkupVelocityService", false)]
        public static void MarkupVelocityClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new MarkupVelocityClientFactory(grpcServiceUrl);
            builder.RegisterInstance(factory.GetManualInputService()).As<IMarkupVelocityService>().SingleInstance();
        }
    }
    

}

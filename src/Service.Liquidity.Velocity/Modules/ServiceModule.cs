using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.NoSql;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using Service.Liquidity.Velocity.Domain.Models.NoSql;
using Service.Liquidity.Velocity.Jobs;

namespace Service.Liquidity.Velocity.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var noSqlClient = builder.CreateNoSqlClient(Program.Settings.MyNoSqlReaderHostPort, Program.LogFactory);
            builder.RegisterMyNoSqlReader<MarkupVelocitySettingsNoSql>(noSqlClient, MarkupVelocitySettingsNoSql.TableName);

            builder.RegisterMyNoSqlWriter<VelocityNoSql>(
                () => Program.Settings.MyNoSqlWriterUrl, VelocityNoSql.TableName);

            builder.RegisterMyNoSqlWriter<MarkupVelocityNoSql>(
                () => Program.Settings.MyNoSqlWriterUrl, MarkupVelocityNoSql.TableName);

            builder.RegisterMyNoSqlWriter<MarkupVelocitySettingsNoSql>(
                () => Program.Settings.MyNoSqlWriterUrl, MarkupVelocitySettingsNoSql.TableName);

            builder.RegisterType<LiquidityVelocityCalcBackgroundService>()
                .SingleInstance()
                .AutoActivate()
                .AsSelf();

            builder.RegisterType<MarkupVelocityCalcBackgroundService>()
                .SingleInstance()
                .AutoActivate()
                .AsSelf();
        }
    }
}

using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.NoSql;
using Service.Liquidity.Velocity.Domain.Models.NoSql;
using Service.Liquidity.Velocity.Services;

namespace Service.Liquidity.Velocity.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMyNoSqlWriter<VelocityNoSql>(
                Program.ReloadedSettings(e => e.MyNoSqlWriterUrl),
                VelocityNoSql.TableName);
            
            builder.RegisterType<VelocityCalcBackgroundService>().SingleInstance().AutoActivate().AsSelf();

        }
    }
}

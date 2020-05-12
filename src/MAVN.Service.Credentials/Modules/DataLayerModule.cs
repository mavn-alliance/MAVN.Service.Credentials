using Autofac;
using JetBrains.Annotations;
using MAVN.Common.MsSql;
using MAVN.Service.Credentials.MsSqlRepositories.Repositories;
using MAVN.Service.Credentials.Domain.Repositories;
using MAVN.Service.Credentials.MsSqlRepositories.Contexts;
using MAVN.Service.Credentials.Settings;
using Lykke.SettingsReader;

namespace MAVN.Service.Credentials.Modules
{
    [UsedImplicitly]
    public class DataLayerModule : Module
    {
        private readonly DbSettings _settings;

        public DataLayerModule(IReloadingManager<AppSettings> appSettings)
        {
            _settings = appSettings.CurrentValue.CredentialsService.Db;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMsSql(
                _settings.DataConnString,
                connString => new CredentialsContext(connString, false),
                dbConn => new CredentialsContext(dbConn));

            builder.RegisterType<AdminCredentialsRepository>()
                .As<IAdminCredentialsRepository>()
                .SingleInstance();

            builder.RegisterType<CustomerCredentialsRepository>()
                .As<ICustomerCredentialsRepository>()
                .SingleInstance();
            
            builder.RegisterType<PasswordResetRepository>()
                .As<IPasswordResetRepository>()
                .SingleInstance();

            builder.RegisterType<PartnerCredentialsRepository>()
                .As<IPartnerCredentialsRepository>()
                .SingleInstance();
        }
    }
}


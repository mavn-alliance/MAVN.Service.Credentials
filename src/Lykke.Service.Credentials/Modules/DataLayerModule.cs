using Autofac;
using JetBrains.Annotations;
using Lykke.Common.MsSql;
using Lykke.Service.Credentials.MsSqlRepositories.Repositories;
using Lykke.Service.Credentials.Domain.Repositories;
using Lykke.Service.Credentials.MsSqlRepositories.Contexts;
using Lykke.Service.Credentials.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.Credentials.Modules
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


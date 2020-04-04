using Autofac;
using Falcon.Common;
using JetBrains.Annotations;
using MAVN.Service.Credentials.Domain.Helpers;
using MAVN.Service.Credentials.Domain.Models;
using MAVN.Service.Credentials.Domain.Services;
using MAVN.Service.Credentials.DomainServices;
using MAVN.Service.Credentials.DomainServices.Helpers;
using MAVN.Service.Credentials.Settings;
using Lykke.SettingsReader;
using StackExchange.Redis;

namespace MAVN.Service.Credentials.Modules
{
    [UsedImplicitly]
    public class ServiceModule : Module
    {
        private readonly AppSettings _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings.CurrentValue;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CustomerCredentialsService>()
                .As<ICustomerCredentialsService>()
                .SingleInstance();

            builder.RegisterType<AdminCredentialsService>()
                .As<IAdminCredentialsService>()
                .SingleInstance();

            builder.Register(context =>
            {
                var connectionMultiplexer =
                    ConnectionMultiplexer.Connect(_appSettings.CredentialsService.Redis.ConnString);
                return connectionMultiplexer;
            }).As<IConnectionMultiplexer>().SingleInstance();

            builder.RegisterType<PasswordResetService>()
                .As<IPasswordResetService>()
                .SingleInstance()
                .WithParameter("maxAllowedRequestsNumber",
                    _appSettings.CredentialsService.LimitationSettings.MaxAllowedRequestsNumber)
                .WithParameter("instanceName", _appSettings.CredentialsService.Redis.InstanceName)
                .WithParameter("monitoredPeriod", _appSettings.CredentialsService.LimitationSettings.MonitoredPeriod)
                .WithParameter("identifierTimeSpan", _appSettings.CredentialsService.IdentifierTimeSpan)
                .WithParameter("resetIdentifierLength", _appSettings.CredentialsService.ResetIdentifierLength);

            builder.RegisterType<ResetIdentifierService>()
                .As<IResetIdentifierService>()
                .SingleInstance()
                .WithParameter("maxAllowedRequestsNumber",
                    _appSettings.CredentialsService.LimitationSettings.MaxAllowedRequestsNumber)
                .WithParameter("instanceName", _appSettings.CredentialsService.Redis.InstanceName)
                .WithParameter("monitoredPeriod", _appSettings.CredentialsService.LimitationSettings.MonitoredPeriod)
                .WithParameter("identifierTimeSpan", _appSettings.CredentialsService.IdentifierTimeSpan)
                .WithParameter("resetIdentifierLength", _appSettings.CredentialsService.ResetIdentifierLength);
            
            builder.RegisterType<Sha256HashingUtil>()
                .AsSelf()
                .SingleInstance();

            var passwordRules = new PasswordValidationRulesDto
            {
                AllowWhiteSpaces = _appSettings.CredentialsService.PasswordValidationSettings.AllowWhiteSpaces,
                MinLength = _appSettings.CredentialsService.PasswordValidationSettings.MinLength,
                MinUpperCase = _appSettings.CredentialsService.PasswordValidationSettings.MinUpperCase,
                MinLowerCase = _appSettings.CredentialsService.PasswordValidationSettings.MinLowerCase,
                MinNumbers = _appSettings.CredentialsService.PasswordValidationSettings.MinNumbers,
                AllowedSpecialSymbols =
                    _appSettings.CredentialsService.PasswordValidationSettings.AllowedSpecialSymbols,
                MinSpecialSymbols = _appSettings.CredentialsService.PasswordValidationSettings.MinSpecialSymbols,
                MaxLength = _appSettings.CredentialsService.PasswordValidationSettings.MaxLength,
            };
            builder.RegisterType<PasswordValidator>()
                .WithParameter(TypedParameter.From(passwordRules))
                .As<IPasswordValidator>()
                .SingleInstance();

            builder.RegisterType<PartnerCredentialsService>()
                .As<IPartnerCredentialsService>()
                .SingleInstance();

            builder.RegisterType<Base34Util>()
                .As<IBase34Util>()
                .SingleInstance();
            
            builder.RegisterType<CredentialsGeneratorService>()
                .As<ICredentialsGeneratorService>()
                .SingleInstance();

            builder.RegisterType<SettingsService>()
                .As<ISettingsService>()
                .WithParameter(TypedParameter.From(_appSettings.CredentialsService.PinCodeLength))
                .SingleInstance();
        }
    }
}

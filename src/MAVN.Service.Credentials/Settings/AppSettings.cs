using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace MAVN.Service.Credentials.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public CredentialsSettings CredentialsService { get; set; }
    }
}

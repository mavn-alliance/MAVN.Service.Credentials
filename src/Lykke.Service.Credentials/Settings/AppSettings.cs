using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Service.Credentials.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public CredentialsSettings CredentialsService { get; set; }
    }
}

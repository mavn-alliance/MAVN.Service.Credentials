using System;
using JetBrains.Annotations;

namespace MAVN.Service.Credentials.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class CredentialsSettings
    {
        public DbSettings Db { get; set; }
        public RedisSettings Redis { get; set; }
        public LimitationSettings LimitationSettings { get; set; }    
        public TimeSpan IdentifierTimeSpan { get; set; }
        public int ResetIdentifierLength { get; set; }
        public PasswordValidationSettings PasswordValidationSettings { get; set; }
        public int PinCodeLength { get; set; }
    }
}

using Lykke.Service.Credentials.Domain.Services;

namespace Lykke.Service.Credentials.DomainServices
{
    public class SettingsService : ISettingsService
    {
        public int PinCodeLength { get; }

        public SettingsService(int pinCodeLength)
        {
            PinCodeLength = pinCodeLength;
        }
    }
}

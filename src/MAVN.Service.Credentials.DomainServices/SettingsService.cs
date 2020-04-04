using MAVN.Service.Credentials.Domain.Services;

namespace MAVN.Service.Credentials.DomainServices
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

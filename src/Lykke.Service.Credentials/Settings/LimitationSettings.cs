using System;

namespace Lykke.Service.Credentials.Settings
{
    public class LimitationSettings
    {
        public int MaxAllowedRequestsNumber { get; set; }
        public TimeSpan MonitoredPeriod { get; set; }
    }
}

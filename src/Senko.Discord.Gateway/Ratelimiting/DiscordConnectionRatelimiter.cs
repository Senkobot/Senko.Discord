using System;
using System.Threading.Tasks;

namespace Senko.Discord.Gateway.Ratelimiting
{
    public class DiscordConnectionRatelimiter : IDiscordConnectionRatelimiter
    {
        private DateTime _lastIdentifyAccepted = DateTime.MinValue;

        public ValueTask<bool> CanIdentifyAsync()
        {
            if (_lastIdentifyAccepted.AddSeconds(5) > DateTime.Now)
            {
                return new ValueTask<bool>(false);
            }

            _lastIdentifyAccepted = DateTime.Now;
            return new ValueTask<bool>(true);
        }
    }
}

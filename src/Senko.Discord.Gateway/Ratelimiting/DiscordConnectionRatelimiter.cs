using System;
using System.Threading.Tasks;

namespace Senko.Discord.Gateway.Ratelimiting
{
    public class DiscordConnectionRatelimiter : IDiscordConnectionRatelimiter
    {
        private DateTime _lastIdentifyAccepted = DateTime.MinValue;

        public Task<bool> CanIdentifyAsync()
        {
            if (_lastIdentifyAccepted.AddSeconds(5) > DateTime.Now)
            {
                return Task.FromResult(false);
            }

            _lastIdentifyAccepted = DateTime.Now;
            return Task.FromResult(true);
        }
    }
}

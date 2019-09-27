using System.Threading.Tasks;

namespace Senko.Discord.Gateway.Ratelimiting
{
    public interface IDiscordConnectionRatelimiter
    {
        ValueTask<bool> CanIdentifyAsync();
    }
}

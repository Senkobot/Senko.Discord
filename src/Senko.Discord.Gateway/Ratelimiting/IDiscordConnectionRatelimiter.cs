using System.Threading.Tasks;

namespace Senko.Discord.Gateway.Ratelimiting
{
    public interface IDiscordConnectionRatelimiter
    {
        Task<bool> CanIdentifyAsync();
    }
}

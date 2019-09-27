using System.Threading.Tasks;

namespace Senko.Discord.Rest.Http
{
    public interface IDiscordApiRateLimiter
    {
        ValueTask<bool> CanStartRequestAsync(string method, string requestUri);

        ValueTask OnRequestSuccessAsync(HttpResponse response);
    }
}
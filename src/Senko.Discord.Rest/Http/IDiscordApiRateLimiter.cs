using System.Threading.Tasks;

namespace Senko.Discord.Rest.Http
{
    public interface IDiscordApiRateLimiter
    {
		Task<bool> CanStartRequestAsync(RequestMethod method, string requestUri);

		Task OnRequestSuccessAsync(HttpResponse response);
    }
}
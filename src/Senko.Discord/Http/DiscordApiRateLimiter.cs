﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Foundatio.Caching;
using Senko.Discord.Packets;
using Senko.Discord.Rest.Http;

namespace Senko.Discord.Http
{
    public class DiscordApiRateLimiter : IDiscordApiRateLimiter
    {
        private readonly ICacheClient _cache;

        const string LimitHeader = "X-RateLimit-Limit";
        const string RemainingHeader = "X-RateLimit-Remaining";
        const string ResetHeader = "X-RateLimit-Reset";
        const string GlobalHeader = "X-RateLimit-Global";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetCacheKey(string route, string id)
            => $"discord:ratelimit:{route}:{id}";

        public DiscordApiRateLimiter(ICacheClient cache)
        {
            _cache = cache;
        }

        public async ValueTask<bool> CanStartRequestAsync(string method, string requestUri)
        {
            string key = GetCacheKey(requestUri.Split('/')[0], requestUri.Split('/')[1]);

            var cache = await _cache.GetAsync<Ratelimit>(key);

            if (!cache.HasValue)
            {
                return true;
            }

            var rateLimit = cache.Value;

            rateLimit.Remaining--;

            await _cache.SetAsync(key, rateLimit);

            return !rateLimit.IsRatelimited();
        }

        public ValueTask OnRequestSuccessAsync(HttpResponse response)
        {
            var httpMessage = response.HttpResponseMessage;

            Uri requestUri = httpMessage.RequestMessage.RequestUri;
            string[] paths = requestUri.AbsolutePath.Split('/');
            string key = GetCacheKey(paths[2], paths[3]);

            if (!httpMessage.Headers.Contains(LimitHeader))
            {
                return default;
            }

            var rateLimit = new Ratelimit();

            if (httpMessage.Headers.TryGetValues(RemainingHeader, out var values))
            {
                rateLimit.Remaining = int.Parse(values.First());
            }

            if (httpMessage.Headers.TryGetValues(LimitHeader, out var limitValues))
            {
                rateLimit.Limit = int.Parse(limitValues.First());
            }

            if (httpMessage.Headers.TryGetValues(ResetHeader, out var resetValues))
            {
                rateLimit.Reset = int.Parse(resetValues.First());
            }

            if (httpMessage.Headers.TryGetValues(GlobalHeader, out var globalValues))
            {
                rateLimit.Global = int.Parse(globalValues.First());
            }

            return new ValueTask(_cache.SetAsync(key, rateLimit));
        }
    }
}

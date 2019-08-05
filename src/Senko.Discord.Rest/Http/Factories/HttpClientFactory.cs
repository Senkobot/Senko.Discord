using System;

namespace Senko.Discord.Rest.Http.Factories
{
    public class HttpClientFactory
    {
		internal struct HttpClientFactoryProperties
		{
			public Uri BaseUri { get; internal set; }
			public IDiscordApiRateLimiter RateLimiter { get; internal set; }
		}

		private HttpClientFactoryProperties _properties;

		public HttpClientFactory()
		{
			_properties = new HttpClientFactoryProperties();
		}

		public HttpClient CreateNew()
		{
			HttpClient client = new HttpClient();

			if (_properties.BaseUri != null)
			{
				client._client.BaseAddress = _properties.BaseUri;
			}

			if(_properties.RateLimiter != null)
			{
				client._rateLimiter = _properties.RateLimiter;
			}

			return client;
		}

		public HttpClientFactory HasBaseUri(string baseUri)
		{
			return HasBaseUri(new Uri(baseUri));
		}
		public HttpClientFactory HasBaseUri(Uri baseUri)
		{
			_properties.BaseUri = baseUri;
			return this;
		}

		public HttpClientFactory WithRateLimiter(IDiscordApiRateLimiter rateLimiter)
		{
			_properties.RateLimiter = rateLimiter;
			return this;
		}
	}
}

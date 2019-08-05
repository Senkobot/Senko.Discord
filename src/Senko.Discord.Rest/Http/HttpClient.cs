using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Senko.Discord.Rest.Http.Exceptions;

namespace Senko.Discord.Rest.Http
{
	public class HttpClient : IDisposable
	{
		public HttpRequestHeaders Headers => _client.DefaultRequestHeaders;

		internal readonly System.Net.Http.HttpClient _client;
		internal readonly bool _ensureSuccess;

		internal IDiscordApiRateLimiter _rateLimiter;

		public event Action<string, string> OnRequestComplete;

		public HttpClient(string base_address, bool ensureSuccess = false)
			: this()
		{
			_client.BaseAddress = new Uri(base_address);
			_ensureSuccess = ensureSuccess;
		}
		internal HttpClient()
		{
			_client = new System.Net.Http.HttpClient();
		}

		public HttpClient AddHeader(string header, string value)
		{
			_client.DefaultRequestHeaders.Add(header, value);
			return this;
		}

		public void Dispose()
		{
			_client.Dispose();
		}

		public async Task<HttpResponse> DeleteAsync(string url)
			=> await SendAsync(RequestMethod.DELETE, url);

		public async Task<HttpResponse> GetAsync(string url)
			=> await SendAsync(RequestMethod.GET, url);

		public async Task<Stream> GetStreamAsync(string url)
		{
			var response = await GetAsync(url);
			return await response.HttpResponseMessage.Content.ReadAsStreamAsync();
		}

		public Task<HttpResponse> PostAsync(string url, string value = null)
			=> SendAsync(RequestMethod.POST, url, value);

        public Task<HttpResponse> PostJsonAsync<T>(string url, T value)
            => SendJsonAsync(RequestMethod.POST, url, value);

        public Task<HttpResponse> PatchAsync(string url, string value = null)
			=> SendAsync(RequestMethod.PATCH, url, value);
        
        public Task<HttpResponse> PatchJsonAsync<T>(string url, T value)
            => SendJsonAsync(RequestMethod.PATCH, url, value);

		public Task<HttpResponse> PutAsync(string url, string value = null)
			=> SendAsync(RequestMethod.PUT, url, value);
        
        public Task<HttpResponse> PutJsonAsync<T>(string url, T value)
            => SendJsonAsync(RequestMethod.PUT, url, value);


		public async Task<HttpResponse> PostMultipartAsync(string url, params MultiformItem[] items)
		{
			var req = new HttpRequestMessage(new HttpMethod("POST"), _client.BaseAddress + url);

			string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");

			req.Headers.Add("Connection", "keep-alive");
			req.Headers.Add("Keep-Alive", "600");

			var content = new MultipartFormDataContent(boundary);
			if (items != null && items.Any())
				foreach (var kvp in items)
					content.Add(kvp.Content, kvp.Name);

			req.Content = content;

			return await SendAsync(req);
		}

		public HttpClient SetAuthorization(string key)
		{
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(key);
			return this;
		}
		public HttpClient SetAuthorization(string scheme, string value)
		{
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, value);
			return this;
		}

        public async Task<HttpResponse> SendAsync(RequestMethod method, string url)
        {
            url = url.TrimStart('/');

            var m = new HttpMethod(method.ToString().ToUpper());
            using (var msg = new HttpRequestMessage(m, url))
            {
                return await SendAsync(msg);
            }
        }

		public async Task<HttpResponse> SendAsync(RequestMethod method, string url, string value)
		{
			url = url.TrimStart('/');

			HttpMethod m = new HttpMethod(method.ToString().ToUpper());
			using (HttpRequestMessage msg = new HttpRequestMessage(m, url))
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					msg.Content = new StringContent(value, Encoding.UTF8, "application/json");
				}

				return await SendAsync(msg);
			}
		}

        public async Task<HttpResponse> SendJsonAsync<T>(RequestMethod method, string url, T value)
        {
            var json = JsonHelper.SerializeFromPool(value);
            var response = await SendAsync(method, url, json, "application/json");
            JsonHelper.ReturnToPool(json);
            return response;
        }

        public async Task<HttpResponse> SendAsync(RequestMethod method, string url, ArraySegment<byte> value, string contentType = null)
        {
            url = url.TrimStart('/');

            HttpMethod m = new HttpMethod(method.ToString().ToUpper());
            using (HttpRequestMessage msg = new HttpRequestMessage(m, url))
            {
                if (value != null)
                {
                    msg.Content = new ByteArrayContent(value.Array, value.Offset, value.Count);

                    if (contentType != null)
                    {
                        msg.Content.Headers.Add("Content-Type", contentType);
                    }
                }

                return await SendAsync(msg);
            }
        }

		public async Task<HttpResponse> SendAsync(HttpRequestMessage message)
		{
			if(_rateLimiter != null)
			{
				if(!await _rateLimiter.CanStartRequestAsync(
					(RequestMethod)Enum.Parse(typeof(RequestMethod), message.Method.Method), 
					message.RequestUri.ToString()
				))
				{
					throw new RateLimitException(message.RequestUri);
				}
			}

			var response = await _client.SendAsync(message);
            var restResponse = new HttpResponse
            {
                HttpResponseMessage = response,
                Body = await response.Content.ReadAsByteArrayAsync(),
                Success = response.IsSuccessStatusCode
            };

            if (restResponse.Success)
			{
				if (_rateLimiter != null)
				{
					await _rateLimiter.OnRequestSuccessAsync(restResponse);
				}

				OnRequestComplete?.Invoke(
					message.Method.Method, 
					message.RequestUri.AbsolutePath
				);
			}

			if (_ensureSuccess)
			{
				response.EnsureSuccessStatusCode();
			}

			return restResponse;
		}
	}

	// Probably move this to some object file.
	public class MultiformItem
	{
		public HttpContent Content { get; set; }
		public string Name { get; set; }
		public string FileName { get; set; } = null;
	}
}

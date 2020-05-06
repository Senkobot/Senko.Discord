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

		public ValueTask<HttpResponse> DeleteAsync(string url)
        {
            return SendAsync(HttpMethod.Delete, url);
        }

        public ValueTask<HttpResponse> GetAsync(string url)
        {
            return SendAsync(HttpMethod.Get, url);
        }

        public async ValueTask<Stream> GetStreamAsync(string url)
		{
			var response = await GetAsync(url);

			return await response.HttpResponseMessage.Content.ReadAsStreamAsync();
		}

		public ValueTask<HttpResponse> PostAsync(string url, string value = null)
			=> SendAsync(HttpMethod.Post, url, value);

        public ValueTask<HttpResponse> PostJsonAsync<T>(string url, T value)
            => SendJsonAsync(HttpMethod.Post, url, value);

        public ValueTask<HttpResponse> PatchAsync(string url, string value = null)
			=> SendAsync(HttpMethod.Patch, url, value);
        
        public ValueTask<HttpResponse> PatchJsonAsync<T>(string url, T value)
            => SendJsonAsync(HttpMethod.Patch, url, value);

		public ValueTask<HttpResponse> PutAsync(string url, string value = null)
			=> SendAsync(HttpMethod.Put, url, value);
        
        public ValueTask<HttpResponse> PutJsonAsync<T>(string url, T value)
            => SendJsonAsync(HttpMethod.Put, url, value);


		public ValueTask<HttpResponse> PostMultipartAsync(string url, params MultiformItem[] items)
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

			return SendAsync(req);
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

        public async ValueTask<HttpResponse> SendAsync(HttpMethod method, string url)
        {
            url = url.TrimStart('/');

            var m = new HttpMethod(method.ToString().ToUpper());
            using (var msg = new HttpRequestMessage(m, url))
            {
                return await SendAsync(msg);
            }
        }

		public async ValueTask<HttpResponse> SendAsync(HttpMethod method, string url, string value)
		{
			url = url.TrimStart('/');

			using (var msg = new HttpRequestMessage(method, url))
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					msg.Content = new StringContent(value, Encoding.UTF8, "application/json");
				}

				return await SendAsync(msg);
			}
		}

        public ValueTask<HttpResponse> SendJsonAsync<T>(HttpMethod method, string url, T value)
        {
            var json = JsonHelper.Serialize(value);
            return SendAsync(method, url, json, "application/json");
        }

        public async ValueTask<HttpResponse> SendAsync(HttpMethod method, string url, byte[] value, string contentType = null)
        {
            url = url.TrimStart('/');

            using (var msg = new HttpRequestMessage(method, url))
            {
                if (value != null)
                {
                    msg.Content = new ByteArrayContent(value);

                    if (contentType != null)
                    {
                        msg.Content.Headers.Add("Content-Type", contentType);
                    }
                }

                return await SendAsync(msg);
            }
        }

		public async ValueTask<HttpResponse> SendAsync(HttpRequestMessage message)
		{
			if (_rateLimiter != null)
			{
				await _rateLimiter.WaitAsync(
					message.Method.Method,
					message.RequestUri.ToString()
				);
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

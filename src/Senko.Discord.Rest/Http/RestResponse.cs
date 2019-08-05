using System.Net.Http;

namespace Senko.Discord.Rest.Http
{
	public class HttpResponse
	{
		public byte[] Body { get; internal set; }

		public HttpResponseMessage HttpResponseMessage { get; internal set; }

		public bool Success { get; internal set; }
	}
}
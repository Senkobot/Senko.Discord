using System;
using System.Net.Http;

namespace Senko.Discord.Rest.Http.Exceptions
{
    public class RateLimitException : HttpRequestException
    {
		private Uri _msg;	

		public override string Message => $"Request '{_msg}' was blocked for exceeding the ratelimit.";

		public RateLimitException(Uri message) : base()
		{
			_msg = message;
		}
	}
}

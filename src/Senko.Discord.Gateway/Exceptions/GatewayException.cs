using System;

namespace Senko.Discord.Gateway
{
    public class GatewayException : Exception
    {
        public GatewayException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

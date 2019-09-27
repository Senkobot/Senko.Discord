using System;

namespace Senko.Discord.Exceptions
{
    public class DiscordGatewayException : DiscordException
    {
        public DiscordGatewayException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

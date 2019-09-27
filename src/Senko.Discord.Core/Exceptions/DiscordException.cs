using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text;

namespace Senko.Discord.Exceptions
{
    public class DiscordException : Exception
    {
        public DiscordException()
        {
        }

        protected DiscordException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DiscordException(string message) : base(message)
        {
        }

        public DiscordException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

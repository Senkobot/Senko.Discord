namespace Senko.Discord.Exceptions
{
    public class DiscordRestException : DiscordException
    {
        public DiscordRestException(string message, int code)
            : base(message)
        {
            Code = code;
        }

        public int Code { get; }
    }
}

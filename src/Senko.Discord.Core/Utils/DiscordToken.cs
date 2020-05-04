using System;

namespace Senko.Discord
{
    public struct DiscordToken
    {
        public DiscordToken(string token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (token
                .ToLowerInvariant()
                .StartsWith("bearer "))
            {
                Token = token.Substring(7);
                Type = TokenType.BEARER;
            }
            else if (token
                .ToLowerInvariant()
                .StartsWith("bot "))
            {
                Token = token.Substring(4);
                Type = TokenType.BOT;
            }
            else
            {
                Token = token;
                Type = TokenType.BOT;
            }
        }

        public string Token { get; }
        public TokenType Type { get; }

        public string GetOAuthType()
        {
            var x = Type.ToString()
                .ToLowerInvariant()
                .ToCharArray();
            x[0] = char.ToUpperInvariant(x[0]);
            return new string(x);
        }

        public static implicit operator DiscordToken(string token)
            => new DiscordToken(token);
    }
}

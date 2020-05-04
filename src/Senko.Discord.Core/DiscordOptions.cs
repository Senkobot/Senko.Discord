using Senko.Discord.Gateway;

namespace Senko.Discord
{
    public class DiscordOptions
    {
        /// <summary>
        /// The gateway version.
        /// </summary>
        public int Version { get; set; } = 6;

        /// <summary>
        /// Your secret Discord Bot token.
        /// </summary>
		public string Token { get; set; }

        /// <summary>
        /// Whether the gateway should receive gzip-compressed packets.
        /// </summary>
		public bool EnableCompression { get; set; }

        /// <summary>
        /// Total shards running on this token
        /// </summary>
		public int ShardAmount { get; set; } = 1;

        /// <summary>
        /// The enabled intents.
        /// </summary>
        public GatewayIntent? Intents { get; set; }
    }
}
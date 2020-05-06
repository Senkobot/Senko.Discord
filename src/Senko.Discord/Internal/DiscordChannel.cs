using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
	public class DiscordChannel : IDiscordChannel
	{
		protected DiscordChannelPacket _packet;
		protected IDiscordClient _client;

		public DiscordChannel(DiscordChannelPacket packet, IDiscordClient client)
		{
			_packet = packet;
			_client = client;
		}

		public string Name
			=> _packet.Name;

		public ulong Id
			=> _packet.Id;

        public ulong? GuildId
            => _packet.GuildId;

        public bool IsNsfw
			=> _packet?.IsNsfw
                .GetValueOrDefault(false) 
                    ?? false;

		public ValueTask DeleteAsync()
		{
			return _client.DeleteChannelAsync(Id);
		}
	}
}
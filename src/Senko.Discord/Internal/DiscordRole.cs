using Senko.Discord.Packets;

namespace Senko.Discord
{
	public class DiscordRole : IDiscordRole
    {
		private readonly DiscordRolePacket _packet;
		private readonly IDiscordClient _client;
        private string _escapedName;

        public DiscordRole(DiscordRolePacket packet, IDiscordClient client)
		{
			_packet = packet;
			_client = client;
		}

		public string Name
			=> _escapedName ??= _client.EscapeEveryoneAndHere(_packet.Name);

        public string RawName
            => _packet.Name;

        public Color Color
			=> new Color((uint)_packet.Color);

		public int Position
			=> _packet.Position;

		public ulong Id
			=> _packet.Id;

		public GuildPermission Permissions
			=> (GuildPermission)_packet.Permissions;

        public bool IsManaged 
            => _packet.Managed;

        public bool IsHoisted 
            => _packet.IsHoisted;

        public bool IsMentionable 
            => _packet.Mentionable;
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Senko.Discord.Internal;
using Senko.Discord.Packets;

namespace Senko.Discord
{
    public static class ClientExtensions
    {
        public static IDiscordChannel GetChannelFromPacket(this IDiscordClient client, DiscordChannelPacket packet)
        {
            switch (packet.Type)
            {
                case ChannelType.GUILDTEXT:
                case ChannelType.GUILDNEWS:
                    return packet.GuildId.HasValue ? new DiscordGuildTextChannel(packet, client, packet.GuildId.Value) : null;

                case ChannelType.CATEGORY:
                case ChannelType.GUILDVOICE:
                    return packet.GuildId.HasValue ? new DiscordGuildChannel(packet, client, packet.GuildId.Value) : null;

                case ChannelType.DM:
                case ChannelType.GROUPDM:
                    return new DiscordTextChannel(packet, client);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

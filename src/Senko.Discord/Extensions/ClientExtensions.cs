using System;
using System.Collections.Generic;
using System.Text;
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
                    return new DiscordGuildTextChannel(packet, client);

                case ChannelType.CATEGORY:
                case ChannelType.GUILDVOICE:
                    return new DiscordGuildChannel(packet, client);

                case ChannelType.DM:
                case ChannelType.GROUPDM:
                    return new DiscordTextChannel(packet, client);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

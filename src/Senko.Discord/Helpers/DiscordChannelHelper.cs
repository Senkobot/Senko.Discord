using Senko.Discord.Internal;
using System.Threading.Tasks;
using Senko.Discord.Packets;
using Senko.Discord.Rest;

namespace Senko.Discord.Helpers
{
    internal static class DiscordChannelHelper
    {
        public static async ValueTask<DiscordMessagePacket> CreateMessageAsync(
            IDiscordApiClient client,
            DiscordChannelPacket channel,
            MessageArgs args)
        {
            var message = await client.SendMessageAsync(channel.Id, args);
            if(channel.Type == ChannelType.GUILDTEXT 
                || channel.Type == ChannelType.GUILDVOICE
                || channel.Type == ChannelType.CATEGORY)
            {
                message.GuildId = channel.GuildId;
            }
            return message;
        }
    }
}

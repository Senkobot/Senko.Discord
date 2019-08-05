using Senko.Discord.Internal;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord.Helpers
{
    internal static class DiscordChannelHelper
    {
        public static async Task<DiscordMessage> CreateMessageAsync(
            IDiscordClient client, 
            DiscordChannelPacket channel, 
            MessageArgs args)
        {
            var message = await client.ApiClient.SendMessageAsync(channel.Id, args);
            if(channel.Type == ChannelType.GUILDTEXT 
                || channel.Type == ChannelType.GUILDVOICE
                || channel.Type == ChannelType.CATEGORY)
            {
                message.GuildId = channel.GuildId;
            }
            return new DiscordMessage(message, client);
        }
    }
}

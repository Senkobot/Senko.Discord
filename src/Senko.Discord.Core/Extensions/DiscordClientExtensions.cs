using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Senko.Discord.Packets;

namespace Senko.Discord
{
    public static class DiscordClientExtensions
    {
        public static Task<IDiscordMessage> SendMessageAsync(this IDiscordClient client, ulong channelId, string content, DiscordEmbed embed = null)
            => client.SendMessageAsync(channelId, new MessageArgs(content, embed));

        public static Task<IDiscordMessage> SendMessageAsync(this IDiscordClient client, ulong channelId, DiscordEmbed embed)
            => client.SendMessageAsync(channelId, new MessageArgs(string.Empty, embed));

        public static Task<IDiscordMessage> EditMessageAsync(this IDiscordClient client, ulong channelId, ulong messageId, string content, DiscordEmbed embed = null)
            => client.EditMessageAsync(channelId, messageId, new MessageArgs(content, embed));

        public static Task<IDiscordMessage> EditMessageAsync(this IDiscordClient client, ulong messageId, ulong channelId, DiscordEmbed embed)
            => client.EditMessageAsync(channelId, messageId, new MessageArgs(string.Empty, embed));

        public static Task<IDiscordMessage> EditAsync(this IDiscordMessage message, string content, DiscordEmbed embed = null)
            => message.EditAsync(new EditMessageArgs(content, embed));

        public static Task<IDiscordMessage> EditAsync(this IDiscordMessage message, DiscordEmbed embed)
            => message.EditAsync(new EditMessageArgs(string.Empty, embed));
    }
}

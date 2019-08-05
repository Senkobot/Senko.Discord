using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senko.Discord.Gateway.Connection;
using Senko.Discord.Packets;

namespace Senko.Discord.Gateway
{
    public partial class GatewayCluster : IDiscordGateway
    {
        public Dictionary<int, IDiscordGateway> Shards { get; set; } = new Dictionary<int, IDiscordGateway>();

        /// <summary>
        /// Used to spawn specific shards only
        /// </summary>
        /// <param name="options">general gateway properties</param>
        /// <param name="shardLogger">The logger for the <see cref="GatewayShard"/></param>
        /// <param name="provider"></param>
        public GatewayCluster(IOptions<DiscordOptions> options, ILogger<GatewayShard> shardLogger, IServiceProvider provider)
        {
            for (var i = 0; i < options.Value.ShardAmount; i++)
            {
                var shardProperties = new DiscordOptions
                {
                    EnableCompression = options.Value.EnableCompression,
                    ShardAmount = options.Value.ShardAmount,
                    Token = options.Value.Token,
                    Version = options.Value.Version
                };

                Shards.Add(i, new GatewayShard(shardProperties, provider, i));
            }
        }

        public async Task SendAsync(int shardId, GatewayOpcode opCode, object payload)
        {
            if(Shards.TryGetValue(shardId, out var shard))
            {
                await shard.SendAsync(shardId, opCode, payload);
            }
        }

        public async Task RestartAsync()
        {
            foreach(var shard in Shards.Values)
            {
                await shard.RestartAsync();
            }
        }

        public async Task StartAsync()
        {
            foreach(var shard in Shards.Values)
            {
                shard.OnChannelCreate += OnChannelCreate;
                shard.OnChannelDelete += OnChannelDelete;
                shard.OnChannelUpdate += OnChannelUpdate;
                shard.OnGuildBanAdd += OnGuildBanAdd;
                shard.OnGuildBanRemove += OnGuildBanRemove;
                shard.OnGuildCreate += OnGuildCreate;
                shard.OnGuildUpdate += OnGuildUpdate;
                shard.OnGuildDelete += OnGuildDelete;
                shard.OnGuildMemberAdd += OnGuildMemberAdd;
                shard.OnGuildMemberRemove += OnGuildMemberRemove;
                shard.OnGuildMemberUpdate += OnGuildMemberUpdate;
                shard.OnGuildEmojiUpdate += OnGuildEmojiUpdate;
                shard.OnGuildRoleCreate += OnGuildRoleCreate;
                shard.OnGuildRoleUpdate += OnGuildRoleUpdate;
                shard.OnGuildRoleDelete += OnGuildRoleDelete;
                shard.OnMessageCreate += OnMessageCreate;
                shard.OnMessageUpdate += OnMessageUpdate;
                shard.OnMessageDelete += OnMessageDelete;
                shard.OnMessageDeleteBulk += OnMessageDeleteBulk;
                shard.OnPresenceUpdate += OnPresenceUpdate;
                shard.OnReady += OnReady;
                shard.OnResume += OnResume;
                shard.OnTypingStart += OnTypingStart;
                shard.OnUserUpdate += OnUserUpdate;

                await shard.StartAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task StopAsync()
        {
            foreach (var shard in Shards.Values)
            {
                shard.OnChannelCreate -= OnChannelCreate;
                shard.OnChannelDelete -= OnChannelDelete;
                shard.OnChannelUpdate -= OnChannelUpdate;
                shard.OnGuildBanAdd -= OnGuildBanAdd;
                shard.OnGuildBanRemove -= OnGuildBanRemove;
                shard.OnGuildCreate -= OnGuildCreate;
                shard.OnGuildUpdate -= OnGuildUpdate;
                shard.OnGuildDelete -= OnGuildDelete;
                shard.OnGuildMemberAdd -= OnGuildMemberAdd;
                shard.OnGuildMemberRemove -= OnGuildMemberRemove;
                shard.OnGuildMemberUpdate -= OnGuildMemberUpdate;
                shard.OnGuildEmojiUpdate -= OnGuildEmojiUpdate;
                shard.OnGuildRoleCreate -= OnGuildRoleCreate;
                shard.OnGuildRoleDelete -= OnGuildRoleDelete;
                shard.OnGuildRoleDelete -= OnGuildRoleDelete;
                shard.OnMessageCreate -= OnMessageCreate;
                shard.OnMessageUpdate -= OnMessageUpdate;
                shard.OnMessageDelete -= OnMessageDelete;
                shard.OnMessageDeleteBulk -= OnMessageDeleteBulk;
                shard.OnPresenceUpdate -= OnPresenceUpdate;
                shard.OnReady -= OnReady;
                shard.OnResume -= OnResume;
                shard.OnTypingStart -= OnTypingStart;
                shard.OnUserUpdate -= OnUserUpdate;

                await shard.StopAsync()
                    .ConfigureAwait(false);
            }
        }

        #region Events

        public event Func<DiscordChannelPacket, Task> OnChannelCreate;
        public event Func<DiscordChannelPacket, Task> OnChannelUpdate;
        public event Func<DiscordChannelPacket, Task> OnChannelDelete;
        public event Func<DiscordGuildPacket, Task> OnGuildCreate;
        public event Func<DiscordGuildPacket, Task> OnGuildUpdate;
        public event Func<DiscordGuildUnavailablePacket, Task> OnGuildDelete;
        public event Func<DiscordGuildMemberPacket, Task> OnGuildMemberAdd;
        public event Func<ulong, DiscordUserPacket, Task> OnGuildMemberRemove;
        public event Func<GuildMemberUpdateEventArgs, Task> OnGuildMemberUpdate;
        public event Func<ulong, DiscordUserPacket, Task> OnGuildBanAdd;
        public event Func<ulong, DiscordUserPacket, Task> OnGuildBanRemove;
        public event Func<ulong, DiscordEmoji[], Task> OnGuildEmojiUpdate;
        public event Func<ulong, DiscordRolePacket, Task> OnGuildRoleCreate;
        public event Func<ulong, DiscordRolePacket, Task> OnGuildRoleUpdate;
        public event Func<ulong, ulong, Task> OnGuildRoleDelete;
        public event Func<DiscordMessagePacket, Task> OnMessageCreate;
        public event Func<DiscordMessagePacket, Task> OnMessageUpdate;
        public event Func<MessageDeleteArgs, Task> OnMessageDelete;
        public event Func<MessageBulkDeleteEventArgs, Task> OnMessageDeleteBulk;
        public event Func<DiscordPresencePacket, Task> OnPresenceUpdate;
        public event Func<GatewayReadyPacket, Task> OnReady;
        public event Func<GatewayReadyPacket, Task> OnResume;
        public event Func<TypingStartEventArgs, Task> OnTypingStart;
        public event Func<DiscordUserPacket, Task> OnUserUpdate;

        #endregion
    }
}
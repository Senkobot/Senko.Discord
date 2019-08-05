using Senko.Discord.Gateway.Connection;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Senko.Discord.Packets;

namespace Senko.Discord.Gateway
{
	public class GatewayShard : IDisposable, IDiscordGateway
    {
        private readonly GatewayConnection _connection;
		private readonly CancellationTokenSource _tokenSource;
		private bool _isRunning;
        private readonly ILogger<GatewayShard> _logger;

        public GatewayShard(DiscordOptions configuration, IServiceProvider provider, int shardId = 0)
        {
            ShardId = shardId;
            _logger = provider.GetRequiredService<ILogger<GatewayShard>>();
            _tokenSource = new CancellationTokenSource();
			_connection = new GatewayConnection(configuration, provider, ShardId);
		}

        public int ShardId { get; }

        public ConnectionStatus Status => _connection.ConnectionStatus;

        public string[] TraceServers { get; private set; }

        public async Task RestartAsync()
        {
            await _connection.ReconnectAsync();
        }

		public async Task StartAsync()
		{
			if (_isRunning)
			{
				return;
			}

            _connection.Dispatch += Dispatch;

            await _connection.StartAsync();
			_isRunning = true;
		}

		public async Task StopAsync()
		{
			if (!_isRunning)
			{
				return;
			}

            _connection.Dispatch -= Dispatch;
            _tokenSource.Cancel();

			await _connection.StopAsync();

			_isRunning = false;
		}

        public Task Dispatch(GatewayMessageIdentifier identifier, ReadOnlyMemory<byte> data)
        {
            if (identifier.OpCode != GatewayOpcode.Dispatch)
            {
                return Task.CompletedTask;
            }

            switch (identifier.EventName)
            {
                case "MESSAGE_CREATE":
                    return Dispatch(OnMessageCreate, data);

                case "TYPING_START":
                    return Dispatch(OnTypingStart, data);

                case "PRESENCE_UPDATE":
                    return Dispatch(OnPresenceUpdate, data);

                case "MESSAGE_UPDATE":
                    return Dispatch(OnMessageUpdate, data);

                case "MESSAGE_DELETE":
                    return Dispatch(OnMessageDelete, data);

                case "GUILD_MEMBER_ADD":
                    return Dispatch(OnGuildMemberAdd, data);

                case "GUILD_MEMBER_UPDATE":
                    return Dispatch(OnGuildMemberUpdate, data);

                case "MESSAGE_DELETE_BULK":
                    return Dispatch(OnMessageDeleteBulk, data);

                case "GUILD_EMOJIS_UPDATE":
                    return Dispatch<GuildEmojisUpdateEventArgs>(packet => OnGuildEmojiUpdate.InvokeAsync(packet.GuildId, packet.Emojis), data);

                case "GUILD_MEMBER_REMOVE":
                    return Dispatch<GuildIdUserArgs>(packet => OnGuildMemberRemove.InvokeAsync(packet.GuildId, packet.User), data);

                case "GUILD_BAN_ADD":
                    return Dispatch<GuildIdUserArgs>(packet => OnGuildBanAdd.InvokeAsync(packet.GuildId, packet.User), data);

                case "GUILD_BAN_REMOVE":
                    return Dispatch<GuildIdUserArgs>(packet => OnGuildBanRemove.InvokeAsync(packet.GuildId, packet.User), data);

                case "GUILD_CREATE":
                    return Dispatch(OnGuildCreate, data);

                case "GUILD_ROLE_CREATE":
                    return Dispatch<RoleEventArgs>(packet => OnGuildRoleCreate.InvokeAsync(packet.GuildId, packet.Role), data);

                case "GUILD_ROLE_UPDATE":
                    return Dispatch<RoleEventArgs>(packet => OnGuildRoleUpdate.InvokeAsync(packet.GuildId, packet.Role), data);

                case "GUILD_ROLE_DELETE":
                    return Dispatch<RoleDeleteEventArgs>(packet => OnGuildRoleDelete.InvokeAsync(packet.GuildId, packet.RoleId), data);

                case "GUILD_UPDATE":
                    return Dispatch(OnGuildUpdate, data);

                case "GUILD_DELETE":
                    return Dispatch(OnGuildDelete, data);

                case "CHANNEL_CREATE":
                    return Dispatch(OnChannelCreate, data);

                case "CHANNEL_UPDATE":
                    return Dispatch(OnChannelUpdate, data);

                case "CHANNEL_DELETE":
                    return Dispatch(OnChannelDelete, data);

                case "USER_UPDATE":
                    return Dispatch(OnUserUpdate, data);

                case "READY":
                {
                    var packet = JsonHelper.Deserialize<GatewayMessage<GatewayReadyPacket>>(data.Span).Data;
                    TraceServers = packet.TraceGuilds;
                    _connection.SessionId = packet.SessionId;
                    _logger.LogInformation($"Shard {packet.CurrentShard} is ready.");
                    return OnReady.InvokeAsync(packet);
                }

                case "RESUMED":
                {
                    var packet = JsonHelper.Deserialize<GatewayMessage<GatewayReadyPacket>>(data.Span).Data;
                    TraceServers = packet.TraceGuilds;
                    return OnResume.InvokeAsync(packet);
                }

                case "PRESENCES_REPLACE":
                    return Task.CompletedTask;

                default:
                    _logger.LogWarning("Unhandled event {EventName}.", identifier.EventName);
                    return Task.CompletedTask;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Task Dispatch<T>(Func<T, Task> func, ReadOnlyMemory<byte> data)
        {
            var packet = JsonHelper.Deserialize<GatewayMessage<T>>(data.Span);
            _connection.SequenceNumber = packet.SequenceNumber;
            return func.InvokeAsync(packet.Data);
        }

		public Task SendAsync(int shardId, GatewayOpcode opCode, object payload)
		{
			if (payload == null)
			{
				throw new ArgumentNullException(nameof(payload));
			}

			return _connection.SendCommandAsync(opCode, payload, _tokenSource.Token);
		}

		public void Dispose()
		{
			_tokenSource.Dispose();
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
        #endregion Events
    }
}
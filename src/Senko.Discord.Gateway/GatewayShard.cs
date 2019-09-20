using Senko.Discord.Gateway.Connection;
using System;
using System.Runtime.CompilerServices;
using System.Text;
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
        private readonly IDiscordPacketHandler _packetHandler;

        public GatewayShard(DiscordOptions configuration, IServiceProvider provider, int shardId = 0)
        {
            ShardId = shardId;
            _packetHandler = provider.GetRequiredService<IDiscordPacketHandler>();
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
                    return _packetHandler.OnMessageCreate(Deserialize<DiscordMessagePacket>(data));

                case "TYPING_START":
                    return _packetHandler.OnTypingStart(Deserialize<TypingStartEventArgs>(data));

                case "PRESENCE_UPDATE":
                    return _packetHandler.OnPresenceUpdate(Deserialize<DiscordPresencePacket>(data));

                case "MESSAGE_UPDATE":
                    return _packetHandler.OnMessageUpdate(Deserialize<DiscordMessagePacket>(data));

                case "MESSAGE_DELETE":
                    return _packetHandler.OnMessageDelete(Deserialize<MessageDeleteArgs>(data));

                case "GUILD_MEMBER_ADD":
                    return _packetHandler.OnGuildMemberAdd(Deserialize<DiscordGuildMemberPacket>(data));

                case "GUILD_MEMBER_UPDATE":
                    return _packetHandler.OnGuildMemberUpdate(Deserialize<GuildMemberUpdateEventArgs>(data));

                case "MESSAGE_DELETE_BULK":
                    return _packetHandler.OnMessageDeleteBulk(Deserialize<MessageBulkDeleteEventArgs>(data));

                case "MESSAGE_REACTION_ADD":
                    return _packetHandler.OnMessageReactionAdd(Deserialize<MessageReactionArgs>(data));

                case "MESSAGE_REACTION_REMOVE":
                    return _packetHandler.OnMessageReactionRemove(Deserialize<MessageReactionArgs>(data));

                case "GUILD_EMOJIS_UPDATE":
                    return _packetHandler.OnGuildEmojiUpdate(Deserialize<GuildEmojisUpdateEventArgs>(data));

                case "GUILD_MEMBER_REMOVE":
                    return _packetHandler.OnGuildMemberRemove(Deserialize<GuildIdUserArgs>(data));

                case "GUILD_BAN_ADD":
                    return _packetHandler.OnGuildBanAdd(Deserialize<GuildIdUserArgs>(data));

                case "GUILD_BAN_REMOVE":
                    return _packetHandler.OnGuildBanRemove(Deserialize<GuildIdUserArgs>(data));

                case "GUILD_CREATE":
                    return _packetHandler.OnGuildCreate(Deserialize<DiscordGuildPacket>(data));

                case "GUILD_ROLE_CREATE":
                    return _packetHandler.OnGuildRoleCreate(Deserialize<RoleEventArgs>(data));

                case "GUILD_ROLE_UPDATE":
                    return _packetHandler.OnGuildRoleUpdate(Deserialize<RoleEventArgs>(data));

                case "GUILD_ROLE_DELETE":
                    return _packetHandler.OnGuildRoleDelete(Deserialize<RoleDeleteEventArgs>(data));

                case "GUILD_UPDATE":
                    return _packetHandler.OnGuildUpdate(Deserialize<DiscordGuildPacket>(data));

                case "GUILD_DELETE":
                    return _packetHandler.OnGuildDelete(Deserialize<DiscordGuildUnavailablePacket>(data));

                case "CHANNEL_CREATE":
                    return _packetHandler.OnChannelCreate(Deserialize<DiscordChannelPacket>(data));

                case "CHANNEL_UPDATE":
                    return _packetHandler.OnChannelUpdate(Deserialize<DiscordChannelPacket>(data));

                case "CHANNEL_DELETE":
                    return _packetHandler.OnChannelDelete(Deserialize<DiscordChannelPacket>(data));

                case "USER_UPDATE":
                    return _packetHandler.OnUserUpdate(Deserialize<DiscordUserPacket>(data));

                case "READY":
                {
                    var packet = JsonHelper.Deserialize<GatewayMessage<GatewayReadyPacket>>(data.Span).Data;
                    TraceServers = packet.TraceGuilds;
                    _connection.SessionId = packet.SessionId;
                    _logger.LogInformation($"Shard {packet.CurrentShard} is ready.");
                    return _packetHandler.OnReady(packet);
                }

                case "RESUMED":
                {
                    var packet = JsonHelper.Deserialize<GatewayMessage<GatewayReadyPacket>>(data.Span).Data;
                    TraceServers = packet.TraceGuilds;
                    return _packetHandler.OnResume(packet);
                }

                case "PRESENCES_REPLACE":
                    // This event can be ignored: https://github.com/discordapp/discord-api-docs/issues/683#issuecomment-420940350
                    return Task.CompletedTask;

                default:
                    _logger.LogWarning("Unhandled event {EventName}.", identifier.EventName);
                    _logger.LogTrace("Unhandled event data: {EventData}", Encoding.UTF8.GetString(data.Span));
                    return Task.CompletedTask;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T Deserialize<T>(ReadOnlyMemory<byte> data)
        {
            var packet = JsonHelper.Deserialize<GatewayMessage<T>>(data.Span);
            _connection.SequenceNumber = packet.SequenceNumber;
            return packet.Data;
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
    }
}
using System.IO.Compression;
using Senko.Discord.Gateway.Utils;
using System;
using System.Buffers;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Senko.Discord.Gateway.Ratelimiting;
using Senko.Discord.Packets;

namespace Senko.Discord.Gateway.Connection
{
    public enum ConnectionStatus
    {
        Connecting,
        Connected,
        Identifying,
        Resuming,
        Disconnecting,
        Disconnected,
        Error
    }
    
    public class GatewayConnection
    {
        public event Func<GatewayMessageIdentifier, ReadOnlyMemory<byte>, ValueTask> Dispatch;

        private ClientWebSocket _webSocketClient;
        private readonly DiscordOptions _configuration;

        private Task _runTask = null;
        private Task _heartbeatTask = null;

        private CancellationTokenSource _connectionToken;
        private SemaphoreSlim _heartbeatLock;

        private readonly SourceStream _sourceStream;
        private DeflateStream _deflateStream;
        private readonly ILogger<GatewayConnection> _logger;
        private readonly int _shardId;
        private readonly IDiscordConnectionRatelimiter _ratelimiter;

        public bool IsRunning => _runTask != null && !_connectionToken.IsCancellationRequested;

        internal int? SequenceNumber;

        /// <summary>
        /// Creates a new gateway connection
        /// </summary>
        /// <param name="configuration"></param>
		public GatewayConnection(DiscordOptions configuration, IServiceProvider provider, int shardId)
        {
            if (string.IsNullOrWhiteSpace(configuration.Token))
            {
                throw new ArgumentNullException(nameof(configuration), "Token cannot be empty.");
            }
            
            _sourceStream = new SourceStream();
            _configuration = configuration;
            _logger = provider.GetRequiredService<ILogger<GatewayConnection>>();
            _ratelimiter = provider.GetRequiredService<IDiscordConnectionRatelimiter>();
            _shardId = shardId;
        }

        public ConnectionStatus ConnectionStatus { get; private set; } = ConnectionStatus.Disconnected;

        public string SessionId { get; set; }

        /// <summary>
        ///     Start the connection.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the connection is already started.</exception>
        public async Task StartAsync()
        {
            // Check all possible statuses before reconnecting.
            if (ConnectionStatus == ConnectionStatus.Connected
                || ConnectionStatus == ConnectionStatus.Connecting
                || ConnectionStatus == ConnectionStatus.Resuming
                || ConnectionStatus == ConnectionStatus.Identifying)
            {
                throw new InvalidOperationException("Shard has already started.");
            }


            try
            {
                ConnectionStatus = ConnectionStatus.Connecting;

                _heartbeatLock = new SemaphoreSlim(0, 1);
                _connectionToken = new CancellationTokenSource();

                var connectionUri = new WebSocketUrlBuilder("wss://gateway.discord.gg/")
                    .SetCompression(_configuration.EnableCompression)
                    .SetVersion(_configuration.Version)
                    .Build();

                _webSocketClient = new ClientWebSocket();

                if (_configuration.EnableCompression)
                {
                    _deflateStream?.Dispose();
                    _deflateStream = new DeflateStream(_sourceStream, CompressionMode.Decompress, true);
                }

                await _webSocketClient.ConnectAsync(new Uri(connectionUri), _connectionToken.Token);

                if (SequenceNumber.HasValue)
                {
                    ConnectionStatus = ConnectionStatus.Resuming;

                    await SendCommandAsync(GatewayOpcode.Resume, new GatewayResumePacket
                    {
                        Sequence = SequenceNumber.Value,
                        SessionId = SessionId,
                        Token = _configuration.Token
                    }, _connectionToken.Token);
                }
                else
                {
                    ConnectionStatus = ConnectionStatus.Identifying;
                    await IdentifyAsync();
                }

                _runTask = RunAsync();
                ConnectionStatus = ConnectionStatus.Connected;
            }
            catch
            {
                await StopAsync();
                throw;
            }
        }

        /// <summary>
        ///     Stop the connection.
        /// </summary>
        public async Task StopAsync()
        {
            ConnectionStatus = ConnectionStatus.Disconnecting;

            if (_connectionToken == null || _runTask == null)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                return;
            }

            _connectionToken.Cancel();

            try
            {
                _runTask.Wait();
                _heartbeatTask.Wait();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An exception occured while stopping the run task and heartbeat task.");
            }

            if (_webSocketClient.State == WebSocketState.Open || _webSocketClient.State == WebSocketState.CloseReceived)
            {
                try
                {
                    await _webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, _connectionToken.Token);
                }
                catch (ObjectDisposedException) { /* Means the websocket has been disposed, and is ready to be reused. */ }
                catch (TaskCanceledException) { /* Ignore */ }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "An exception occured while closing the WebSocket.");
                }
            }

            _webSocketClient.Dispose();

            _connectionToken = null;
            _heartbeatTask = null;
            _runTask = null;
            _webSocketClient = null;
            ConnectionStatus = ConnectionStatus.Disconnected;
        }
        
        /// <summary>
        ///     The life-cycle of the connection.
        /// </summary>
        private async Task RunAsync()
        {
            var closedGracefully = false;

            try
            {
                _logger.LogInformation("Starting the WebSocket connection.");

                while (!_connectionToken.IsCancellationRequested && _webSocketClient.State == WebSocketState.Open)
                {
                    await ReceiveAndHandlePacketAsync();
                }

                closedGracefully = _webSocketClient.CloseStatus == WebSocketCloseStatus.NormalClosure;
            }
            catch (TaskCanceledException)
            {
                closedGracefully = true;
            }
            catch (WebSocketException w)
            {
                if (w.WebSocketErrorCode != WebSocketError.Success)
                {
                    _logger.LogError(w, "An exception occured while listening to the WebSocket connection.");
                }

                if (w.ErrorCode == 4010 || w.ErrorCode == 4011)
                {
                    _logger.LogError("Failed to restart the connection. The connection replied that the shard is invalid.");
                    return;
                }
            }
            catch (Exception e)
            {
                ConnectionStatus = ConnectionStatus.Error;
                _logger.LogError(e, "An exception occured while listening to the WebSocket connection.");
                await Task.Delay(5000);
            }

            if (!closedGracefully)
            {
                SessionId = null;
                SequenceNumber = null;
            }

            if (!_connectionToken.IsCancellationRequested)
            {
                _logger.LogInformation("The gateway connection has been closed, beginning to reconnect.");

                _ = Task.Run(() => ReconnectAsync());
            }
        }

        /// <summary>
        ///     Decompress the incoming data.
        /// </summary>
        /// <param name="data">The compressed data.</param>
        /// <param name="length">The length of the uncompressed data.</param>
        /// <returns>The new memory owner.</returns>
        private unsafe IMemoryOwner<byte> DecompressData(ReadOnlyMemory<byte> data, out int length)
        {
            var owner = MemoryPool<byte>.Shared.Rent(GatewayConstants.WebSocketReceiveSize);
            length = 0;

            fixed (byte* pBuffer = &data.Span[0])
            {
                using var stream = new UnmanagedMemoryStream(pBuffer, data.Length);

                if (data.Span[0] == 0x78)
                {
                    stream.Seek(2, SeekOrigin.Begin);
                }

                _sourceStream.BaseStream = stream;

                while (true)
                {
                    var readLength = _deflateStream.Read(owner.Memory.Span.Slice(length));

                    if (readLength == 0)
                    {
                        break;
                    }

                    length += readLength;

                    if (length == owner.Memory.Length)
                    {
                        var newMemory = MemoryPool<byte>.Shared.Rent(owner.Memory.Length * 2);
                        owner.Memory.CopyTo(newMemory.Memory);

                        owner.Dispose();
                        owner = newMemory;
                    }
                }

                _sourceStream.BaseStream = null;
            }

            return owner;
        }

        /// <summary>
        ///     Handle the incoming data.
        /// </summary>
        /// <param name="data">The uncompressed data.</param>
        private ValueTask HandleMessageAsync(ReadOnlyMemory<byte> data)
        {
            var identifier = GatewayMessageIdentifier.Read(data.Span);

            if (!identifier.OpCode.HasValue)
            {
                _logger.LogWarning("Received a message that could not be read by the gateway: {Json}", Encoding.UTF8.GetString(data.ToArray()));
                return default;
            }

            _logger.LogTrace("Received packet with OpCode {OpCode} (event: {EventName}, size: {Size})", identifier.OpCode, identifier.EventName ?? "<null>", data.Length);

            switch (identifier.OpCode)
            {
                case GatewayOpcode.Dispatch:
                    if (identifier.EventName == "READY" || identifier.EventName == "RESUMED")
                    {
                        _heartbeatLock.Release();
                    }

                    return Dispatch.InvokeAsync(identifier, data);

                case GatewayOpcode.Heartbeat:
                    return SendHeartbeatAsync();

                case GatewayOpcode.HeartbeatAcknowledge:
                    _heartbeatLock.Release();
                    return default;

                case GatewayOpcode.Hello:
                {
                    var packet = JsonHelper.Deserialize<GatewayMessage<GatewayHelloPacket>>(data.Span);
                    _heartbeatTask = HeartbeatAsync(packet.Data.HeartbeatInterval);
                    return default;
                }

                case GatewayOpcode.InvalidSession:
                {
                    var canResume = JsonHelper.Deserialize<GatewayMessage<bool>>(data.Span).Data;

                    if (!canResume)
                    {
                        SequenceNumber = null;
                    }

                    _logger.LogInformation("Received that the connection has an invalid session, beginning to reconnect.");
                    return ReconnectAsync();
                }

                default:
                    _logger.LogWarning("Unhandled OpCode {OpCode}.", identifier.OpCode);
                    return default;
            }
        }

        public async Task HeartbeatAsync(int latency)
        {
            // Will stop running heartbeat if connectionToken is cancelled.
            while (!_connectionToken.IsCancellationRequested)
            {
                try
                {
                    if (!await _heartbeatLock.WaitAsync(latency, _connectionToken.Token))
                    {
                        _ = Task.Run(() => ReconnectAsync());
                        break;
                    }

                    await SendHeartbeatAsync()
                        .ConfigureAwait(false);

                    await Task.Delay(latency, _connectionToken.Token)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An exception occured while sending the heartbeat.");
                    break;
                }
            }
        }

        public async Task IdentifyAsync()
        {
            GatewayIdentifyPacket identifyPacket = new GatewayIdentifyPacket
            {
                Compressed = _configuration.EnableCompression,
                Token = _configuration.Token,
                LargeThreshold = 250,
                Shard = new[] { _shardId, _configuration.ShardAmount },
                Intents = _configuration.Intents
            };

            var canIdentify = await _ratelimiter.CanIdentifyAsync()
                .ConfigureAwait(false);

            while (!canIdentify)
            {
                _logger.LogDebug("Could not identify yet, retrying in 5 seconds.");

                await Task.Delay(5000)
                    .ConfigureAwait(false);

                canIdentify = await _ratelimiter.CanIdentifyAsync()
                    .ConfigureAwait(false);
            }

            await SendCommandAsync(GatewayOpcode.Identify, identifyPacket, _connectionToken.Token)
                .ConfigureAwait(false);
        }

        public async ValueTask ReconnectAsync(int initialDelay = 1000, bool shouldIncrease = true)
        {
            var delay = initialDelay;
            bool connected = false;

            await StopAsync()
                .ConfigureAwait(false);

            while (!connected)
            {
                try
                {
                    await StartAsync().ConfigureAwait(false);
                    connected = true;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Reconnection failed, will retry in {Delay} seconds.", delay / 1000);
                    await Task.Delay(delay)
                        .ConfigureAwait(false);

                    SequenceNumber = null;

                    if (shouldIncrease)
                    {
                        delay += initialDelay;
                    }
                }
            }
        }

        public ValueTask SendCommandAsync<T>(GatewayOpcode opcode, T data, CancellationToken token = default)
        {
            var msg = new GatewayMessage<T>
            {
                OpCode = opcode,
                Data = data,
                EventName = null,
                SequenceNumber = null
            };

            return SendCommandAsync(msg, token);
        }

        private async ValueTask SendCommandAsync<T>(GatewayMessage<T> msg, CancellationToken token)
        {
            var json = JsonHelper.Serialize(msg);

            _logger.LogTrace("Sending packet with OpCode {OpCode} (event: {EventName}, size: {Size})", msg.OpCode, msg.EventName ?? "<null>", json.Length);

            await _webSocketClient.SendAsync(json, WebSocketMessageType.Text, true, token);
        }

        private ValueTask SendHeartbeatAsync()
        {
            var msg = new GatewayMessage<int?>
            {
                OpCode = GatewayOpcode.Heartbeat,
                Data = SequenceNumber
            };

            return SendCommandAsync(msg, _connectionToken.Token);
        }
        
        /// <summary>
        ///     Receive the message from the current <see cref="_webSocketClient"/>.
        ///     After the message has been received the <see cref="HandleMessageAsync"/> will be called.
        /// </summary>
        private async Task ReceiveAndHandlePacketAsync()
        {
            var owner = MemoryPool<byte>.Shared.Rent(GatewayConstants.WebSocketReceiveSize);
            var length = 0;

            try
            {
                ValueWebSocketReceiveResult response;

                do
                {
                    response = await _webSocketClient.ReceiveAsync(owner.Memory.Slice(length), _connectionToken.Token);
                    
                    if (_connectionToken.IsCancellationRequested)
                    {
                        return;
                    }

                    length += response.Count;

                    if (length == owner.Memory.Length)
                    {
                        var newMemory = MemoryPool<byte>.Shared.Rent(owner.Memory.Length * 2);
                        owner.Memory.CopyTo(newMemory.Memory);

                        owner.Dispose();
                        owner = newMemory;
                    }
                } while (!response.EndOfMessage);

                if (length == 0)
                {
                    owner.Dispose();
                    return;
                }
            }
            catch
            {
                owner.Dispose();
                return;
            }

            if (_configuration.EnableCompression)
            {
                var newOwner = DecompressData(owner.Memory.Slice(0, length), out length);
                owner.Dispose();
                owner = newOwner;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await HandleMessageAsync(owner.Memory.Slice(0, length));
                }
                catch (Exception e)
                {
                    var json = Encoding.UTF8.GetString(owner.Memory.Span.Slice(0, length));
                    _logger.LogError(e, "An exception occured while handling the message.");
                }
                finally
                {
                    owner.Dispose();
                }
            });
        }
    }
}
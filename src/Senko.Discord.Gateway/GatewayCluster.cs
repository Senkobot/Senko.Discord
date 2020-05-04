using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senko.Discord.Gateway.Connection;
using Senko.Discord.Packets;

namespace Senko.Discord.Gateway
{
    public partial class GatewayCluster : IDiscordGateway
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordOptions _options;
        private readonly SemaphoreSlim _initializeLock = new SemaphoreSlim(1, 1);

        private bool _initialized;
        private IDiscordGateway[] _shards;

        /// <summary>
        /// Used to spawn specific shards only
        /// </summary>
        /// <param name="options">general gateway properties</param>
        /// <param name="provider"></param>
        public GatewayCluster(IOptions<DiscordOptions> options, IServiceProvider provider)
        {
            _provider = provider;
            _options = options.Value;
        }

        public IReadOnlyCollection<IDiscordGateway> Shards => _shards;

        public async ValueTask SendAsync(int shardId, GatewayOpcode opCode, object payload)
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("The cluster is not started yet.");
            }

            if (shardId < 0 || shardId >= _shards.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(shardId));
            }

            await _shards[shardId].SendAsync(shardId, opCode, payload);
        }

        public async ValueTask RestartAsync()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("The cluster is not started yet.");
            }

            foreach (var shard in _shards)
            {
                await shard.RestartAsync();
            }
        }

        public async ValueTask StartAsync()
        {
            await _initializeLock.WaitAsync();

            try
            {
                // Initialize the shards.
                if (_shards == null)
                {
                    _shards = new IDiscordGateway[_options.ShardAmount];

                    for (var i = 0; i < _options.ShardAmount; i++)
                    {
                        var shardProperties = new DiscordOptions
                        {
                            EnableCompression = _options.EnableCompression,
                            ShardAmount = _options.ShardAmount,
                            Token = _options.Token,
                            Version = _options.Version,
                            Intents = _options.Intents
                        };

                        _shards[i] = new GatewayShard(shardProperties, _provider, i);
                    }
                }

                // Start the shards.
                _initialized = true;

                await Task.WhenAll(_shards.Select(s => s.StartAsync().AsTask()));
            }
            finally
            {
                _initializeLock.Release();
            }

        }

        public ValueTask StopAsync()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("The cluster is not started yet.");
            }

            return new ValueTask(Task.WhenAll(_shards.Select(s => s.StopAsync().AsTask())));
        }
    }
}
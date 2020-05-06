﻿using Senko.Discord.Packets;

namespace Senko.Discord
{
    public class DiscordAttachment : IDiscordAttachment
    {
        private readonly DiscordAttachmentPacket _packet;

        public DiscordAttachment(DiscordAttachmentPacket packet)
        {
            _packet = packet;
        }

        /// <inheritdoc/>
        public string FileName => _packet.FileName;

        /// <inheritdoc/>
        public int? Height => _packet.Height;

        /// <inheritdoc/>
        public ulong Id => _packet.Id;

        /// <inheritdoc/>
        public string ProxyUrl => _packet.ProxyUrl;

        /// <inheritdoc/>
        public int Size => _packet.Size;

        /// <inheritdoc/>
        public string Url => _packet.Url;

        /// <inheritdoc/>
        public int? Width => _packet.Width;
    }
}

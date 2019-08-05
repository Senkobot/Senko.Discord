using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Senko.Discord.Packets
{
    [DataContract]
    public class DiscordGuildPacket : ISnowflake
    {
        [DataMember(Name = "id", Order = 1)]
        public ulong Id { get; set; }

        [DataMember(Name = "name", Order = 2)]
        public string Name { get; set; }

        [DataMember(Name = "icon", Order = 3)]
        public string Icon { get; set; }

        [DataMember(Name = "splash", Order = 4)]
        public string Splash { get; set; }

        [DataMember(Name = "owner_id", Order = 5)]
        public ulong OwnerId { get; set; }

        [DataMember(Name = "region", Order = 6)]
        public string Region { get; set; }

        [DataMember(Name = "afk_channel_id", Order = 7)]
        public ulong? AfkChannelId { get; set; }

        [DataMember(Name = "afk_timeout", Order = 8)]
        public int? AfkTimeout { get; set; }

        [DataMember(Name = "embed_enabled", Order = 9)]
        public bool EmbedEnabled { get; set; }

        [DataMember(Name = "embed_channel_id", Order = 10)]
        public ulong? EmbedChannelId { get; set; }

        [DataMember(Name = "verification_level", Order = 11)]
        public int VerificationLevel { get; set; }

        [DataMember(Name = "default_message_notifications", Order = 12)]
        public int DefaultMessageNotifications { get; set; }

        [DataMember(Name = "explicit_content_filter", Order = 13)]
        public int ExplicitContentFilter { get; set; }

        [DataMember(Name = "roles", Order = 14)]
        public List<DiscordRolePacket> Roles { get; set; } = new List<DiscordRolePacket>();

        [DataMember(Name = "emojis", Order = 15)]
        public DiscordEmoji[] Emojis { get; set; }

        [DataMember(Name = "features", Order = 16)]
        public List<string> Features { get; set; }

        [DataMember(Name = "mfa_level", Order = 17)]
        public int MFALevel { get; set; }

        [DataMember(Name = "application_id", Order = 18)]
        public ulong? ApplicationId { get; set; }

        [DataMember(Name = "widget_enabled", Order = 19)]
        public bool? WidgetEnabled { get; set; }

        [DataMember(Name = "widget_channel_id", Order = 20)]
        public ulong? WidgetChannelId { get; set; }

        [DataMember(Name = "system_channel_id", Order = 21)]
        public ulong? SystemChannelId { get; set; }

        [DataMember(Name = "joined_at", Order = 22)]
        public DateTimeOffset CreatedAt { get; set; }

        [DataMember(Name = "large", Order = 23)]
        public bool? IsLarge { get; set; }

        [DataMember(Name = "unavailable", Order = 24)]
        public bool? Unavailable { get; set; }

        [DataMember(Name = "member_count", Order = 25)]
        public int? MemberCount { get; set; }

        [DataMember(Name = "owner", Order = 26)]
        public bool? IsOwner { get; set; }

        [DataMember(Name = "permissions", Order = 27)]
        public int? Permissions { get; set; }

        [DataMember(Name = "premium_tier", Order = 28)]
        public int? PremiumTier { get; set; }

        [DataMember(Name = "premium_subscription_count", Order = 29)]
        public int? PremiumSubscriberCount { get; set; }

        [DataMember(Name = "members", Order = 30)]
        public List<DiscordGuildMemberPacket> Members { get; set; } = new List<DiscordGuildMemberPacket>();

        [DataMember(Name = "channels", Order = 31)]
        public List<DiscordChannelPacket> Channels { get; set; } = new List<DiscordChannelPacket>();

        [DataMember(Name = "presences", Order = 32)]
        public List<DiscordPresencePacket> Presences { get; set; } = new List<DiscordPresencePacket>();

        [DataMember(Name = "voice_states", Order = 33)]
        public List<DiscordVoiceStatePacket> VoiceStates { get; set; }

        public void OverwriteContext(DiscordGuildPacket guild)
        {
            Name = guild.Name;
            Icon = guild.Icon;
            Splash = guild.Splash;
            OwnerId = guild.OwnerId;
            Region = guild.Region;
            AfkChannelId = guild.AfkChannelId;
            AfkTimeout = guild.AfkTimeout;
            Permissions = guild.Permissions;
            EmbedEnabled = guild.EmbedEnabled;
            EmbedChannelId = guild.EmbedChannelId;
            VerificationLevel = guild.VerificationLevel;
            DefaultMessageNotifications = guild.DefaultMessageNotifications;
            ExplicitContentFilter = guild.ExplicitContentFilter;
            MFALevel = guild.MFALevel;
            ApplicationId = guild.ApplicationId;
            WidgetEnabled = guild.WidgetEnabled;
            WidgetChannelId = guild.WidgetChannelId;
            SystemChannelId = guild.SystemChannelId;
            PremiumTier = guild.PremiumTier;
            PremiumSubscriberCount = guild.PremiumSubscriberCount;
        }
    }
}
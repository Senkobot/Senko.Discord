using System;
using Senko.Discord.Helpers;
using Senko.Discord.Internal;
using Senko.Discord.Packets;
using Xunit;

namespace Senko.Discord.Tests
{
    public class StringHelperTest
    {
        [Fact]
        public void TestNormal()
        {
            Assert.Null(StringHelper.Normalize(null));
            Assert.Equal("example", StringHelper.Normalize("𝔢𝔵𝔞𝔪𝔭𝔩𝔢"));
            Assert.Equal("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz0123456789", StringHelper.Normalize("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"));
        }
        
        [Fact]
        public void TestUser()
        {
            var specialUser = new DiscordUser(new DiscordUserPacket
            {
                Username = "𝔢𝔵𝔞𝔪𝔭𝔩𝔢"
            }, null);
            
            Assert.True(specialUser.Matches("𝔢𝔵𝔞𝔪𝔭𝔩𝔢"));
            Assert.True(specialUser.Matches("example"));
        }
        
        [Fact]
        public void TestGuildUser()
        {
            var specialUser = new DiscordGuildUser(new DiscordGuildMemberPacket
            {
                User = new DiscordUserPacket
                {
                    Username = "𝔢𝔵𝔞𝔪𝔭𝔩𝔢"
                }
            }, null);
            
            Assert.True(specialUser.Matches("𝔢𝔵𝔞𝔪𝔭𝔩𝔢"));
            Assert.True(specialUser.Matches("example"));
        }
        
        [Fact]
        public void TestGuildUserWithNickname()
        {
            var specialUser = new DiscordGuildUser(new DiscordGuildMemberPacket
            {
                Nickname = "𝔢𝔵𝔞𝔪𝔭𝔩𝔢",
                User = new DiscordUserPacket
                {
                    Username = "user"
                }
            }, null);
            
            Assert.True(specialUser.Matches("𝔢𝔵𝔞𝔪𝔭𝔩𝔢"));
            Assert.True(specialUser.Matches("example"));
        }
    }
}
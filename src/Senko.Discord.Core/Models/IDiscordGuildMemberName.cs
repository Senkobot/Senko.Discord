namespace Senko.Discord
{
    public interface IDiscordGuildMemberName : ISnowflake
    {
        string Username { get; }
        
        string NormalizedUsername { get; }
        
        string Nickname { get; }
        
        string NormalizedNickname { get; }
    }
}
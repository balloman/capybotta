using Discord.WebSocket;

namespace Capybotta.Bot.Extensions;

public static class SocketGuildExtensions
{
    public static async Task DisconnectFromVoice(this SocketGuild guild)
    {
        var vc = guild.VoiceChannels.FirstOrDefault(channel => channel.Users.Contains(guild.CurrentUser));
        if (vc is not null)
        {
            await vc.DisconnectAsync();
        }
    }
}

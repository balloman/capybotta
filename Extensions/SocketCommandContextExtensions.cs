using Discord;
using Discord.Commands;

namespace Capybotta.Bot.Extensions;

public static class SocketCommandContextExtensions
{
    
    /// <summary>
    /// Attempts to get the voice channel of the user that executed the command
    /// </summary>
    /// <returns>The audio channel if it exists, otherwise null</returns>
    public static IAudioChannel? GetVoiceChannel(this SocketCommandContext context)
    {
        var channels = context.Guild.VoiceChannels;
        return channels.FirstOrDefault(voiceChannel => voiceChannel.Users.Contains(context.User));
    }
}

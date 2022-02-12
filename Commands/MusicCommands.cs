using Discord.Commands;
using JetBrains.Annotations;

namespace Capybotta.Bot.Commands;

[PublicAPI]
public class MusicCommands : ModuleBase<SocketCommandContext>
{
    [Command("play", RunMode = RunMode.Async), Summary("Attempts to play a song.")]
    public async Task PlayCommand(string url)
    {
        
    }
}

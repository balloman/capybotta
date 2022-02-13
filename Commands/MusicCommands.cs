using Capybotta.Bot.Services;
using Discord.Commands;
using JetBrains.Annotations;

namespace Capybotta.Bot.Commands;

[PublicAPI]
public class MusicCommands : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<MusicCommands> _logger;
    private readonly MusicPlayerService _musicService;

    public MusicCommands(ILogger<MusicCommands> logger, MusicPlayerService musicService)
    {
        _logger = logger;
        _musicService = musicService;
    }
    
    [Command("play", RunMode = RunMode.Async), Summary("Attempts to play a song.")]
    public async Task PlayCommand(string url)
    {
        _logger.LogInformation("{User} requested to play {Link} in {Channel}", Context.User.Username, 
            url, Context.Channel.Name);
        await _musicService.Play(url, Context);
    }
}

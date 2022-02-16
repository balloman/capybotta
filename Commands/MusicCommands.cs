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

    [Command("stop", RunMode = RunMode.Async), Summary("Attempts to stop the current song.")]
    public async Task StopCommand()
    {
        _logger.LogInformation("{User} has requested to stop the music in {Guild}", Context.User.Username, 
            Context.Guild.Name);
        await _musicService.Stop(Context);
    }
    //
    // [Command("seek", RunMode = RunMode.Async), Summary("Attempts to seek to a specific time in the current song.")]
    // public async Task SeekCommand(double time)
    // {
    //     _logger.LogInformation("{User} has requested to seek the music to {Duration} in {Guild}", Context.User.Username, 
    //         time, Context.Guild.Name);
    //     await _musicService.Skip(Context, (int)time);
    // }
}

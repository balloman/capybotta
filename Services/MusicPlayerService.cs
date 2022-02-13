using System.Diagnostics;
using Capybotta.Bot.Extensions;
using Discord.Audio;
using Discord.Commands;

namespace Capybotta.Bot.Services;

public class MusicPlayerService
{
    private readonly ILogger<MusicPlayerService> _logger;
    private readonly YoutubeService _youtubeService;

    public MusicPlayerService(ILogger<MusicPlayerService> logger, YoutubeService youtubeService)
    {
        _logger = logger;
        _youtubeService = youtubeService;
    }

    public async Task Play(string url, SocketCommandContext context)
    {
        Stream stream;
        if (url.ToLower().Contains("you"))
        {
            var youtubeStream = await _youtubeService.GetStream(url);
            if (youtubeStream is null)
            {
                await context.
                    Channel.SendMessageAsync("There was an error trying to find the stream for this video...");
                return;
            }
            stream = youtubeStream;
        } else
        {
            await context.Channel.SendMessageAsync("This is not a youtube video...");
            return;
        }

        var vc = context.GetVoiceChannel();
        if (vc is null)
        {
            await context.Channel.SendMessageAsync("You need to be in a voice channel to use this command...");
            return;
        }

        using var audioClient = await vc.ConnectAsync();
        using var ffmpeg = CreateFfmpegProcess();
        _logger.LogInformation("Starting playback of {Url}", url);
        var playbackTask = ffmpeg.StandardOutput.BaseStream.CopyToAsync(audioClient.CreatePCMStream(AudioApplication.Music));
        await stream.CopyToAsync(ffmpeg.StandardInput.BaseStream);
        await playbackTask;
    }

    private static Process CreateFfmpegProcess()
    {
        var ffmpegProcess = Process.Start(new ProcessStartInfo
        {
            FileName = $"ffmpeg",
            Arguments = $"-hide_banner -loglevel panic -i - -ac 2 -f s16le -ar 48000 -",
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        });
        if (ffmpegProcess == null)
        {
            throw new Exception("Failed to start ffmpeg process");
        }

        return ffmpegProcess;
    }
}

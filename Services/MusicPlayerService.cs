using System.Diagnostics;
using Capybotta.Bot.Extensions;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;

namespace Capybotta.Bot.Services;

public class MusicPlayerService
{
    private readonly ILogger<MusicPlayerService> _logger;
    private readonly YoutubeService _youtubeService;

    private readonly Dictionary<ulong, PlaybackRecord> _voiceStreams = new();

    public MusicPlayerService(ILogger<MusicPlayerService> logger, YoutubeService youtubeService)
    {
        _logger = logger;
        _youtubeService = youtubeService;
    }

    public async Task Play(string url, SocketCommandContext context)
    {
        var guildId = context.Guild.Id;
        if (!_voiceStreams.ContainsKey(guildId)) _voiceStreams.Add(guildId, new PlaybackRecord());
        var currentRecord = _voiceStreams[guildId];
        if (url.ToLower().Contains("you"))
        {
            var youtubeStream = await _youtubeService.GetStream(url);
            if (youtubeStream is null)
            {
                await context.
                    Channel.SendMessageAsync("There was an error trying to find the stream for this video...");
                return;
            }
            currentRecord.Stream = youtubeStream.Stream;
            currentRecord.Duration = youtubeStream.Duration;
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
        currentRecord.FfmpegProcess?.Kill();
        currentRecord.FfmpegProcess = CreateFfmpegProcess();
        var ffmpeg = currentRecord.FfmpegProcess;
        _logger.LogInformation("Starting playback of {Url}", url);
        _ = context.Channel.SendMessageAsync($"OKAY I PULL UP. HOP OUT AT {vc.Name}!");
        var playbackTask = ffmpeg.StandardOutput.BaseStream.CopyToAsync(audioClient.CreatePCMStream(AudioApplication.Music));
        await currentRecord.Stream.CopyToAsync(ffmpeg.StandardInput.BaseStream);
        await playbackTask;
        currentRecord.FfmpegProcess.Dispose();
    }

    public async Task Stop(SocketCommandContext context)
    {
        var guildId = context.Guild.Id;
        if (!_voiceStreams.ContainsKey(guildId)) await context.Channel.SendMessageAsync("There is no music playing...");
        var currentRecord = _voiceStreams[guildId];
        _ = context.Channel.SendMessageAsync("Stopping playback...");
        await currentRecord.Stream!.DisposeAsync();
        currentRecord.FfmpegProcess?.Kill();
        currentRecord.FfmpegProcess?.Dispose();
        await context.Guild.DisconnectFromVoice();
    }

    public async Task Skip(SocketCommandContext context, int seconds)
    {
        var guildId = context.Guild.Id;
        if (!_voiceStreams.ContainsKey(guildId)) await context.Channel.SendMessageAsync("There is no music playing...");
        var currentRecord = _voiceStreams[guildId];
        if (currentRecord.Stream is null)
        {
            await context.Channel.SendMessageAsync("There is no music playing...");
            return;
        }
        var skippedDurationFactor = seconds / currentRecord.Duration;
        if (skippedDurationFactor > 1)
        {
            await context.Channel.SendMessageAsync("You cannot skip more than the duration of the song...");
            return;
        }

        _ = context.Channel.SendMessageAsync($"Skipping to {seconds} seconds...");
        var skippedBytes = skippedDurationFactor * currentRecord.Stream.Length;
        currentRecord.Stream.Seek(skippedBytes, SeekOrigin.Begin);
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

    private record PlaybackRecord
    {
        public Stream? Stream { get; set; }
        public Process? FfmpegProcess { get; set; }
        public int Duration { get; set; }
    }
}

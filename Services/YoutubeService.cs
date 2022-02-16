using Capybotta.Bot.Services.Interfaces;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Capybotta.Bot.Services;

public class YoutubeService : ISoundSourceService
{
    private readonly ILogger<YoutubeService> _logger;
    private readonly YoutubeClient _ytClient;

    public YoutubeService(ILogger<YoutubeService> logger)
    {
        _logger = logger;
        _ytClient = new YoutubeClient();
    }

    public async Task<ISoundSourceService.VideoData?> GetStream(string url)
    {
        var streamInfoOptional = await GetAudioStreamInfo(url);
        if (streamInfoOptional?.streamInfo != null)
        {
            var duration = streamInfoOptional.Value.Duration;
            var stream = await _ytClient.Videos.Streams.GetAsync(streamInfoOptional.Value.streamInfo!);
            var videoData = new ISoundSourceService.VideoData(stream, duration);
            return videoData;
        }
        _logger.LogError("Failed to get audio stream for url {Url}", url);
        return null;

    }

    private async Task<(AudioOnlyStreamInfo? streamInfo, int Duration)?> GetAudioStreamInfo(string id)
    {
        _logger.LogInformation("Getting audio stream for {Id}", id);
        var videoId = VideoId.TryParse(id);
        if (videoId == null)
        {
            _logger.LogError("Failed to parse video id {Id}", id);
            return null;
        }

        var duration = (await _ytClient.Videos.GetAsync(videoId.Value)).Duration!.Value.TotalSeconds;
        var streamManifest = await _ytClient.Videos.Streams.GetManifestAsync(videoId.Value);
        var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        return (audioStreamInfo as AudioOnlyStreamInfo, (int) duration);
    }
}

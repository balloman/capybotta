using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Capybotta.Bot.Services;

public class YoutubeService
{
    private readonly ILogger<YoutubeService> _logger;
    private readonly YoutubeClient _ytClient;

    public YoutubeService(ILogger<YoutubeService> logger)
    {
        _logger = logger;
        _ytClient = new YoutubeClient();
    }

    public async Task<AudioOnlyStreamInfo?> GetAudioStream(string id)
    {
        var videoId = VideoId.Parse(id);
        _logger.LogInformation("Getting audio stream for {Id}", id);
        var streamManifest = await _ytClient.Videos.Streams.GetManifestAsync(videoId);
        var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        return audioStreamInfo as AudioOnlyStreamInfo;
    }
}

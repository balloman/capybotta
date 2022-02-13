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

    public async Task<Stream?> GetStream(string url)
    {
        var streamInfo = await GetAudioStreamInfo(url);
        if (streamInfo != null) return await _ytClient.Videos.Streams.GetAsync(streamInfo);
        _logger.LogError("Failed to get audio stream for url {Url}", url);
        return null;

    }

    private async Task<AudioOnlyStreamInfo?> GetAudioStreamInfo(string id)
    {
        _logger.LogInformation("Getting audio stream for {Id}", id);
        var videoId = VideoId.TryParse(id);
        if (videoId == null)
        {
            _logger.LogError("Failed to parse video id {Id}", id);
            return null;
        }
        var streamManifest = await _ytClient.Videos.Streams.GetManifestAsync(videoId.Value);
        var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        return audioStreamInfo as AudioOnlyStreamInfo;
    }
}

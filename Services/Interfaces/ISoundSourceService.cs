namespace Capybotta.Bot.Services.Interfaces;

public interface ISoundSourceService
{
    public Task<VideoData?> GetStream(string url);

    public record VideoData(Stream Stream, int Duration);
}

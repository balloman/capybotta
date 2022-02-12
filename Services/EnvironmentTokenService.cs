using Capybotta.Bot.Services.Interfaces;

namespace Capybotta.Bot.Services;

public class EnvironmentTokenService : ITokenService
{
    private readonly IConfiguration _configuration;


    public EnvironmentTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    /// <inheritdoc />
    public string Token
    {
        get
        {
            var settingsVariable = _configuration["Token"];
            if (!string.IsNullOrWhiteSpace(settingsVariable))
            {
                return settingsVariable;
            }
            return Environment.GetEnvironmentVariable("BOT_TOKEN") ?? throw new Exception("Token Not Found...");
        }
    }
}

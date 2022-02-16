using Capybotta.Bot.Handlers;
using Capybotta.Bot.Services.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Capybotta.Bot.Services;

public class DiscordService : BackgroundService 
{
    private readonly ILogger<DiscordService> _logger;
    private readonly ITokenService _tokenService;
    private readonly CommandHandler _commandHandler;
    private readonly DiscordSocketClient _client;

    public DiscordService(ILogger<DiscordService> logger, ITokenService tokenService, IServiceProvider services)
    {
        _logger = logger;
        _tokenService = tokenService;
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All,
            AlwaysDownloadUsers = true,
        });
        var commandService = new CommandService();
        _commandHandler = new CommandHandler(_client, commandService, services);
        _client.Log += message => Task.Run(() => logger.LogInformation("{DiscordLog}", message));
        _client.Ready += OnReady;
    }
    
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _client.LoginAsync(TokenType.Bot, _tokenService.Token);
        await _client.StartAsync();
        await _commandHandler.InstallCommandsAsync();
        await Task.Delay(-1, stoppingToken);
    }

    private async Task OnReady()
    {
        _logger.LogInformation("Connected to Discord");
        var guilds = _client.Guilds;
        foreach (var guild in guilds)
        {
            _logger.LogInformation("Joined guild {GuildName}", guild.Name);
            // await guild.SystemChannel.SendMessageAsync("OKAY I PULL UP!!!!");
        }
    }
}

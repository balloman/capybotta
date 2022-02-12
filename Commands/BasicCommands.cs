using Discord.Commands;
using JetBrains.Annotations;

namespace Capybotta.Bot.Commands;

[PublicAPI]
public class BasicCommands : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<BasicCommands> _logger;

    public BasicCommands(ILogger<BasicCommands> logger)
    {
        _logger = logger;
    }
    
    [Command("ping", RunMode = RunMode.Async), Summary("Attempts to ping the bot.")]
    public async Task PingCommand()
    {
        _logger.LogInformation("Ping command called from {Server}", Context.Guild.Name);
        var channel = Context.Channel;
        await channel.SendMessageAsync("Pong!");
    }
}

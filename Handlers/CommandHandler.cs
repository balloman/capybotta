using System.Reflection;
using Capybotta.Bot.Services;
using Discord.Commands;
using Discord.WebSocket;

namespace Capybotta.Bot.Handlers;

public class CommandHandler
{
    private const string COMMAND_PREFIX = "!capy ";
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commandService;
    private readonly IServiceProvider _serviceProvider;

    public CommandHandler(DiscordSocketClient socketClient, CommandService commandService, IServiceProvider serviceProvider)
    {
        _client = socketClient;
        _commandService = commandService;
        _serviceProvider = serviceProvider;
    }

    public async Task InstallCommandsAsync()
    {
        _client.MessageReceived += ClientOnMessageReceived;
        await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
    }

    private Task ClientOnMessageReceived(SocketMessage arg)
    {
        if (arg is not SocketUserMessage message) return Task.CompletedTask;
        var argPos = 0;
        if (!message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.Author.IsBot) return Task.CompletedTask;
        var context = new SocketCommandContext(_client, message);
        return _commandService.ExecuteAsync(context, argPos, _serviceProvider);
    }
}

using Discord;
using Discord.Commands;
using JetBrains.Annotations;

namespace Capybotta.Bot.Commands;

[PublicAPI]
public class TargetedCommands : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<TargetedCommands> _logger;

    public TargetedCommands(ILogger<TargetedCommands> logger)
    {
        _logger = logger;
    }
    
    [Command("avatar", RunMode = RunMode.Async), Summary("Attempts to get the user's avatar")]
    public async Task AvatarCommand(IUser user)
    {
        _logger.LogInformation("Avatar command called");
        var mentionedUsers = Context.Message.MentionedUsers;
        if (mentionedUsers.Count <= 1)
        {
            await ReplyAsync("You need to mention a user to get their avatar");
            return;
        }
        _logger.LogInformation("Grabbing avatar for user {User}", user.Username);
        var avatarUrl = user.GetAvatarUrl()!;
        await ReplyAsync("Profile picture of " + user.Username + ": " + avatarUrl.Replace("size=128", "size=2048"));
    }

    [Command("gpdhere", RunMode = RunMode.Async), Summary("Attempts to send a special package to the user")]
    public async Task GpHereCommand()
    {
        _logger.LogInformation("Sending to {Guild}", Context.Guild.Name);
        await Context.Message.DeleteAsync();
        foreach (var textChannel in Context.Guild.TextChannels)
        {
            try
            {
                await textChannel.SendMessageAsync(ConstantMessages.GPD);
            } catch (Exception e)
            {
                _logger.LogInformation("Don't have access to channel {Channel}", textChannel.Name);
            }
        }
    }

    [Command("gpd", RunMode = RunMode.Async), Summary("Attempts to send a special package to the user")]
    public async Task GpCommand(IUser user)
    {
        _logger.LogInformation("Sending stuff to {User}", user);
        await Context.Message.DeleteAsync();
        await ReplyAsync($"Sending to {user.Username}. Thanks for using Gaylibaba!");
        await user.SendMessageAsync(ConstantMessages.GPD, true);
    }
}

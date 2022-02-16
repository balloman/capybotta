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
        await ReplyAsync("Profile picture of " + user.Username + ": " + avatarUrl);
    }
}

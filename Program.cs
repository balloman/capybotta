using Capybotta.Bot.Handlers;
using Capybotta.Bot.Services;
using Capybotta.Bot.Services.Interfaces;

var host = Host.CreateDefaultBuilder(args).ConfigureServices(services =>
    {
        services.AddSingleton<YoutubeService>();
        services.AddSingleton<ITokenService, EnvironmentTokenService>();
        services.AddHostedService<DiscordService>();
    })
    .Build();

await host.RunAsync();

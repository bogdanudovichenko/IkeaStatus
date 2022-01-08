using IkeaBot;
using IkeaBot.Models.Options;
using IkeaBot.Repositories;
using IkeaBot.ApiClients;
using IkeaBot.Services;

Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services.Configure<TelegramOptions>(configuration.GetSection("Telegram"));

        services.AddSingleton<ProductsRepository>();        
        services.AddSingleton<TelegramBotApiClient>();
        services.AddSingleton<IkeaStatusService>();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

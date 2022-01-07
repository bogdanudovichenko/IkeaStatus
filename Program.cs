using IkeaBot;

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

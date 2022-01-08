Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

var configuration = new ConfigurationBuilder()
    .AddCommandLine(args)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.secrets.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices(services =>
    {
        services.Configure<TelegramOptions>(configuration.GetSection("Telegram"));

        services.AddSingleton<ProductsRepository>();
        services.AddSingleton<TelegramBotApiClient>();
        services.AddSingleton<IkeaStatusService>();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

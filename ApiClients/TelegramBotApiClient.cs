using System.Text;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using IkeaBot.Models;
using IkeaBot.Models.Options;

namespace IkeaBot.ApiClients;

public class TelegramBotApiClient
{
    private readonly TelegramOptions _options;
    private readonly TelegramBotClient _telegramBotClient;

    public TelegramBotApiClient(IOptions<TelegramOptions> options)
    {
        _options = options.Value;
        _telegramBotClient = new TelegramBotClient(_options.ApiKey);
    }

    public async Task SendMessageAsync(IList<Product> products)
    {
        var msgBuilder = new StringBuilder();

        foreach (var product in products)
        {
            msgBuilder.AppendLine("------");
            msgBuilder.AppendLine(product.Url);
            msgBuilder.AppendLine("------");
        }

        await _telegramBotClient.SendTextMessageAsync(_options.ChatName, msgBuilder.ToString());
    }
}
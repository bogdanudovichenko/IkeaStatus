public record TelegramOptions
{
    public string ApiKey { get; init; } = null!;
    public string ChatName { get; init; } = null!;
}
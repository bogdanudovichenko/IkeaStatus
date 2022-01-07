using Microsoft.Playwright;

public static class BrowserExtensions
{
    public static async Task<IBrowser> LauncChromeAsync(this IPlaywright playwright)
    {
        var options = new BrowserTypeLaunchOptions
        {
            Headless = true
        };

        try
        {
            return await playwright.Chromium.LaunchAsync(options);
        }
        catch
        {
            Microsoft.Playwright.Program.Main(new[] { "install" });
            return await playwright.Chromium.LaunchAsync(options);
        }
    }
}
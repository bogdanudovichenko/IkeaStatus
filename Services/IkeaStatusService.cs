using Microsoft.Playwright;

namespace IkeaBot.Services;

public class IkeaStatusService
{
    private readonly float Timeout = (float)TimeSpan.FromSeconds(20).TotalMilliseconds;
    private readonly ProductsRepository _productsRepository;
    private readonly TelegramBotApiClient _telegramBotApiClient;

    public IkeaStatusService(
        ProductsRepository productsRepository,
        TelegramBotApiClient TelegramBotApiClient)
    {
        _productsRepository = productsRepository;
        _telegramBotApiClient = TelegramBotApiClient;
    }

    public async Task CheckProductsStatusAsync()
    {
        var products = await _productsRepository.GetProductsAsync();

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.LauncChromeAsync();
        var page = await browser.NewPageAsync();

        foreach (var product in products)
        {
            await page.GotoAsync(product.Url);

            var available = await IsProductAvailableAsync(page, product);
            if (!available)
            {
                return;
            }
        }

        await _telegramBotApiClient.SendMessageAsync(products);
    }

    private async Task<bool> IsProductAvailableAsync(IPage page, Product product)
    {
        if (!await TryAddProductToShoppingCartAsync(page))
        {
            return false;
        }        

        await GoToShoppingCartAsync(page);
        await SetProductQuantityAsync(page, product.Quantity);
        return await TryCheckoutProductAsync(page);
    }

    private async Task<bool> TryAddProductToShoppingCartAsync(IPage page)
    {
        var content = await page.ContentAsync();
        if (content.Contains("Немає в наявності"))
        {
            return false;
        }

        const string buyBtnSelector = ".range-revamp-buy-module  .range-revamp-btn--emphasised";
        var buyBtn = await page.QuerySelectorAsync(buyBtnSelector);
        if (buyBtn == null)
        {
            throw new Exception($"Can't get buy button by selector: {buyBtnSelector}");
        }

        await buyBtn.ClickAsync();
        await page.WaitForTimeoutAsync(Timeout);
        return true;
    }

    private async Task GoToShoppingCartAsync(IPage page)
    {
        await page.GotoAsync("https://secure.ikea.com/ua/uk/mcommerce/shoppingcart");
        await page.WaitForTimeoutAsync(Timeout);
    }

    private async Task SetProductQuantityAsync(IPage page, int quantity)
    {
        const string quantityProductSelectSelector = "select.quantityProduct";
        var quantityProductSelect = await page.QuerySelectorAsync(quantityProductSelectSelector);
        if (quantityProductSelect == null)
        {
            throw new Exception($"Can't get quantity product select by selector: {quantityProductSelectSelector}");
        }

        await page.SelectOptionAsync(quantityProductSelectSelector, quantity.ToString());
        await page.WaitForTimeoutAsync(Timeout);
    }

    private async Task<bool> TryCheckoutProductAsync(IPage page)
    {
        const string checkoutButtonTopSelector = "#checkoutButtonTop";
        var checkoutButtonTop = await page.QuerySelectorAsync(checkoutButtonTopSelector);
        if (checkoutButtonTop == null)
        {
            throw new Exception($"Can'get checkoutButtonTop by selector: {checkoutButtonTopSelector}");
        }

        await checkoutButtonTop.ClickAsync();
        await page.WaitForTimeoutAsync(Timeout);

        var errorMassageBlock = await page.QuerySelectorAsync(".message-error");
        return errorMassageBlock == null;
    }
}
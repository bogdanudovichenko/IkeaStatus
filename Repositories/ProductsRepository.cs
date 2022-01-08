namespace IkeaBot.Repositories;

public class ProductsRepository
{
    public async Task<IList<Product>> GetProductsAsync()
    {
        var wishesJson = await File.ReadAllTextAsync("products.json");

        return JsonSerializer.Deserialize<IList<Product>>(wishesJson, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }) ?? new List<Product>();
    }
}
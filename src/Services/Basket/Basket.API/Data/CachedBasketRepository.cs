using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data
{
    public class CachedBasketRepository (IBasketRepository repository, IDistributedCache cache) : IBasketRepository
    {
    
        public async Task<bool> DeleteBasketAsync(string username, CancellationToken cancellationToken = default)
        {
           
             await repository.DeleteBasketAsync(username,cancellationToken);
            await cache.RemoveAsync(username, cancellationToken);
            return true;
        }

        public async Task<ShoppingCart> GetBasketAsync(string username, CancellationToken cancellationToken = default)
        {
            var cachedBasket = await cache.GetStringAsync( username, cancellationToken);

            if (!string.IsNullOrEmpty(cachedBasket))
            {
                var basket = JsonSerializer.Deserialize<ShoppingCart>(
                    cachedBasket);
                return basket!;
            }

            var basketFromDatabase =
                await repository.GetBasketAsync(username, cancellationToken);

            await cache.SetStringAsync(
                username,
                JsonSerializer.Serialize(basketFromDatabase),
                cancellationToken);

            return basketFromDatabase;
        }

        public async Task<ShoppingCart> StoreBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default)
        {
            await repository.StoreBasketAsync(basket, cancellationToken);

            await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket));
                
            return basket;
        }
    }
}

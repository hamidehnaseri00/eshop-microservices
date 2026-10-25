using Basket.API.Exception;
using Marten;

namespace Basket.API.Data
{
    public class BasketRepository(IDocumentSession session) : IBasketRepository
    {
        public async Task<bool> DeleteBasketAsync(string username, CancellationToken cancellationToken = default)
        {
            session.Delete<ShoppingCart>(username);

            await session.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<ShoppingCart> GetBasketAsync(string username, CancellationToken cancellationToken = default)
        {
           var basket=await session.LoadAsync<ShoppingCart>(username,cancellationToken);
           return basket is null ? throw new BasketNotFoundException(username) : basket;
        }

        public  async Task<ShoppingCart> StoreBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default)
        {
            session.Store(basket);
            await session.SaveChangesAsync(cancellationToken);
            return basket;
        }
    }
}

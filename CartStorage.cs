using System.Collections.Concurrent;

namespace ShoppingCart
{
    public interface ICartStorage
    {
        Dictionary<int,int> GetCart(int cartId);
        void AddItem(int cartId, int itemId);
        void RemoveItem(int cartId, int itemId);
        ConcurrentDictionary<int, Dictionary<int, int>> GetAllCarts();
    }

    public class CartStorage : ICartStorage
    {
        private ConcurrentDictionary<int, Dictionary<int, int>> _storage;

        public CartStorage()
        {
            // Adding some test data here for easy of use via the swagger ui
            var testData = new Dictionary<int, int>() { { 2, 3 } };
            _storage = new ConcurrentDictionary<int, Dictionary<int, int>>();
            _storage.TryAdd(1, testData);
        }

        public void AddItem(int cartId, int itemId)
        {
            // If cart doesn't exist, make new cart with only the item in it.
            // If it does exist, update the cart to increase the item count
            _storage.AddOrUpdate(cartId, _ => new Dictionary<int, int> { { itemId, 1 } }, (cartId, cart) =>
            {
                if (cart.ContainsKey(itemId))
                {
                    cart[itemId] += 1;
                }
                else
                {
                    cart[itemId] = 1;
                }

                return cart;
            });
        }

        public void RemoveItem(int cartId, int itemId)
        {
            // Removing an item that doesn't exist should really be a no-op, but the ConcurrentDictionary API makes that a bit awkward
            _storage.AddOrUpdate(cartId, _ => new Dictionary<int, int> { }, (cartId, cart) =>
            {
                if (cart.ContainsKey(itemId))
                {
                    var itemCount = cart[itemId];
                    if (itemCount == 1)
                    {
                        cart.Remove(itemId);
                    }
                    else
                    {
                        cart[itemId] -= 1;
                    }
                }

                return cart;
            });
        }

        public Dictionary<int,int> GetCart(int cartId)
        {
            if (_storage.TryGetValue(cartId, out var cart))
            {
                return cart;
            }
            else
            {
                // Would probably make a custom exception here, but sticking with plain old Exception to keep time.
                throw new Exception($"Cart with ID {cartId} not found");
            }
        }

        public ConcurrentDictionary<int,Dictionary<int,int>> GetAllCarts()
        {
            return _storage;
        }
    }
}

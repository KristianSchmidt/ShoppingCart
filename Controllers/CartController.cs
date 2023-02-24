using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace ShoppingCart.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartStorage _cartStorage;

        public CartController(ICartStorage cartStorage)
        {
            _cartStorage = cartStorage;
        }

        [HttpGet()]
        [Route("/list")]
        public ActionResult<Dictionary<int, int>> List(int cartId)
        {
            try
            {
                return _cartStorage.GetCart(cartId);
            }
            catch (Exception) // Catching blanket exceptions is of course not preferred, but will have to make do
            {
                return NotFound();
            }
        }

        [HttpPost()]
        [Route("/add")]
        public void AddItem(int cartId, int itemId)
        {
            _cartStorage.AddItem(cartId, itemId);
        }

        [HttpDelete()]
        [Route("/remove")]
        public void RemoveItem(int cartId, int itemId)
        {
            _cartStorage.RemoveItem(cartId, itemId);
        }

        [HttpGet()]
        [Route("/getall")]
        public ConcurrentDictionary<int,Dictionary<int,int>> GetAllCarts()
        {
            return _cartStorage.GetAllCarts();
        }
    }
}

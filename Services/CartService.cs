using Newtonsoft.Json;
using BlojayElectronics.Models;

namespace BlojayElectronics.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "ShoppingCart";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private List<CartItem> GetCartItems()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return new List<CartItem>();
            var cartJson = session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cartJson) ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }

        public void AddToCart(Product product, int quantity = 1)
        {
            var cart = GetCartItems();
            var existing = cart.FirstOrDefault(c => c.ProductId == product.Id);
            if (existing != null) existing.Quantity += quantity;
            else
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl
                });
            SaveCart(cart);
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null) cart.Remove(item);
            SaveCart(cart);
        }

        public void UpdateQuantity(int productId, int newQuantity)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                if (newQuantity <= 0) cart.Remove(item);
                else item.Quantity = newQuantity;
            }
            SaveCart(cart);
        }

        public List<CartItem> GetCart() => GetCartItems();
        public decimal GetTotal() => GetCartItems().Sum(c => c.Price * c.Quantity);
        public void ClearCart() => SaveCart(new List<CartItem>());
    }
}
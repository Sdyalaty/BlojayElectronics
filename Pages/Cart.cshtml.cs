using BlojayElectronics.Models;
using BlojayElectronics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlojayElectronics.Pages
{
    public class CartModel : PageModel
    {
        private readonly CartService _cart;
        public CartModel(CartService cart) => _cart = cart;
        public List<CartItem> CartItems { get; set; } = new();
        public decimal Total { get; set; }
        public void OnGet() => LoadCart();
        public IActionResult OnPostUpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0) _cart.RemoveFromCart(productId);
            else _cart.UpdateQuantity(productId, quantity);
            return RedirectToPage();
        }
        public IActionResult OnPostRemove(int productId)
        {
            _cart.RemoveFromCart(productId);
            return RedirectToPage();
        }
        private void LoadCart()
        {
            CartItems = _cart.GetCart();
            Total = _cart.GetTotal();
        }
    }
}

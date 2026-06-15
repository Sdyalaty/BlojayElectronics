using BlojayElectronics.Data;
using BlojayElectronics.Models;
using BlojayElectronics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BlojayElectronics.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly CartService _cart;
        public CheckoutModel(AppDbContext db, CartService cart) { _db = db; _cart = cart; }
        [BindProperty] public CheckoutInput OrderInput { get; set; } = new();
        public List<CartItem> CartItems { get; set; } = new();
        public decimal Total { get; set; }
        public IActionResult OnGet()
        {
            LoadCart();
            if (!CartItems.Any()) return RedirectToPage("/Cart");
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            LoadCart();
            if (!CartItems.Any()) return RedirectToPage("/Cart");
            if (!ModelState.IsValid) return Page();
            var order = new Order
            {
                CustomerName = OrderInput.FullName,
                CustomerEmail = OrderInput.Email,
                CustomerPhone = OrderInput.Phone,
                DeliveryAddress = OrderInput.Address,
                OrderDate = DateTime.UtcNow,
                TotalAmount = Total,
                OrderItems = CartItems.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.Price
                }).ToList()
            };
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            _cart.ClearCart();
            return RedirectToPage("/OrderSuccess", new { orderId = order.Id });
        }
        private void LoadCart()
        {
            CartItems = _cart.GetCart();
            Total = _cart.GetTotal();
        }
    }
    public class CheckoutInput
    {
        [Required] public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, Phone] public string Phone { get; set; } = string.Empty;
        [Required] public string Address { get; set; } = string.Empty;
    }
}
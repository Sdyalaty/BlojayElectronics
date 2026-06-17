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
        private readonly AuthService _auth; // added

        public CheckoutModel(AppDbContext db, CartService cart, AuthService auth)
        {
            _db = db;
            _cart = cart;
            _auth = auth;
        }

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

            // Get logged-in customer (if any)
            var customer = await _auth.GetLoggedInCustomerAsync();

            var order = new Order
            {
                CustomerName = OrderInput.FullName,
                CustomerEmail = OrderInput.Email,
                CustomerPhone = OrderInput.Phone,
                DeliveryAddress = OrderInput.Address,
                OrderDate = DateTime.UtcNow,
                TotalAmount = Total,
                CustomerId = customer?.Id, // save customer ID
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

            // Redirect to Payment simulation page
            return RedirectToPage("/Payment", new { orderId = order.Id });
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
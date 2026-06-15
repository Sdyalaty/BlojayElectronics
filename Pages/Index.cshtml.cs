using BlojayElectronics.Data;
using BlojayElectronics.Models;
using BlojayElectronics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlojayElectronics.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly CartService _cart;
        public IndexModel(AppDbContext db, CartService cart) { _db = db; _cart = cart; }
        public List<Product> FeaturedProducts { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public async Task OnGetAsync()
        {
            FeaturedProducts = await _db.Products.Take(4).ToListAsync();
            Categories = await _db.Categories.ToListAsync();
        }
        public async Task<IActionResult> OnPostAddToCartAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product != null) _cart.AddToCart(product, 1);
            return RedirectToPage("/Index");
        }
    }
}
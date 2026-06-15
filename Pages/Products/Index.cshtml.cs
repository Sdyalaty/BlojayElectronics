using BlojayElectronics.Data;
using BlojayElectronics.Models;
using BlojayElectronics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlojayElectronics.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly CartService _cart;
        public IndexModel(AppDbContext db, CartService cart) { _db = db; _cart = cart; }
        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        [BindProperty(SupportsGet = true)] public int SelectedCategoryId { get; set; } = 0;
        public async Task OnGetAsync()
        {
            Categories = await _db.Categories.ToListAsync();
            var query = _db.Products.Include(p => p.Category).AsQueryable();
            if (SelectedCategoryId > 0) query = query.Where(p => p.CategoryId == SelectedCategoryId);
            Products = await query.ToListAsync();
        }
        public async Task<IActionResult> OnPostAddToCartAsync(int id, string returnUrl = null)
        {
            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                _cart.AddToCart(product, 1);
                TempData["SuccessMessage"] = $"{product.Name} added to cart!";
            }
            else TempData["ErrorMessage"] = "Product not found.";
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToPage(new { selectedCategoryId = SelectedCategoryId });
            return Redirect(returnUrl);
        }
    }
}

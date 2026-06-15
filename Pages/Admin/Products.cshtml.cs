using BlojayElectronics.Data;
using BlojayElectronics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlojayElectronics.Pages.Admin
{
    public class ProductsModel : AdminPageModel
    {
        private readonly AppDbContext _db;
        public ProductsModel(AppDbContext db, AdminAuthService adminAuth) : base(adminAuth) => _db = db;
        public List<Models.Product> Products { get; set; } = new();
        public async Task OnGetAsync() => Products = await _db.Products.Include(p => p.Category).ToListAsync();
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product deleted.";
            }
            return RedirectToPage();
        }
    }
}

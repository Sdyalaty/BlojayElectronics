using BlojayElectronics.Data;
using BlojayElectronics.Models;
using BlojayElectronics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BlojayElectronics.Pages.Admin
{
    public class AddProductModel : AdminPageModel
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public AddProductModel(AppDbContext db, AdminAuthService adminAuth, IWebHostEnvironment env) : base(adminAuth)
        { _db = db; _env = env; }
        [BindProperty] public ProductInput Input { get; set; } = new();
        public List<SelectListItem> CategoryList { get; set; } = new();
        public async Task OnGetAsync() => await LoadCategories();
        public async Task<IActionResult> OnPostAsync()
        {
            await LoadCategories();
            if (!ModelState.IsValid) return Page();
            string imageUrl = "/images/products/default.jpg";
            if (Input.ImageFile != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "images", "products");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid().ToString() + "_" + Input.ImageFile.FileName;
                var filePath = Path.Combine(uploads, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await Input.ImageFile.CopyToAsync(stream);
                imageUrl = "/images/products/" + fileName;
            }
            var product = new Product
            {
                Name = Input.Name,
                Description = Input.Description,
                Price = Input.Price,
                CategoryId = Input.CategoryId,
                ImageUrl = imageUrl
            };
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product added.";
            return RedirectToPage("/Admin/Products");
        }
        private async Task LoadCategories()
        {
            var cats = await _db.Categories.ToListAsync();
            CategoryList = cats.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
        }
    }
    public class ProductInput
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        [Required, Range(0.01, 999999)] public decimal Price { get; set; }
        [Required] public int CategoryId { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
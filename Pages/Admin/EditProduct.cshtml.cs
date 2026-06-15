using BlojayElectronics.Data;
using BlojayElectronics.Models;
using BlojayElectronics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BlojayElectronics.Pages.Admin
{
    public class EditProductModel : AdminPageModel
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public EditProductModel(AppDbContext db, AdminAuthService adminAuth, IWebHostEnvironment env) : base(adminAuth)
        { _db = db; _env = env; }
        [BindProperty] public EditProductInput Input { get; set; } = new();
        public List<SelectListItem> CategoryList { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return RedirectToPage("/Admin/Products");
            Input.Id = p.Id;
            Input.Name = p.Name;
            Input.Description = p.Description;
            Input.Price = p.Price;
            Input.CategoryId = p.CategoryId;
            Input.ImageUrl = p.ImageUrl;
            await LoadCategories();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await LoadCategories();
            if (!ModelState.IsValid) return Page();
            var p = await _db.Products.FindAsync(Input.Id);
            if (p == null) return RedirectToPage("/Admin/Products");
            p.Name = Input.Name;
            p.Description = Input.Description;
            p.Price = Input.Price;
            p.CategoryId = Input.CategoryId;
            if (Input.ImageFile != null)
            {
                // delete old
                if (!string.IsNullOrEmpty(p.ImageUrl) && !p.ImageUrl.Contains("default.jpg"))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, p.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }
                var uploads = Path.Combine(_env.WebRootPath, "images", "products");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid().ToString() + "_" + Input.ImageFile.FileName;
                var filePath = Path.Combine(uploads, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await Input.ImageFile.CopyToAsync(stream);
                p.ImageUrl = "/images/products/" + fileName;
            }
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product updated.";
            return RedirectToPage("/Admin/Products");
        }
        private async Task LoadCategories()
        {
            var cats = await _db.Categories.ToListAsync();
            CategoryList = cats.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
        }
    }
    public class EditProductInput
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        [Required, Range(0.01, 999999)] public decimal Price { get; set; }
        [Required] public int CategoryId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; }
    }
}

using BlojayElectronics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BlojayElectronics.Pages.Admin
{
    public class LoginModel : PageModel
    {
        private readonly AdminAuthService _adminAuth;
        public LoginModel(AdminAuthService adminAuth) => _adminAuth = adminAuth;
        [BindProperty] public AdminLoginInput Input { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;
        public void OnGet()
        {
            if (_adminAuth.IsLoggedIn()) RedirectToPage("/Admin/Products");
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var success = await _adminAuth.LoginAsync(Input.Username, Input.Password);
            if (!success)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }
            return RedirectToPage("/Admin/Products");
        }
    }
    public class AdminLoginInput
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
    }
}

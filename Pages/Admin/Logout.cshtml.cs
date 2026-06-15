using BlojayElectronics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlojayElectronics.Pages.Admin
{
    public class LogoutModel : PageModel
    {
        private readonly AdminAuthService _adminAuth;
        public LogoutModel(AdminAuthService adminAuth) => _adminAuth = adminAuth;
        public IActionResult OnGet()
        {
            _adminAuth.Logout();
            return RedirectToPage("/Admin/Login");
        }
    }
}
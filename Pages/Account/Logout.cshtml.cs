using BlojayElectronics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlojayElectronics.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly AuthService _auth;
        public LogoutModel(AuthService auth) => _auth = auth;
        public IActionResult OnGet()
        {
            _auth.Logout();
            return RedirectToPage("/Index");
        }
    }
}
using BlojayElectronics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlojayElectronics.Pages.Admin
{
    public abstract class AdminPageModel : PageModel
    {
        protected readonly AdminAuthService _adminAuth;
        public AdminPageModel(AdminAuthService adminAuth) => _adminAuth = adminAuth;
        protected IActionResult EnsureAuthenticated()
        {
            if (!_adminAuth.IsLoggedIn()) return RedirectToPage("/Admin/Login");
            return null;
        }
        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var result = EnsureAuthenticated();
            if (result != null) { context.Result = result; return; }
            await next();
        }
    }
}
using BlojayElectronics.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlojayElectronics.Components
{
    public class LoginStatusViewComponent : ViewComponent
    {
        private readonly AuthService _auth;
        public LoginStatusViewComponent(AuthService auth) => _auth = auth;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _auth.GetLoggedInCustomerAsync();
            var model = new LoginStatusViewModel
            {
                IsLoggedIn = user != null,
                CustomerName = user?.FullName ?? ""
            };
            return View(model);
        }
    }

    public class LoginStatusViewModel
    {
        public bool IsLoggedIn { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }
}
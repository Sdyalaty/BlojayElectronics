using BlojayElectronics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BlojayElectronics.Pages.Account
{
    public class SignInModel : PageModel
    {
        private readonly AuthService _auth;
        public SignInModel(AuthService auth) => _auth = auth;
        [BindProperty] public LoginInput Input { get; set; } = new();
        public void OnGet() { }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var success = await _auth.LoginAsync(Input.Email, Input.Password);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }
            return RedirectToPage("/Index");
        }
    }
    public class LoginInput
    {
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }
}

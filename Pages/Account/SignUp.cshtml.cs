using BlojayElectronics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BlojayElectronics.Pages.Account
{
    public class SignUpModel : PageModel
    {
        private readonly AuthService _auth;
        public SignUpModel(AuthService auth) => _auth = auth;
        [BindProperty] public RegisterInput Input { get; set; } = new();
        public void OnGet() { }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            if (Input.Password != Input.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return Page();
            }
            var success = await _auth.RegisterAsync(Input.FullName, Input.Email, Input.Password);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Email already registered.");
                return Page();
            }
            await _auth.LoginAsync(Input.Email, Input.Password);
            return RedirectToPage("/Index");
        }
    }
    public class RegisterInput
    {
        [Required] public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, MinLength(3)] public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")] public string ConfirmPassword { get; set; } = string.Empty;
    }
}
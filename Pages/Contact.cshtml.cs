using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlojayElectronics.Pages
{
    public class ContactModel : PageModel
    {
        [BindProperty] public ContactInput Contact { get; set; } = new();
        public bool Sent { get; set; }
        public void OnGet() { }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            Sent = true;
            return Page();
        }
    }
    public class ContactInput
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
using BlojayElectronics.Data;
using BlojayElectronics.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlojayElectronics.Pages
{
    public class PaymentModel : PageModel
    {
        private readonly AppDbContext _db;

        public PaymentModel(AppDbContext db)
        {
            _db = db;
        }

        public Order? Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int orderId)
        {
            Order = await _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (Order == null)
                return RedirectToPage("/Index");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int orderId)
        {
            // Simulate payment processing – just a delay
            await Task.Delay(500); // pretend processing

            // Redirect to order confirmation
            return RedirectToPage("/OrderSuccess", new { orderId });
        }
    }
}
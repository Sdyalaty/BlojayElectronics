using BlojayElectronics.Data;
using BlojayElectronics.Models;
using BlojayElectronics.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlojayElectronics.Pages.Account
{
    public class OrdersModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly AuthService _auth;

        public OrdersModel(AppDbContext db, AuthService auth)
        {
            _db = db;
            _auth = auth;
        }

        public List<Order> Orders { get; set; } = new();

        public async Task OnGetAsync()
        {
            var customer = await _auth.GetLoggedInCustomerAsync();
            if (customer != null)
            {
                Orders = await _db.Orders
                    .Where(o => o.CustomerId == customer.Id)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
        }
    }
}

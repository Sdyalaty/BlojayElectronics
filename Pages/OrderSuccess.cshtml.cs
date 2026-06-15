using BlojayElectronics.Data;
using BlojayElectronics.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlojayElectronics.Pages
{
    public class OrderSuccessModel : PageModel
    {
        private readonly AppDbContext _db;
        public OrderSuccessModel(AppDbContext db) => _db = db;
        public Order? Order { get; set; }
        public async Task OnGetAsync(int orderId)
        {
            Order = await _db.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId);
        }
    }
}

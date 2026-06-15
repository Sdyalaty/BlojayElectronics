using BlojayElectronics.Data;
using BlojayElectronics.Models;
using Microsoft.EntityFrameworkCore;

namespace BlojayElectronics.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string UserSessionKey = "LoggedInCustomerId";

        public AuthService(AppDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> RegisterAsync(string fullName, string email, string password)
        {
            if (await _db.Customers.AnyAsync(c => c.Email == email)) return false;
            var customer = new Customer { FullName = fullName, Email = email, Password = password };
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == email && c.Password == password);
            if (customer == null) return false;
            _httpContextAccessor.HttpContext?.Session.SetInt32(UserSessionKey, customer.Id);
            return true;
        }

        public void Logout() => _httpContextAccessor.HttpContext?.Session.Remove(UserSessionKey);
        public async Task<Customer?> GetLoggedInCustomerAsync()
        {
            var id = _httpContextAccessor.HttpContext?.Session.GetInt32(UserSessionKey);
            return id == null ? null : await _db.Customers.FindAsync(id.Value);
        }
        public bool IsLoggedIn() => _httpContextAccessor.HttpContext?.Session.GetInt32(UserSessionKey) != null;
    }
}

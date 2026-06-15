using BlojayElectronics.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BlojayElectronics.Services
{
    public class AdminAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _db;
        private const string AdminSessionKey = "AdminLoggedIn";
        private const string AdminIdKey = "AdminId";

        public AdminAuthService(IHttpContextAccessor httpContextAccessor, AppDbContext db)
        {
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }

        public bool IsLoggedIn()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return session != null && session.GetInt32(AdminSessionKey) == 1;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var admin = await _db.Admins.FirstOrDefaultAsync(a => a.Username == username);
            if (admin == null) return false;
            if (!VerifyPassword(password, admin.PasswordHash)) return false;
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetInt32(AdminSessionKey, 1);
                session.SetInt32(AdminIdKey, admin.Id);
            }
            return true;
        }

        public void Logout()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.Remove(AdminSessionKey);
            session?.Remove(AdminIdKey);
        }

        private bool VerifyPassword(string plain, string hash)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(plain);
            var computed = Convert.ToBase64String(sha256.ComputeHash(bytes));
            return computed == hash;
        }
    }
}
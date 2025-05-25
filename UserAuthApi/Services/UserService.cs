using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UserAuthApi.Context;
using UserAuthApi.Models;

namespace UserAuthApi.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string username, string password)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Username == username);
            if (userExists)
            {
                return (false, "Användarnamnet är redan upptaget");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return (true, "Användare registrerad");
        }

        public async Task<(bool Success, string Message)> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return (false, "Felaktigt användarnamn eller lösenord");
            }

            return (true, "Inloggning lyckades");
        }
    }
}

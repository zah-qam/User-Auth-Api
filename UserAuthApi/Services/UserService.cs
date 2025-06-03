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
        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasUpper = false,
                 hasLower = false,
                 hasDigit = false,
                 hasSpecial = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
                if (hasUpper && hasLower && hasDigit && hasSpecial)
                    return true;
            }
            return false;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string username, string password)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Username == username);
            if (userExists)
            {
                return (false, "Användarnamnet är redan upptaget");
            }
            
            if (!IsValidPassword(password))
            {
                return (false, "Lösenordet måste vara minst 8 tecken långt och innehålla en stor bokstav, liten bokstav, siffra och specialtecken.");
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

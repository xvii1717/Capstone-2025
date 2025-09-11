using Microsoft.EntityFrameworkCore;
using ProductivityApp.Data;
using ProductivityApp.Models;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace ProductivityApp.Services
{
    public class UserService : IUserService
    {
        private readonly ProductivityDbContext _context;

        public UserService(ProductivityDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null) return null;

            var inputHash = HashPassword(password, user.Salt);
            if (inputHash != user.PasswordHash) return null;

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> RegisterAsync(string username, string password, string? securityAnswer = null)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Username == username))
                return null;

            var salt = GenerateSalt();
            var passwordHash = HashPassword(password, salt);

            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                Salt = salt,
                SecurityAnswer = securityAnswer?.ToLowerInvariant(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await LogSecurityEventAsync(user.Id, "Registration", "User account created");

            return user;
        }

        public async Task<bool> ResetPasswordAsync(string username, string securityAnswer, string newPassword)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null || user.SecurityAnswer == null) return false;

            if (!user.SecurityAnswer.Equals(securityAnswer, StringComparison.OrdinalIgnoreCase))
                return false;

            var salt = GenerateSalt();
            user.PasswordHash = HashPassword(newPassword, salt);
            user.Salt = salt;

            await _context.SaveChangesAsync();
            await LogSecurityEventAsync(user.Id, "PasswordReset", "Password reset via security question");

            return true;
        }

        public async Task<bool> IsAccountLockedAsync(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return false;

            var recentFailedAttempts = await _context.SecurityLogs
                .Where(sl => sl.UserId == user.Id 
                    && sl.EventType == "FailedLogin" 
                    && sl.Timestamp > DateTime.UtcNow.AddMinutes(-15))
                .CountAsync();

            return recentFailedAttempts >= 5;
        }

        public async Task RecordLoginAttemptAsync(string username, bool successful, string? ipAddress = null)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return;

            var eventType = successful ? "Login" : "FailedLogin";
            var description = successful ? "Successful login" : "Failed login attempt";

            await LogSecurityEventAsync(user.Id, eventType, description, successful);
        }

        public async Task<UserSession?> CreateSessionAsync(int userId, TimeSpan duration)
        {
            // Invalidate old sessions
            var oldSessions = await _context.UserSessions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync();

            foreach (var session in oldSessions)
            {
                session.IsActive = false;
            }

            // Create new session
            var sessionToken = GenerateSessionToken();
            var newSession = new UserSession
            {
                UserId = userId,
                SessionToken = sessionToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(duration),
                IsActive = true,
                DeviceInfo = Environment.MachineName
            };

            _context.UserSessions.Add(newSession);
            await _context.SaveChangesAsync();

            return newSession;
        }

        public async Task<User?> ValidateSessionAsync(string sessionToken)
        {
            var session = await _context.UserSessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.SessionToken == sessionToken 
                    && s.IsActive 
                    && s.ExpiresAt > DateTime.UtcNow);

            if (session == null) return null;

            session.LastAccessedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return session.User;
        }

        public async Task InvalidateSessionAsync(string sessionToken)
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.SessionToken == sessionToken);

            if (session != null)
            {
                session.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task LogSecurityEventAsync(int userId, string eventType, string? description = null, bool successful = true)
        {
            var securityLog = new SecurityLog
            {
                UserId = userId,
                EventType = eventType,
                Description = description,
                Timestamp = DateTime.UtcNow,
                IsSuccessful = successful,
                IpAddress = "127.0.0.1" // Local application
            };

            _context.SecurityLogs.Add(securityLog);
            await _context.SaveChangesAsync();
        }

        private string HashPassword(string password, string salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 10000);
            return Convert.ToBase64String(pbkdf2.GetBytes(32));
        }

        private string GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        private string GenerateSessionToken()
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] tokenBytes = new byte[32];
            rng.GetBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes);
        }
    }
}

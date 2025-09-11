using ProductivityApp.Models;
using System.Threading.Tasks;
using System;

namespace ProductivityApp.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<User?> RegisterAsync(string username, string password, string? securityAnswer = null);
        Task<bool> ResetPasswordAsync(string username, string securityAnswer, string newPassword);
        Task<bool> IsAccountLockedAsync(string username);
        Task RecordLoginAttemptAsync(string username, bool successful, string? ipAddress = null);
        Task<UserSession?> CreateSessionAsync(int userId, TimeSpan duration);
        Task<User?> ValidateSessionAsync(string sessionToken);
        Task InvalidateSessionAsync(string sessionToken);
        Task LogSecurityEventAsync(int userId, string eventType, string? description = null, bool successful = true);
    }
}

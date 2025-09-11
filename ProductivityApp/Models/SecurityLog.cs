using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace ProductivityApp.Models
{
    public class SecurityLog
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string EventType { get; set; } = string.Empty; // Login, Logout, FailedLogin, PasswordReset, etc.
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [StringLength(45)]
        public string? IpAddress { get; set; }
        
        [StringLength(200)]
        public string? UserAgent { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public bool IsSuccessful { get; set; } = true;
        
        [StringLength(100)]
        public string? AdditionalData { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
    }
}

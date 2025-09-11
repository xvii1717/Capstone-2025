using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace ProductivityApp.Models
{
    public class UserSession
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string SessionToken { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public DateTime? LastAccessedAt { get; set; }
        
        [StringLength(200)]
        public string? DeviceInfo { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
    }
}

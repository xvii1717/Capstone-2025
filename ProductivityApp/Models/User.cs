using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ProductivityApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Salt { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? SecurityAnswer { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();
        public virtual ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
        public virtual ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
        public virtual ICollection<SecurityLog> SecurityLogs { get; set; } = new List<SecurityLog>();
    }
}

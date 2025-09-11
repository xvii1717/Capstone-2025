using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace ProductivityApp.Models
{
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(2000)]
        public string? Description { get; set; }
        
        public DateTime? DueDate { get; set; }
        
        [StringLength(20)]
        public string Priority { get; set; } = "Medium";
        
        [StringLength(50)]
        public string Status { get; set; } = "Pending";
        
        [StringLength(50)]
        public string? Category { get; set; }
        
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        
        public int? EstimatedMinutes { get; set; }
        public int? ActualMinutes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual CalendarEvent? RelatedEvent { get; set; }
        
        [ForeignKey("RelatedEvent")]
        public int? RelatedEventId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace ProductivityApp.Models
{
    public class CalendarEvent
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
        
        [Required]
        public DateTime EventDate { get; set; }
        
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        
        [StringLength(50)]
        public string? Category { get; set; }
        
        [StringLength(20)]
        public string Priority { get; set; } = "Medium";
        
        public bool IsCompleted { get; set; } = false;
        public bool IsRecurring { get; set; } = false;
        
        [StringLength(50)]
        public string? RecurrencePattern { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual TodoItem? RelatedTodo { get; set; }
    }
}

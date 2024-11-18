using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    public class Todo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string UserId { get; set; } // Owner of the To-Do item

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } // Navigation property

        [Required]
        public TodoStatus Status { get; set; } = TodoStatus.Pending; // Default to Pending
    }

    public enum TodoStatus
    {
        Pending,
        Completed,
        InProgress,
        Overdue,
        Cancelled
    }
}

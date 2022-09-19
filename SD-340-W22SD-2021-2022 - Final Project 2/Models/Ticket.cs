using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Models
{
    public enum Priority
    {
        low,
        medium,
        high
    }

    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        public bool Completed { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [Range(1, 999)]
        public int Hours { get; set; }

        [Required]
        public Priority Priority { get; set; }
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        //public virtual ICollection<Comment> Comment { get; set; } = new HashSet<Comment>();
        [ForeignKey("DeveloperId")]
        public string? DeveloperId { get; set; }
        public virtual ApplicationUser? Developer { get; set; } 
    }
}

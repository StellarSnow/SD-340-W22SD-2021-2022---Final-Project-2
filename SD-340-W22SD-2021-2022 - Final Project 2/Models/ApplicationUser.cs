using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Models
{
    public class ApplicationUser : IdentityUser
    {
        [InverseProperty("Developers")]
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }
}

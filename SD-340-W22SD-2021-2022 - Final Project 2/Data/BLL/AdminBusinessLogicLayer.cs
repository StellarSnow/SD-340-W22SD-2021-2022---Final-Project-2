using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Data.BLL
{
    public class AdminBusinessLogicLayer
    {
        public ApplicationDbContext _context { get; set; }

        public AdminBusinessLogicLayer(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public List<ApplicationUser> GetUnassignedDevelopers()
        {
            List<string> userIds = _context.UserRoles.Select(ur => ur.UserId).ToList();
            List<ApplicationUser> users = _context.Users.Where(u => !userIds.Contains(u.Id)).ToList();
            
            return users;
        }



    }
}

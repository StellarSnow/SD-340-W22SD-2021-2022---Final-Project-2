using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Data.BLL
{
    public class AdminBusinessLogicLayer
    {
        public ApplicationDbContext _context { get; set; }
        private ProjectRepository _repo;
        private UserManager<ApplicationUser> _userManager;

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
        public ICollection<ApplicationUser> UnassignedDeveloperCheck()
        {
            return _repo.GetAllUsers();
        }

        public void AssignDeveloper(int id)
        {
            ApplicationUser user = _repo.FindUser(id);

            if (id != null)
            {
                _userManager.AddToRoleAsync(user, "Developer");

            }
            else
            {
                throw new Exception("ID not found");
            }

        }

        public void AssignProjectManager(int id)
        {
            ApplicationUser user = _repo.FindUser(id);

            if (id != null)
            {
                _userManager.AddToRoleAsync(user, "Project Manager");

            }
            else
            {
                throw new Exception("ID not found");
            }

        }


    }
}

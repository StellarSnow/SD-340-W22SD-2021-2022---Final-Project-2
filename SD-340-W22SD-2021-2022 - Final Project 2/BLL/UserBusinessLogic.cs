using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.BLL
{
    public class UserBusinessLogic
    {
        public UserManager<ApplicationUser> _userManager;
        
        public UserBusinessLogic(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        // To UnitTest
        // Valid: Test if return a list of users without role.
        // Invalid:.
        public async Task<List<ApplicationUser>> GetAllUsersWithoutRoleAsync()
        {
            List<ApplicationUser> usersWithoutRole = new List<ApplicationUser>();
            List<ApplicationUser> users = _userManager.Users.ToList();

            foreach(ApplicationUser user in users)
            {
                List<string> userRoles = (List<string>)await _userManager.GetRolesAsync(user);

                if(userRoles.Count() == 0)
                {
                    usersWithoutRole.Add(user);
                }
            }
            
            return usersWithoutRole;
        }
        // To UnitTest
        // Valid: Test if user will be changing the role
        // Invalid: If role is not valid throw Argument Exception.
        public async Task AssignUserToARoleAsync(ApplicationUser user, string role)
        {
            if(role == "Developer" || role == "Project Manager")
            {
                await _userManager.AddToRoleAsync(user, role);
            } else
            {
                throw new ArgumentException("Invalid Role");
            }
            
        }
        // To UnitTest
        // Valid: Test if list will return roles of the user
        // Invalid:.
        public async Task <List<string>> GetUserRoles(ApplicationUser user)
        {
            List<string> userRoles = (List<string>)await _userManager.GetRolesAsync(user);
            return userRoles;
            
        }
        // To UnitTest
        // Valid: Test if passing a role will return a list of users.
        // Invalid: If role argument passed an invalid role throw an exception.
        public async Task<List<ApplicationUser>> GetAllUsersWithSpecificRoleAsync(string role)
        {
            if(role == "Developer" || role == "Project Manager")
            {
                return (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync(role);
            } else
            {
                throw new ArgumentException("There's no users with the given role");
            }   
        }
        // To UnitTest
        // Valid: Test method returns a user with the given user id.
        // Invalid: Test method if returns an invalid user throw a nullreferenceexception.
        public ApplicationUser GetUserByUserId(string userId)
        {
            try
            {
                ApplicationUser currUser = _userManager.Users
                    .Include(user => user.OwnedTickets)
                    .Include(user => user.WatchedTickets)
                    .Include(user => user.Tickets)
                    .Include(user => user.Projects)
                    .First(user => user.Id == userId);
                return currUser;
            } catch
            {
                throw new NullReferenceException("User not found");
            }
        }
        // To UnitTest
        // Valid: Test method returns a user with the given name identity.
        // Invalid: Test method if returns an invalid user throw a nullreferenceexception.
        public async Task<ApplicationUser> GetCurrentUserByNameAsync(string identity)
        {
            try
            {
                List <ApplicationUser> users = _userManager.Users.ToList();
                ApplicationUser currUser = await _userManager.FindByNameAsync(identity);
                return currUser;
            } catch
            {
                throw new NullReferenceException("User not found");
            }
            
        }

    }
}

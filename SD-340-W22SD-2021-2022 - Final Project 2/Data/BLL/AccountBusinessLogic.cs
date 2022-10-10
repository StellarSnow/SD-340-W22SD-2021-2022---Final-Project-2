using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;
using System.Linq.Expressions;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Data.BLL
{
    public class AccountBusinessLogic
    {
        private ProjectRepository _repo;
        private UserManager<ApplicationUser> _userManager;
        public AccountBusinessLogic(ProjectRepository repo)
        {
            _repo = repo;
        }

        public Project ProjectDetails (int projectID)
        {
            Project project = _repo.Get(projectID);
            if (project != null)
            {
                return project;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        
        public void CreateProject(string name)
        {   
            Project newProject = new Project();
            newProject.Name = name;
            _repo.Add(newProject);
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
                
            }else
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

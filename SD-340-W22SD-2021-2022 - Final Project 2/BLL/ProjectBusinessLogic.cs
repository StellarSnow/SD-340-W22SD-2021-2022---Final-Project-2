using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_2021_2022___Final_Project_2.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.BLL
{
    public class ProjectBusinessLogic
    {
        public IRepository<Project> repo;
        public readonly UserManager<ApplicationUser> _userManager;

        public ProjectBusinessLogic(IRepository<Project> repo, UserManager<ApplicationUser> userManager)
        {
            this.repo = repo;
            this._userManager = userManager;
        }
        // To UnitTest
        // Valid: Test if project list is returning the correct count
        // Invalid: Test if count is 0 if invalid devId
        public async Task<List<Project>> GetAllProjectsByDeveloperAsync(string userId)
        {
            ApplicationUser currUser = await _userManager.FindByIdAsync(userId);
            if (currUser != null)
            {
                return repo.GetList(p => p.Developers.Contains(currUser)).ToList();
            } else
            {
                throw new NullReferenceException("User not found!");
            }

            
        }
        // To UnitTest
        // Valid: Test if project list is returning the correct count.
        // Invalid:
        public List<Project> GetAllProjects()
        {
            return repo.GetAll().ToList();
        }
        // To UnitTest
        // Valid: Test if the correct project is being returned
        // Invalid: Test if the throws an exception if project not found.
        public Project GetProjectDetails(int projId)
        {
            try
            {
                Project currProject = repo.Get(projId);
                return currProject;
            } catch
            {
                throw new NullReferenceException("Project not found");
            }
            
        }
        // To UnitTest
        // Valid: Test if Project count is incrementing
        // Invalid: If ticket count is still the same
        public void CreateProject(Project project)
        {
            repo.Create(project);
            repo.Save();
        }

    }
}

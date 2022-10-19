using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.Data;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;
using SD_340_W22SD_2021_2022___Final_Project_2.Models.ViewModels;
using SD_340_W22SD_2021_2022___Final_Project_2.BLL;
using SD_340_W22SD_2021_2022___Final_Project_2.DAL;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Controllers
{
    public class ProjectController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProjectBusinessLogic projectBL;
        private readonly UserBusinessLogic userBL;
        private readonly TicketBusinessLogic ticketBL;
        public ProjectController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            projectBL = new ProjectBusinessLogic(new ProjectRepository(context), _userManager);
            userBL = new UserBusinessLogic(_userManager);
            ticketBL = new TicketBusinessLogic(new TicketRepository(context), _userManager);
        }

        [Authorize(Roles = "Project Manager, Developer")]
        public async Task<IActionResult> Index(string? hours, string? priority, bool? completed)
        {
            List<Project>? projects = null;

            try
            {
                ApplicationUser user = await userBL.GetCurrentUserByNameAsync(User.Identity.Name);
                List<string> roles = await userBL.GetUserRoles(user);
                string role = roles.Find(r => r.Equals("Developer"));
                if (role == null)
                {
                    role = roles.Find(r => r.Equals("Project Manager"));
                }
                if (role.Equals("Developer"))
                {
                    projects = await projectBL.GetAllProjectsByDeveloperAsync(user.Id);
                }

                if (role.Equals("Project Manager"))
                {
                    projects = projectBL.GetAllProjects();
                }

                //Abstract into helper method for unit testing.
                if (hours == "asc")
                {
                    projects.ForEach(p =>
                    {
                        p.Ticket = p.Ticket.OrderBy(t => t.Hours).ToList();
                    });
                }
                else if (hours == "desc")
                {
                    projects.ForEach(p =>
                    {
                        p.Ticket = p.Ticket.OrderByDescending(t => t.Hours).ToList();
                    });
                }
                else if (priority == "asc")
                {
                    projects.ForEach(p =>
                    {
                        p.Ticket = p.Ticket.OrderBy(t => t.Priority).ToList();
                    });
                }
                else if (priority == "desc")
                {
                    projects.ForEach(p =>
                    {
                        p.Ticket = p.Ticket.OrderByDescending(t => t.Priority).ToList();
                    });
                }
                //No Database call ends here

                if (completed == true)
                {
                    projects.ForEach(p =>
                    {
                        p.Ticket = ticketBL.GetCompletedTickets(p.Id);
                    });
                } else if (completed == false)
                {
                    projects.ForEach(p =>
                    {
                        p.Ticket = ticketBL.GetUncompletedTickets(p.Id);
                    });
                }
               
                return View(projects);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Create()
        {
            List<ApplicationUser>? developers;
            CreateProjectViewModel vm;
            Project project = new Project();

            try
            {
                developers = await userBL.GetAllUsersWithSpecificRoleAsync("Developer");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }

            vm = new CreateProjectViewModel(project, developers);

            return View(vm);
        }

        [Authorize(Roles = "Project Manager")]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id, Name, Ticket, Developers")] Project project, string[] developerIds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                foreach (String developerId in developerIds)
                {
                    ApplicationUser dev = userBL.GetUserByUserId(developerId);
                    project.Developers.Add(dev);
                }
                projectBL.CreateProject(project);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult Details(int projectId)
        {
            Project project = projectBL.GetProjectDetails(projectId);
            if (project == null)
            {
                return NotFound();
            }
            List<Ticket> tickets = project.Ticket.ToList();
            List<ApplicationUser> developers = project.Developers.ToList();
            return View(project);
        }
    }
}

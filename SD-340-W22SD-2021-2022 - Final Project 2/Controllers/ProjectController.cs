using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.Data;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;
using SD_340_W22SD_2021_2022___Final_Project_2.Models.ViewModels;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.BLL;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Controllers
{
    public class ProjectController : Controller
    {
        private ProjectRepository _projectRepository;
        private AccountBusinessLogic _accountBLL;
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _projectRepository = new ProjectRepository(context);
            _accountBLL = new AccountBusinessLogic(_projectRepository);
        }

        [Authorize(Roles = "Project Manager, Developer")]
        public async Task<IActionResult> Index(string? hours, string? priority, bool? completed)
        {
            List<Project>? projects = null;

            try
            {
                ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                List<string> roles = (List<String>)await _userManager.GetRolesAsync(user);
                string role = roles.Find(r => r.Equals("Developer"));

                if (role == null)
                {
                    role = roles.Find(r => r.Equals("Project Manager"));
                }

                if (role.Equals("Developer"))
                {
                    projects = _context.Project
                        .Include(p => p.Ticket)
                        .Include(d => d.Developers)
                        .Where(p => p.Developers.Any(p => p.Id.Equals(user.Id)))
                        .OrderBy(p => p.Name).ToList();
                }

                if (role.Equals("Project Manager"))
                {
                    projects = _context.Project
                        .Include(p => p.Ticket)
                        .OrderBy(p => p.Name)
                        .ToList();
                }

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

                if(completed == true)
                {
                    projects.ForEach(p =>
                    {
                        p.Ticket = p.Ticket.Where(t => t.Completed == true).ToList();
                    });
                } else if (completed == false)
                {
                    projects.ForEach(p =>
                    {
                        p.Ticket = p.Ticket.Where(t => t.Completed == false).ToList();
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
                developers = (List<ApplicationUser>?)await _userManager.GetUsersInRoleAsync("Developer");
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
                await _accountBLL.CreateProjectAsync(developerIds);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult Details(int projectId)
        {
            return View(_accountBLL.ProjectDetails(projectId));
        }
    }
}

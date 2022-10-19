using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.BLL;
using SD_340_W22SD_2021_2022___Final_Project_2.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Data;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;
using SD_340_W22SD_2021_2022___Final_Project_2.Models.ViewModels;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TicketBusinessLogic ticketBL;
        private readonly ProjectBusinessLogic projectBL;
        private readonly UserBusinessLogic userBL;

        public TicketController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            ticketBL = new TicketBusinessLogic(new TicketRepository(context), _userManager);
            projectBL = new ProjectBusinessLogic(new ProjectRepository(context), _userManager);
            userBL = new UserBusinessLogic(_userManager);
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Create(int projectId)
        {
            Project? project = projectBL.GetProjectDetails(projectId);
            if (project == null)
            {
                return BadRequest();
            }
            List<ApplicationUser>? developers = project.Developers.ToList();
            CreateTicketViewModel vm;
            Ticket ticket = new Ticket();
            vm = new CreateTicketViewModel(project, ticket, developers);
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Create(
            [Bind("Id, Completed, Name, Hours, Priority, ProjectId, Project")] Ticket ticket,
            int projectId, string[] taskOwnerIds, Priority priority = Priority.low)
        {
            if (taskOwnerIds.Count() == 0)
            {
                return RedirectToAction("Create", new { projectId = projectId });
            }
            Ticket newTicket = new Ticket();
            newTicket.ProjectId = projectId;
            newTicket.Name = ticket.Name;
            newTicket.Hours = ticket.Hours;
            newTicket.Priority = priority;
            newTicket.Completed = false;

            foreach (String taskOwnerId in taskOwnerIds)
            {
                ApplicationUser dev = userBL.GetUserByUserId(taskOwnerId);
                newTicket.TaskOwners.Add(dev);
            }
            ticketBL.CreateTicket(newTicket);
            return RedirectToAction("Index", "Project");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleTicket(int projectId, int ticketId)
        {
            try
            {
                ApplicationUser currentUser = await userBL.GetCurrentUserByNameAsync(User.Identity.Name);
                Ticket ticket = ticketBL.GetTicket(ticketId);
                if (ticket.TaskOwners.FirstOrDefault(to => to.Id == currentUser.Id) == null)
                {
                    return Unauthorized("Only developers who are a task owner of this project can mark a task as complete");
                }
                ticketBL.UpdateTicketStatus(ticket);
                return RedirectToAction("Details", "Project", new { projectId = projectId });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRequiredHours(int projectId, int ticketId, int hours)
        {
            try
            {
                ApplicationUser currentUser = await userBL.GetCurrentUserByNameAsync(User.Identity.Name);
                Ticket ticket = ticketBL.GetTicket(ticketId);
                if (ticket.TaskOwners.FirstOrDefault(to => to.Id == currentUser.Id) == null)
                {
                    return Unauthorized("Only developers who are a task owner of this project can adjust required hours of a task");
                }
                ticketBL.UpdateTicketRequiredHours(ticket, hours);
                return RedirectToAction("Details", "Project", new { projectId = projectId });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToWatchList(int projectId, int ticketId)
        {
            try
            {
                ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                Project project = projectBL.GetProjectDetails(projectId);
                Ticket ticket = ticketBL.GetTicket(ticketId);
                if (project.Developers.FirstOrDefault(d => d.Id == currentUser.Id) == null)
                {
                    return Unauthorized("Only developers assigned to this project can watch the tasks");
                }

                if (ticket.TaskWatchers.FirstOrDefault(u => u.Id == currentUser.Id) == null)
                {
                    ticketBL.UpdateTicketAddWatcher(ticket, currentUser);
                }
                else
                {
                    ticketBL.UpdateTicketRemoveWatcher(ticket, currentUser);
                }
                return RedirectToAction("Details", "Project", new { projectId = projectId });
            }
            catch (Exception ex)
            {
                return BadRequest();

            }
        }
    }
}

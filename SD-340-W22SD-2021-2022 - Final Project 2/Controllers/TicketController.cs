﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.Data;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.BLL;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;
using SD_340_W22SD_2021_2022___Final_Project_2.Models.ViewModels;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TicketRepository _ticketRepository;
        private ProjectRepository _projectRepository;
        private TicketBusinessLogic _ticketBLL;


        public TicketController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _ticketRepository = new TicketRepository(context);
            _projectRepository = new ProjectRepository(context);
            _ticketBLL = new TicketBusinessLogic(_ticketRepository, _projectRepository, userManager);
        }
        public IActionResult Index()

        {
            return View();
        }

        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Create(int projectId)
        {
            Project? project = await _context.Project.Include(p => p.Developers).FirstOrDefaultAsync(p => p.Id == projectId);

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
            //This returns invalid because ModelState.Project is null
            //if (!ModelState.IsValid)
            //{
            //    return RedirectToAction("Create", new { projectId = projectId });
            //}

            CreateTicketBLLViewModel viewModel = await _ticketBLL.CreateTicket(ticket, projectId, taskOwnerIds, priority);

            if (!viewModel.Succeeded)
            {
                return RedirectToAction("Create", new { projectId = projectId });
            }
            else
            {
                return RedirectToAction("Index", "Project");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleTicket(int projectId, int ticketId)
        {
            ToggleTicketBLLViewModel viewModel = await _ticketBLL.ToggleTicketAsync(User, ticketId);

            if (viewModel.Unauthorized)
            {
                return Unauthorized("Only developers who are a task owner of this project can mark a task as complete");
            }
            else if (viewModel.Succeeded)
            {
                return RedirectToAction("Details", "Project", new { projectId = projectId });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRequiredHours(int projectId, int ticketId, int hours)
        {
            ChangeRequiredHoursAsyncBLLViewModel viewModel = await _ticketBLL.ChangeRequiredHoursAsync(User, ticketId, hours);

            if (viewModel.Unauthorized)
            {
                return Unauthorized("Only developers who are a task owner of this project can adjust required hours of a task");
            }
            else if (viewModel.Succeeded)
            {
                return RedirectToAction("Details", "Project", new { projectId = projectId });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToWatchList(int projectId, int ticketId)
        {
            AddToWatchListBLLViewModel viewModel = await _ticketBLL.AddToWatchListAsync(User, projectId, ticketId);

            if (viewModel.Unauthorized)
            {
                return Unauthorized("Only developers assigned to this project can watch the tasks");
            }
            else if(viewModel.Succeeded)
            {
                return RedirectToAction("Details", "Project", new { projectId = projectId });
            }
            else
            {
                return BadRequest();
            }
        }
    }
}

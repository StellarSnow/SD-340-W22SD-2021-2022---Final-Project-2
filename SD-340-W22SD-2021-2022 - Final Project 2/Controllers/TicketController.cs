﻿using Microsoft.AspNetCore.Authorization;
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
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly TicketBusinessLogic ticketBL;

        public TicketController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            ticketBL = new TicketBusinessLogic(new TicketRepository(context), _userManager);
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult> Create(int projectId)
        {
            // add project business logic
            Project? project = await _context.Project.Include(p => p.Developers).FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                return BadRequest();
            }

            // add users business logic
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

            if (taskOwnerIds.Count() == 0)
            {
                return RedirectToAction("Create", new { projectId = projectId });
            }

            // ask zach if this is considered BL because it doesnt look  like its connecting to database
            Ticket newTicket = new Ticket();
            newTicket.ProjectId = projectId;
            newTicket.Name = ticket.Name;
            newTicket.Hours = ticket.Hours;
            newTicket.Priority = priority;
            newTicket.Completed = false;

            foreach (String taskOwnerId in taskOwnerIds)
            {
                //User business logic
                ApplicationUser dev = await _userManager.FindByIdAsync(taskOwnerId);
                newTicket.TaskOwners.Add(dev);
            }


            //await _context.Ticket.AddAsync(newTicket);
            //await _context.SaveChangesAsync();
            
            // BL
            ticketBL.CreateTicket(newTicket);
            return RedirectToAction("Index", "Project");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleTicket(int projectId, int ticketId)
        {
            try
            {
                // user businesss logic
                ApplicationUser currentUser = await _context.Users.Include(u => u.OwnedTickets).FirstAsync(u => u.UserName == User.Identity.Name);

                // get ticket BL
                //Ticket ticket = await _context.Ticket.Include(t => t.TaskOwners).FirstAsync(t => t.Id == ticketId);
                Ticket ticket = ticketBL.GetTicket(ticketId);

                if (ticket.TaskOwners.FirstOrDefault(to => to.Id == currentUser.Id) == null)
                {
                    return Unauthorized("Only developers who are a task owner of this project can mark a task as complete");
                }

                // update ticket
                //ticket.Completed = !ticket.Completed;
                //_context.Ticket.Update(ticket);
                //await _context.SaveChangesAsync();
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
                // user BL
                ApplicationUser currentUser = await _context.Users.Include(u => u.OwnedTickets).FirstAsync(u => u.UserName == User.Identity.Name);
                Ticket ticket = await _context.Ticket.Include(t => t.TaskOwners).FirstAsync(t => t.Id == ticketId);

                if (ticket.TaskOwners.FirstOrDefault(to => to.Id == currentUser.Id) == null)
                {
                    return Unauthorized("Only developers who are a task owner of this project can adjust required hours of a task");
                }


                //ticket.Hours = hours;
                ticketBL.UpdateTicketRequiredHours(ticket, hours);

                //_context.Ticket.Update(ticket);
                //await _context.SaveChangesAsync();

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
                // user BL
                ApplicationUser currentUser = await _context.Users.FirstAsync(u => u.UserName == User.Identity.Name);
                Project project = await _context.Project.Include(p => p.Developers).FirstAsync(p => p.Id == projectId);
                Ticket ticket = await _context.Ticket.Include(t => t.TaskWatchers).FirstAsync(t => t.Id == ticketId);

                if (project.Developers.FirstOrDefault(d => d.Id == currentUser.Id) == null)
                {
                    return Unauthorized("Only developers assigned to this project can watch the tasks");
                }

                if (ticket.TaskWatchers.FirstOrDefault(u => u.Id == currentUser.Id) == null)
                {
                    //ticket.TaskWatchers.Add(currentUser);
                    ticketBL.UpdateTicketAddWatcher(ticket, currentUser);
                }
                else
                {
                    //ticket.TaskWatchers.Remove(currentUser);
                    ticketBL.UpdateTicketRemoveWatcher(ticket, currentUser);
                }

                //_context.Ticket.Update(ticket);
                //await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Project", new { projectId = projectId });
            }
            catch (Exception ex)
            {
                return BadRequest();

            }
        }
    }
}

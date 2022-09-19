using Microsoft.AspNetCore.Mvc;
using SD_340_W22SD_2021_2022___Final_Project_2.Data;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Controllers
{
    public class TicketController : Controller
    {
        private ApplicationDbContext _context;

        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create(int projectId)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(int projectId, string name, int hours, Priority priority)
        {
            Ticket ticket = new Ticket();
            ticket.ProjectId = projectId;
            ticket.Name = name;
            ticket.Hours = hours;
            ticket.Priority = priority;
            ticket.Completed = false;

            _context.Ticket.Add(ticket);
            _context.SaveChanges();

            return RedirectToAction("Details", "Project", new { projectId = projectId });
        }
    }
}

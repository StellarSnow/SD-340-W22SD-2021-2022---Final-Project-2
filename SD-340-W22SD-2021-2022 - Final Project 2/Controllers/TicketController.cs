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
            ViewBag.ProjectId = projectId;

            return View();
        }

        [HttpPost]
        //public IActionResult Create(int projectId, string name, int hours, Priority priority)
        public IActionResult Create(int projectId, 
            [Bind("Completed, Name, Hours, Priority, ProjectId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Ticket.Add(ticket);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest();
            }

            return RedirectToAction("Details", "Project", new { projectId = projectId });
        }
    }
}

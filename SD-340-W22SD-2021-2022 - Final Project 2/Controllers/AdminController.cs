using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.Data;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.BLL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private AdminBusinessLogicLayer _bll;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _bll = new AdminBusinessLogicLayer(context);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UnassignedDevelopers()
        {
            List<ApplicationUser> users = _bll.GetUnassignedDevelopers();
            
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AssignDeveloper(string userId)
        {
            ApplicationUser? user = _context.Users.Find(userId);

            if (user == null)
            {
                return BadRequest();
            }

            int idUser = Int32.Parse(userId);
            _bll.AssignDeveloper(idUser);

            return RedirectToAction(nameof(UnassignedDevelopers));
        }

        [HttpPost]
        public async Task<IActionResult> AssignProjectManager(string userId)
        {
            ApplicationUser? user = _context.Users.Find(userId);

            if (user == null)
            {
                return BadRequest();
            }

            int idUser = Int32.Parse(userId);
            _bll.AssignProjectManager(idUser);

            return RedirectToAction(nameof(UnassignedDevelopers));
        }
    }
}

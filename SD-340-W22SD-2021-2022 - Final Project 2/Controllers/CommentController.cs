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
    public class CommentController : Controller
    {

        private readonly TicketRepository _ticketRepository;
        private readonly CommentRepository _commentRepository;
        private TicketBusinessLogic _ticketBLL;
        private CommentsBusinessLogicLayer _commentBLL;
        private ApplicationDbContext _context;
        private ProjectRepository _projectRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _ticketRepository = new TicketRepository(context);
            _commentRepository = new CommentRepository(context);
            _ticketBLL = new TicketBusinessLogic(_ticketRepository, _projectRepository, userManager);
            _commentBLL = new CommentsBusinessLogicLayer(context, _commentRepository, _ticketRepository, userManager);
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> CommentsForTask(int ticketId)
        {
            ViewBag.ticketId = ticketId;
            CreateCommentViewModel vm;
            List<Comment>? comments;
            Comment newComment = new Comment();

            try
            {
                comments = (List<Comment>?)_ticketBLL.CommentsOnTask(ticketId);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Project");
            }

            newComment.TicketId = ticketId;
            vm = new CreateCommentViewModel(comments, newComment);

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Developer")]
        public async Task<IActionResult> Create([Bind("Id, Content, TicketId, Ticket, UserId, User")] Comment NewComment)
        {
            CreateCommentBLLViewModel viewModel = await _commentBLL.CreateComment(User, NewComment);

            if (viewModel.Unauthorized)
            {
                return Unauthorized("Only task owners and task watchers can add comments to this task");
            }
            else if (!viewModel.Succeeded)
            {
                return RedirectToAction("Index", "Project");
            }
            else
            {
                return RedirectToAction("CommentsForTask", new { ticketId = NewComment.TicketId });
            }
        }
    }
}

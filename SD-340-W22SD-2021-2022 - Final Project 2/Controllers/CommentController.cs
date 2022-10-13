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
        private TicketBusinessLogic _ticketBLL;
        private ApplicationDbContext _context;
        private ProjectRepository _projectRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _ticketRepository = new TicketRepository(context);
            _ticketBLL = new TicketBusinessLogic(_ticketRepository, _projectRepository, userManager);
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
        [Authorize(Roles="Developer")]
        public async Task<IActionResult> Create([Bind("Id, Content, TicketId, Ticket, UserId, User")] Comment NewComment)
        {
            try
            {
                bool taskOwners = true;
                bool taskWatchers = true;

                ApplicationUser currentUser = await _context.Users.Include(u => u.OwnedTickets).FirstAsync(u => u.UserName == User.Identity.Name);
                Ticket checkTicket = await _context.Ticket
                    .Include(t => t.TaskOwners)
                    .Include(t => t.TaskWatchers)
                    .FirstAsync(t => t.Id == NewComment.TicketId);

                if (checkTicket.TaskOwners.FirstOrDefault(to => to.Id == currentUser.Id) == null)
                {
                    taskOwners = false;
                }

                if (checkTicket.TaskWatchers.FirstOrDefault(to => to.Id == currentUser.Id) == null)
                {
                    taskWatchers = false;
                }

                if (!taskOwners && !taskWatchers)
                {
                    return Unauthorized("Only task owners and task watchers can add comments to this task");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Project");
            }

            Comment comment = new Comment();

            try
            {
                string userName = User.Identity.Name;

                ApplicationUser user = await _userManager.FindByNameAsync(userName);

                Ticket ticket = await _context.Ticket.FindAsync(NewComment.TicketId);

                comment.TicketId = NewComment.TicketId;
                comment.Ticket = ticket;
                comment.Content = NewComment.Content;
                comment.User = user;
                comment.UserId = user.Id;

                _context.Comment.Add(comment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Project");
            }

            return RedirectToAction("CommentsForTask", new { ticketId = NewComment.TicketId });
        }
    }
}

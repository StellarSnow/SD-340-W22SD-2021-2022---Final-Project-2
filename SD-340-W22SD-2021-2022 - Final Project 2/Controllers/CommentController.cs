using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.BLL;
using SD_340_W22SD_2021_2022___Final_Project_2.Data;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;
using SD_340_W22SD_2021_2022___Final_Project_2.Models.ViewModels;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Controllers
{
    public class CommentController : Controller
    {
        private readonly CommentBusinessLogic commentBL;
        private readonly TicketBusinessLogic ticketBL;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            commentBL = new CommentBusinessLogic(new CommentRepository(context));
            ticketBL = new TicketBusinessLogic(new ticketRepository(context));
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
               comments = commentBL.GetAllCommentsByTicket(ticketId);
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
                Ticket checkTicket = ticketBL.FindTicketById(NewComment.TicketId);

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

            try
            {
                string userName = User.Identity.Name;

                ApplicationUser user = await _userManager.FindByNameAsync(userName);

                Ticket ticket = ticketBL.FindTicketById(NewComment.TicketId);

                commentBL.CreateAndSaveComment(NewComment, ticket, user);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Project");
            }

            return RedirectToAction("CommentsForTask", new { ticketId = NewComment.TicketId });
        }
    }
}

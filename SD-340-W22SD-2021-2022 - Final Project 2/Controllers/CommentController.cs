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
    public class CommentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CommentBusinessLogic commentBL;
        private readonly UserBusinessLogic userBL;
        private readonly TicketBusinessLogic ticketBL;
        public CommentController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            commentBL = new CommentBusinessLogic(new CommentRepository(context), _userManager);
            ticketBL = new TicketBusinessLogic(new TicketRepository(context), _userManager);
            userBL = new UserBusinessLogic(_userManager);
        }

        public async Task<IActionResult> CommentsForTask(int ticketId)
        {
            ViewBag.ticketId = ticketId;
            CreateCommentViewModel vm;
            List<Comment>? comments;
            Comment newComment = new Comment();
                
            try
            {
                comments = commentBL.GetAllCommentsByTask(ticketId);
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

                ApplicationUser currentUser = await userBL.GetCurrentUserByNameAsync(User.Identity.Name);
                Ticket checkTicket = ticketBL.GetTicket(NewComment.TicketId);
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
                Ticket ticket = ticketBL.GetTicket(NewComment.TicketId);
                comment.TicketId = NewComment.TicketId;
                comment.Ticket = ticket;
                comment.Content = NewComment.Content;
                comment.User = user;
                comment.UserId = user.Id;
                commentBL.CreateComment(comment);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Project");
            }

            return RedirectToAction("CommentsForTask", new { ticketId = NewComment.TicketId });
        }
    }
}

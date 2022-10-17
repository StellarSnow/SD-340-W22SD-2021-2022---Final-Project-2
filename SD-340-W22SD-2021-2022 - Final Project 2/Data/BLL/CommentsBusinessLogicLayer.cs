using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.Data.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;
using SD_340_W22SD_2021_2022___Final_Project_2.Models.ViewModels;
using System.Security.Claims;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Data.BLL
{
    public class CommentsBusinessLogicLayer
    {
        public ApplicationDbContext _context { get; set; }
        private CommentRepository _commentRepository;
        private TicketRepository _ticketRepository;
        private UserManager<ApplicationUser> _userManager;

        public CommentsBusinessLogicLayer(ApplicationDbContext dbContext, CommentRepository commentRepository, TicketRepository ticketRepository, UserManager<ApplicationUser> userManager)
        {
            _context = dbContext;
            _commentRepository = commentRepository;
            _ticketRepository = ticketRepository;
            _userManager = userManager;
        }

        public async Task<CreateCommentBLLViewModel> CreateComment(ClaimsPrincipal User, Comment NewComment)
        {
            try
            {
                bool taskOwners = true;
                bool taskWatchers = true;

                ApplicationUser currentUser = await _userManager.GetUserAsync(User);

                Ticket checkTicket = _ticketRepository.GetTicketWithTaskOwnersAndTaskWatchers(NewComment.TicketId);

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
                    return new CreateCommentBLLViewModel()
                    {
                        Unauthorized = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new CreateCommentBLLViewModel()
                {
                    Succeeded = false
                };
            }
            Comment comment = new Comment();

            try
            {
                string userName = User.Identity.Name;

                ApplicationUser user = await _userManager.FindByNameAsync(userName);

                Ticket ticket = _ticketRepository.Get(NewComment.TicketId);

                comment.TicketId = NewComment.TicketId;
                comment.Ticket = ticket;
                comment.Content = NewComment.Content;
                comment.User = user;
                comment.UserId = user.Id;

                _commentRepository.Add(comment);

                _commentRepository.Save();
            }
            catch (Exception ex)
            {
                return new CreateCommentBLLViewModel()
                {
                    Succeeded = false
                };
            }

            return new CreateCommentBLLViewModel()
            {
                Succeeded = true
            };
        }
    }
}

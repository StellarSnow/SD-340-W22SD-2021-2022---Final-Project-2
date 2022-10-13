using SD_340_W22SD_2021_2022___Final_Project_2.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.BLL
{
    public class CommentBusinessLogic
    {
        IRepository<Comment> repo;

        public CommentBusinessLogic(IRepository<Comment> repo)
        {
            this.repo = repo;
        }

        public List<Comment> GetAllCommentsByTicket(int ticketId)
        {
            return repo.GetAll(comment => comment.TicketId == ticketId);
        }

        public void CreateAndSaveComment(Comment NewComment, Ticket ticket, ApplicationUser user)
        {
            Comment comment = new Comment();
            
            comment.TicketId = NewComment.TicketId;
            comment.Ticket = ticket;
            comment.Content = NewComment.Content;
            comment.User = user;
            comment.UserId = user.Id;

            repo.Add(comment);
            repo.Save();
        }
    }
}

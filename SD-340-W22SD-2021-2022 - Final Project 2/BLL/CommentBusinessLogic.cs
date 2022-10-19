using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_2021_2022___Final_Project_2.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.BLL
{
    public class CommentBusinessLogic
    {
        private IRepository<Comment> repo;
        private readonly UserManager<ApplicationUser> _userManager;
        public CommentBusinessLogic(IRepository<Comment> repo, UserManager<ApplicationUser> userManager)
        {
            this.repo = repo;
            _userManager = userManager;
        }

        // To UnitTest
        // Valid: Test method if returns a list of comments.
        // Invalid: If ticket count is still the same
        public List<Comment> GetAllCommentsByTask(int ticketId)
        {
            return repo.GetList(comment => comment.TicketId == ticketId).ToList();
        }

        public void CreateComment(Comment entity)
        {
            repo.Create(entity);
            repo.Save();
        }
    }
}

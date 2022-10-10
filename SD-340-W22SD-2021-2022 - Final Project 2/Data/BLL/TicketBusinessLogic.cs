using SD_340_W22SD_2021_2022___Final_Project_2.Data.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Data.BLL
{
    public class TicketBusinessLogic
    {
        private TicketRepository _repo;
        public TicketBusinessLogic(TicketRepository repo)
        {
            _repo = repo;
        }
        public ICollection<Comment> CommentsOnTask(int ticketID)
        {            
            return _repo.Get(ticketID).Comment.ToList();
        }


    }
}

using SD_340_W22SD_2021_2022___Final_Project_2.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.BLL
{
    public class TicketBusinessLogic
    {
        IRepository<Ticket> repo;

        public TicketBusinessLogic(IRepository<Ticket> repo)
        {
            this.repo = repo;
        }

        public void CreateAndSaveTicket(Ticket ticket, Priority priority, int projectId, string[] taskOwnerIds, ApplicationUser dev)
        {
            Ticket newTicket = new Ticket();
            newTicket.ProjectId = projectId;
            newTicket.Name = ticket.Name;
            newTicket.Hours = ticket.Hours;
            newTicket.Priority = priority;
            newTicket.Completed = false;
            newTicket.TaskOwners.Add(dev);

            repo.Add(newTicket);
            repo.Save();
        }

        public Ticket FindTicketById(int ticketId)
        {
           return repo.Get(ticketId);
        }

        public Ticket AddOrRemoveWatcher(Ticket ticket, ApplicationUser currentUser)
        {
            if (ticket.TaskWatchers.FirstOrDefault(u => u.Id == currentUser.Id) == null)
            {
                ticket.TaskWatchers.Add(currentUser);
            }
            else
            {
                ticket.TaskWatchers.Remove(currentUser);
            }

            return ticket;
        }

        public Ticket UpdateTicket(Ticket ticket)
        {
            return repo.Update(ticket);
        }

        public void SaveTicket()
        {
            repo.Save();
        }
    }
}

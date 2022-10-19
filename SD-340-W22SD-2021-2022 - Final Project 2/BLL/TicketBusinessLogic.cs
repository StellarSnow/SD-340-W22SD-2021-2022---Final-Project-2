using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_2021_2022___Final_Project_2.DAL;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.BLL
{
    public class TicketBusinessLogic
    {
        private IRepository<Ticket> repo;
        private readonly UserManager<ApplicationUser> _userManager;
        

        public TicketBusinessLogic(IRepository<Ticket> repo, 
            UserManager<ApplicationUser> userManager)
        {
            this.repo = repo;
            this._userManager = userManager;
            
        }

        // To UnitTest
        // Valid: Test if ticket count is incrementing
        // Invalid: If ticket count is still the same
        public void CreateTicket(Ticket ticket)
        {
            repo.Create(ticket);
            repo.Save();
        }
        // To UnitTest
        // Valid: Test if method returns the actual ticket with correct Id
        // Invalid: Test if method throws NullRefException if Invalid Id
        public Ticket GetTicket(int id)
        {
            try
            {
                return repo.Get(id);

            } catch
            {
                throw new NullReferenceException("Ticket not found");
            }
            
        }
        // To UnitTest
        // Valid: Test if ticket list is returning the correct count
        // Invalid:.
        public List<Ticket> GetTicketList(int projectId)
        {
            return repo.GetList(ticket => ticket.ProjectId == projectId).ToList();
        }
        // To UnitTest
        // Valid: Test if ticket list is returning all completed
        // Invalid: Test if ticket list is returning a 0 count
        public List<Ticket> GetCompletedTickets(int projectid)
        {
            List<Ticket> ticketsInProj = repo.GetList(project => project.ProjectId == projectid).ToList();
            return ticketsInProj.Where(ticket => ticket.Completed == true).ToList();
        }
        // To UnitTest
        // Valid: Test if ticket list is returning all Uncompleted
        // Invalid: Test if ticket list is returning a 0 count
        public List<Ticket> GetUncompletedTickets(int projectid)
        {
            List<Ticket> ticketsInProj = repo.GetList(project => project.ProjectId == projectid).ToList();
            return ticketsInProj.Where(ticket => ticket.Completed == false).ToList();
        }
        // To UnitTest
        // Valid: Test if ticket status is updating to the correct value(true/false).
        // Invalid: Ticket not found
        public void UpdateTicketStatus(Ticket entity)
        {
            if(entity != null)
            {
                entity.Completed = !entity.Completed;
                repo.Update(entity);
                repo.Save();
            } else
            {
                throw new NullReferenceException("Ticket not found");
            }
            
        }
        // To UnitTest
        // Valid: Test if ticket status is updating to the correct value(true/false).
        // Invalid: Hrs will not be on the range 1-999.
        public void UpdateTicketRequiredHours(Ticket entity, int hours)
        {
            if(hours <= 0 || hours >= 1000)
            {
                entity.Hours = hours;
                repo.Update(entity);
                repo.Save();
            } else
            {
                throw new ArgumentException("Invalid Hrs");
            }
            
        }
        // To UnitTest
        // Valid: If ticket watcher count increments.
        // Invalid:.
        public void UpdateTicketAddWatcher(Ticket entity, ApplicationUser user)
        {
            entity.TaskWatchers.Add(user);
            repo.Update(entity);
            repo.Save();
        }
        // To UnitTest
        // Valid: If ticket watcher count decrements.
        // Invalid:.
        public void UpdateTicketRemoveWatcher(Ticket entity, ApplicationUser user)
        {

            entity.TaskWatchers.Remove(user);
            repo.Update(entity);
            repo.Save();
        }

    }
}

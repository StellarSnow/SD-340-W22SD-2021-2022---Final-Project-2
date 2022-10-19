using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.Data;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.DAL
{
    public class TicketRepository : IRepository<Ticket>
    {
        private ApplicationDbContext _db;
        public TicketRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public void Create(Ticket entity)
        {
            _db.Ticket.Add(entity);
        }

        public void Delete(Ticket entity)
        {
            throw new NotImplementedException();
        }

        public Ticket Get(int id)
        {
            return _db.Ticket.Include(ticket => ticket.TaskOwners).Include(ticket => ticket.TaskWatchers).Include(ticket => ticket.Comment).First(ticket => ticket.Id == id);
        }

        public Ticket Get(Func<Ticket, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public ICollection<Ticket> GetAll()
        {
            return _db.Ticket.Include(ticket => ticket.TaskOwners).Include(ticket => ticket.TaskWatchers).Include(ticket => ticket.Comment).ToList();
        }

        public ICollection<Ticket> GetList(Func<Ticket, bool> predicate)
        {
            return _db.Ticket.Include(ticket => ticket.TaskOwners).Include(ticket => ticket.TaskWatchers).Include(ticket => ticket.Comment).Where(predicate).ToList();
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public Ticket Update(Ticket entity)
        {
            _db.Ticket.Update(entity);
            return entity;
        }
    }
}
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Data.DAL
{
    public class TicketRepository : IRepository<Ticket>
    {
        private ApplicationDbContext _db;
        public TicketRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public void Add(Ticket entity)
        {
            _db.Ticket.Add(entity);
        }

        public void Delete(Ticket entity)
        {
            _db.Ticket.Remove(entity);
        }

        public Ticket Get(int id)
        {
            return _db.Ticket.Find(id);
        }

        public Ticket Get(Func<Ticket, bool> predicate)
        {
            return _db.Ticket.First(predicate);
        }

        public ICollection<Ticket> GetAll()
        {
            return _db.Ticket.ToList();
        }

        public ICollection<Ticket> GetAll(Func<Ticket, bool> predicate)
        {
            return _db.Ticket.Where(predicate).ToList();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.Data;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.DAL
{
    public class ProjectRepository : IRepository<Project>
    {
        private ApplicationDbContext _db;
        public ProjectRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public void Create(Project entity)
        {
            _db.Project.Add(entity);
        }

        public void Delete(Project entity)
        {
            throw new NotImplementedException();
        }

        public Project Get(int id)
        {
            return _db.Project.Include(proj => proj.Developers).Include(proj => proj.Ticket).ThenInclude(ticket => ticket.TaskWatchers).First(proj => proj.Id == id);
        }

        public Project Get(Func<Project, bool> predicate)
        {
            return _db.Project.Include(proj => proj.Developers).Include(proj => proj.Ticket).ThenInclude(ticket => ticket.TaskWatchers).First(predicate);
        }

        public ICollection<Project> GetAll()
        {
            return _db.Project.Include(proj => proj.Developers).Include(proj => proj.Ticket).ToList();
        }

        public ICollection<Project> GetList(Func<Project, bool> predicate)
        {
            return _db.Project.Include(proj => proj.Developers).Include(proj => proj.Ticket).ThenInclude(ticket => ticket.TaskWatchers).Where(predicate).ToList();
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public Project Update(Project entity)
        {
            throw new NotImplementedException();
        }
    }
}

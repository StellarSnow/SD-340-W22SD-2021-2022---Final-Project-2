using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Data.DAL
{
    public class ProjectRepository : IRepository<Project>
    {
        private ApplicationDbContext _db;
        public ProjectRepository (ApplicationDbContext db)
        {
            _db = db;
        }
        public void Add(Project entity)
        {
            _db.Project.Add(entity);
        }

        public void Delete(Project entity)
        {
            _db.Project.Remove(entity);
        }

        public Project Get(int id)
        {
            return _db.Project.Find(id);
        }


        public Project Get(Func<Project, bool> predicate)
        {
            return _db.Project.FirstOrDefault(predicate);
        }

        public ICollection<Project> GetAll()
        {
            return _db.Project.ToList();
        }

        public ICollection<Project> GetAll(Func<Project, bool> predicate)
        {
            return _db.Project.Where(predicate).ToList();
        }

        public ICollection<ApplicationUser> GetAllUsers()
        {
            return _db.Users.ToList();
        }

        public ApplicationUser FindUser (int userId)
        {
            return _db.Users.Find(userId);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}

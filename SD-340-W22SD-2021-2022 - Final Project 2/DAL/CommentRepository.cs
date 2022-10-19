using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_2021_2022___Final_Project_2.Data;
using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.DAL
{
    public class CommentRepository : IRepository<Comment>
    {
        private ApplicationDbContext _db;
        public CommentRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public void Create(Comment entity)
        {
            _db.Comment.Add(entity);
            _db.SaveChanges();
        }

        public void Delete(Comment entity)
        {
            throw new NotImplementedException();
        }

        public Comment Get(int id)
        {
            throw new NotImplementedException();
        }

        public Comment Get(Func<Comment, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public ICollection<Comment> GetAll()
        {
            throw new NotImplementedException();
        }

        public ICollection<Comment> GetList(Func<Comment, bool> predicate)
        {
            return _db.Comment.Include(comment => comment.User).Where(predicate).ToList();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public Comment Update(Comment entity)
        {
            throw new NotImplementedException();
        }
    }
}

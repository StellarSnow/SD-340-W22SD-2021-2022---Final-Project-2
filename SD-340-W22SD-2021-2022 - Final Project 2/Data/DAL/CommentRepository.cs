using SD_340_W22SD_2021_2022___Final_Project_2.Models;

namespace SD_340_W22SD_2021_2022___Final_Project_2.Data.DAL
{
    public class CommentRepository : IRepository<Comment>
    {
        public ApplicationDbContext _context { get; set; }

        public CommentRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public void Add(Comment entity)
        {
            _context.Comment.Add(entity);
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

        public ICollection<Comment> GetAll(Func<Comment, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public void Update(Comment entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Comment entity)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}

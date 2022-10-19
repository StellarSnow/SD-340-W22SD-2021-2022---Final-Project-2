namespace SD_340_W22SD_2021_2022___Final_Project_2.DAL
{
    public interface IRepository<T> where T : class
    {
        //Create
        void Create(T entity);
        //Read
        T Get(int id);
        T Get(Func<T, bool> predicate);
        ICollection<T> GetAll();
        ICollection<T> GetList(Func<T, bool> predicate);
        //Update
        T Update(T entity);
        //Delete
        void Delete(T entity);
        //Save
        void Save();
    }
}

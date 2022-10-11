namespace SD_340_W22SD_2021_2022___Final_Project_2.DAL
{

    public interface IRepository<T> where T : class
    {
        void Add(T entity);

        T Get(int id);
        T Get(Func<T, bool> predicate);
        ICollection<T> GetAll();
        ICollection<T> GetList(Func<T, bool> predicate);
        T Update(T entity);

        void Delete(T entity);

        void Save();
    }
}

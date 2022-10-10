namespace SD_340_W22SD_2021_2022___Final_Project_2.Data.DAL
{
    public interface IRepository <T> where T : class
    {
        //create
        void Add(T entity);

        //read
        T Get(int id);
        T Get (Func<T, bool> predicate);
        ICollection<T> GetAll();
        ICollection<T> GetAll (Func<T, bool> predicate);    

        
        //delete
        void Delete(T entity);

        void Save();
    }
}

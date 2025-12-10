namespace ApiTalento.Web.Repositories
}
    }
        Task<int> SaveChangesAsync();
        Task<bool> ExistsAsync(int id);
        Task DeleteAsync(T entity);
        Task UpdateAsync(T entity);
        Task<T> AddAsync(T entity);
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
    {
    public interface IGenericRepository<T> where T : class
{


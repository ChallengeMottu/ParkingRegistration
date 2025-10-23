namespace PulseSystem.Infraestructure.Repositories.interfaces;

public interface IMethodsRepository<T> where T : class
{
    IQueryable<T> Query();
    Task<T?> GetByIdAsync(long id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
}
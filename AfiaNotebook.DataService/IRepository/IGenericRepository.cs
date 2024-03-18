namespace AfiaNotebook.DataService.IRepository;
public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    
    Task<T> GetById(Guid id);
    
    Task<bool> Add(T entity);
    
    Task<bool> Upsert(T entity); // update or add if doesn't exists
    
    Task<bool> Delete(Guid id, string userId); 
}

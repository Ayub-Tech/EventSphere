namespace EventSphere.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    // Hämta en post via ID – returnerar null om den inte finns
    Task<T?> GetByIdAsync(Guid id);

    // Hämta alla poster i tabellen
    Task<IEnumerable<T>> GetAllAsync();

    // Lägg till en ny post och returnera den sparade versionen
    Task<T> AddAsync(T entity);

    // Uppdatera en befintlig post
    Task UpdateAsync(T entity);

    // Soft-delete – markerar som borttagen, raderar inte fysiskt
    Task DeleteAsync(Guid id);
}
using EventSphere.Domain.Common;
using EventSphere.Domain.Interfaces;
using EventSphere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventSphere.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // Hämta en post via ID
    public async Task<T?> GetByIdAsync(Guid id)
        => await _dbSet.FindAsync(id);

    // Hämta alla poster (soft-deletade filtreras bort automatiskt)
    public async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    // Lägg till ny post och spara till databasen
    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    // Uppdatera befintlig post
    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    // Soft-delete – sätter IsDeleted = true istället för att radera
    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
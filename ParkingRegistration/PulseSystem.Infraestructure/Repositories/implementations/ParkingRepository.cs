using Microsoft.EntityFrameworkCore;
using PulseSystem.Domain.Entities;
using PulseSystem.Infraestructure.Persistence;
using PulseSystem.Infraestructure.Repositories.interfaces;

namespace PulseSystem.Infraestructure.Repositories.implementations;

public class ParkingRepository : IParkingRepository
{
    
    private readonly PulseSystemContext _context;
    private readonly DbSet<Parking> _dbSet;

    public ParkingRepository(PulseSystemContext context)
    {
        _context = context;
        _dbSet = context.Set<Parking>();
    }
    
    
    public IQueryable<Parking> Query() =>
        _dbSet
            .Include(p => p.Gateways)
            .Include(p => p.Zones)
            .AsNoTracking();


    public async Task<Parking?> GetByIdAsync(long id)
    {
        return await _dbSet
            .Include(p => p.Gateways)
            .Include(p => p.Zones)
            .FirstOrDefaultAsync(p => p.Id == id);
    }


    public async Task AddAsync(Parking entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Parking entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public Task RemoveAsync(Parking entity)
    {
        _dbSet.Remove(entity);
        return _context.SaveChangesAsync();
    }

    // ✅ Ordenar por capacidade do maior para o menor
    public async Task<IEnumerable<Parking>> GetAllOrderByCapacityDescAsync()
    {
        return await _dbSet
            .OrderByDescending(p => p.Capacity)
            .ToListAsync();
    }

    // ✅ Ordenar por área disponível do maior para o menor
    public async Task<IEnumerable<Parking>> GetAllOrderByAvailableAreaDescAsync()
    {
        return await _dbSet
            .OrderByDescending(p => p.AvailableArea)
            .ToListAsync();
    }

    // ✅ Buscar por endereço (comparação de cada campo)
    public async Task<Parking?> GetByLocationAsync(string street, string complement)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p =>
                p.Location.Street == street &&
                p.Location.Complement == complement
            );
    }
    
    public async Task<IEnumerable<Parking>> GetAllPaginatedAsync(int page, int pageSize)
    {
        return await _dbSet
            .OrderBy(p => p.Id) // sempre bom ordenar antes
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }


}
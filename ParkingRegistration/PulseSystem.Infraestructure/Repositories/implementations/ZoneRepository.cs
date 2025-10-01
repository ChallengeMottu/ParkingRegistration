using Microsoft.EntityFrameworkCore;
using PulseSystem.Domain.Entities;
using PulseSystem.Infraestructure.Persistence;
using PulseSystem.Infraestructure.Repositories.interfaces;

namespace PulseSystem.Infraestructure.Repositories.implementations;

public class ZoneRepository : IZoneRepository
{
    
    private readonly PulseSystemContext _context;
    private readonly DbSet<Zone> _dbSet;

    public ZoneRepository(PulseSystemContext context)
    {
        _context = context;
        _dbSet = context.Set<Zone>();
    }

    public IQueryable<Zone> Query() => _dbSet.AsNoTracking();

    public async Task<Zone?> GetByIdAsync(long id)
    {
        return await _dbSet.FindAsync(id);
    }


    public async Task AddAsync(Zone entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Zone entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
    
    public async Task<int> CountByParkingIdAsync(long parkingId)
    {
        return await _context.Zones
            .CountAsync(z => z.ParkingId == parkingId);
    }


    public Task RemoveAsync(Zone entity)
    {
        _dbSet.Remove(entity);
        return _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Zone>> GetByParkingIdAsync(long parkingId)
    {
        return await _dbSet
            .Where(z => z.ParkingId == parkingId)
            .ToListAsync();
    }
}
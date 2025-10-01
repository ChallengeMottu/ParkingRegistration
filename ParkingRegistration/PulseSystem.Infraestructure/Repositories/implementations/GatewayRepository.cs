using Microsoft.EntityFrameworkCore;
using PulseSystem.Domain.Entities;
using PulseSystem.Domain.Enums;
using PulseSystem.Infraestructure.Persistence;
using PulseSystem.Infraestructure.Repositories.interfaces;

namespace PulseSystem.Infraestructure.Repositories.implementations;

public class GatewayRepository : IGatewayRepository
{
    private readonly PulseSystemContext _context;
    private readonly DbSet<Gateway> _dbSet;

    public GatewayRepository(PulseSystemContext context)
    {
        _context = context;
        _dbSet = context.Set<Gateway>();
    }

    public async Task<Gateway?> GetByMacAddressAsync(string macAddress)
    {
        return await _dbSet.FirstOrDefaultAsync(g => g.MacAddress == macAddress);
    }

    public async Task<IEnumerable<Gateway>> GetAllByParkingId(long id)
    {
        return await _dbSet.Where(g => g.ParkingId == id).ToListAsync();
    }


    public IQueryable<Gateway> Query() => _dbSet.AsNoTracking();

    public async Task<Gateway?> GetByIdAsync(long id)
    {
        return await _dbSet
            .FindAsync(id);
    }

    public async Task AddAsync(Gateway entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Gateway entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Gateway entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PulseSystem.Infraestructure.Persistence;
using PulseSystem.Infraestructure.Repositories.implementations;
using PulseSystem.Infraestructure.Repositories.interfaces;

namespace PulseSystem.Infraestructure.Configuration;

public static class CollectionExtensions

{

    public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)

    {

        services.AddDbContext<PulseSystemContext>(options =>

            options.UseOracle(configuration.GetConnectionString("SystemPulse")));

        return services;

    }
 
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    
    {
    
        services.AddScoped<IParkingRepository, ParkingRepository>();
    
        services.AddScoped<IGatewayRepository, GatewayRepository>();
        
        services.AddScoped<IZoneRepository, ZoneRepository>();
    
        return services;
    
    }
 
    

}
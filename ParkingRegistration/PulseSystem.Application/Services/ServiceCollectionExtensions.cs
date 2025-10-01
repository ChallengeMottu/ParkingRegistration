using Microsoft.Extensions.DependencyInjection;
using PulseSystem.Application.Services.Implementations;
using PulseSystem.Application.Services.interfaces;
using PulseSystem.Domain.Entities;

namespace PulseSystem.Application.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IParkingService, ParkingService>();
        services.AddScoped<IGatewayService, GatewayService>();
        services.AddScoped<IZoneService, ZoneService>();
        return services;
    }
}
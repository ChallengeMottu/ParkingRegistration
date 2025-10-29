using Microsoft.Extensions.DependencyInjection;
using PulseSystem.Application.ML.Services;
using PulseSystem.Application.Services.Implementations;
using PulseSystem.Application.Services.Implementations.v2;
using PulseSystem.Application.Services.interfaces;
using PulseSystem.Application.Services.interfaces.v2;

namespace PulseSystem.Application.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IParkingService, ParkingService>();
        services.AddScoped<IGatewayService, GatewayService>();
        services.AddScoped<IZoneService, ZoneService>();
        services.AddScoped<IParkingServiceV2, ParkingServiceV2>();
        services.AddScoped<IGatewayServiceV2, GatewayServiceV2>();
        services.AddScoped<LoginService>();
        services.AddSingleton<IGatewayPredictionService, GatewayPredictionService>();
        return services;
    }
}
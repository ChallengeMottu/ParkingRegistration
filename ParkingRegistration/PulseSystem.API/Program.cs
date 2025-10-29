using PulseSystem.Application.configuration;
using PulseSystem.Application.Exceptions;
using PulseSystem.Application.Services;
using PulseSystem.Configuration;
using PulseSystem.Infraestructure.Configuration;

namespace PulseSystem;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddVersioning();
        builder.Services.AddSwaggerDocumentation();
        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", policy =>
            {
                policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddAppDbContext(builder.Configuration);
        builder.Services.AddRepositories();
        builder.Services.AddServices();
        builder.Services.AddAutoMapper(cfg => { }, typeof(MapperConfig));
        builder.Services.AddHealthChecksConfig(builder.Configuration);

        var app = builder.Build();

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseSwaggerDocumentation();
        app.UseHttpsRedirection();
        app.UseCors("AllowReactApp");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecksEndpoints();

        app.Run();
    }
}
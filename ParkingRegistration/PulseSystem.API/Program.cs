using Microsoft.EntityFrameworkCore;
using PulseSystem.Application.configuration;
using PulseSystem.Application.Services;
using PulseSystem.Application.Exceptions;
using PulseSystem.Infraestructure.Configuration;
using PulseSystem.Infraestructure.Persistence;

namespace PulseSystem;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Parking Registration",
                Version = "v1",
                Description = "Documentação da API REST Pulse do sistema de registro dos Pátios, Zonas e Gateways"
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
        
        builder.Services.AddControllers();
        builder.Services.AddAppDbContext(builder.Configuration);
        builder.Services.AddRepositories();
        

        builder.Services.AddServices();
        builder.Services.AddAutoMapper(cfg => {}, typeof(MapperConfig));

        var app = builder.Build();

        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Parking Registration v1");
        });


        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.MapControllers();

        app.Run();
    }
}
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PulseSystem.Application.configuration;
using PulseSystem.Application.Services;
using PulseSystem.Application.Exceptions;
using PulseSystem.Configuration;
using PulseSystem.Infraestructure.Configuration;

namespace PulseSystem;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();

        // Swagger com XML
        builder.Services.AddSwaggerGen(c =>
        {

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        builder.Services.AddControllers();
        builder.Services.AddAppDbContext(builder.Configuration);
        builder.Services.AddRepositories();
        builder.Services.AddServices();
        builder.Services.AddAutoMapper(cfg => { }, typeof(MapperConfig));

        builder.Services.AddSwaggerDocumentation();
        builder.Services.AddJwtAuthentication(builder.Configuration);

        var app = builder.Build();


        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Parking Registration v1"));
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();



    }
}
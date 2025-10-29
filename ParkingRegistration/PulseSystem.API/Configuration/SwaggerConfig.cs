using Microsoft.OpenApi.Models;

namespace PulseSystem.Configuration;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // V1
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Parking Registration API",
                Version = "v1",
                Description = "Documentação da versão 1 da API REST Pulse System"
            });

            // V2 (exemplo futuro)
            c.SwaggerDoc("v2", new OpenApiInfo
            {
                Title = "Parking Registration API",
                Version = "v2",
                Description = "Documentação da versão 2 da API REST Pulse System"
            });

            // XML (comentários de documentação)
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            // 🔐 JWT no Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Insira o token JWT no formato: Bearer {token}",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pulse System API v1");
            c.SwaggerEndpoint("/swagger/v2/swagger.json", "Pulse System API v2");
        });

        return app;
    }
}

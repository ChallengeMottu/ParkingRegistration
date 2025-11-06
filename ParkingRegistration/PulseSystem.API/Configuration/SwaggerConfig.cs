using Microsoft.OpenApi.Models;

namespace PulseSystem.Configuration;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Parking Registration API",
                Version = "v1",
                Description =
                    "### Parking Registration API – Versão 1\n" +
                    "\n" +
                    "API RESTful destinada ao gerenciamento de pátios, gateways e zonas.\n" +
                    "\n" +
                    "**Funcionalidades principais:**\n" +
                    "- CRUD completo das entidades *Parking*, *Gateway* e *Zone*\n" +
                    "- Validações robustas\n" +
                    "- Aplicação de boas práticas de arquitetura\n" +
                    "- Camada de negócios estruturada\n" +
                    "\n" +
                    "*Documentação da versão 1 da API REST Pulse System.*",
                Contact = new OpenApiContact
                {
                    Name = "Pulse",
                    Email = "pulsecontact@pulse.com"
                }
            });

            c.SwaggerDoc("v2", new OpenApiInfo
            {
                Title = "Parking Registration API",
                Version = "v2",
                Description =
                    "### Parking Registration API – Versão 2\n" +
                    "\n" +
                    "Evolução da API de registro de pátios, com melhorias nas entidades e na estrutura de negócios.\n" +
                    "\n" +
                    "**Principais aprimoramentos da v2:**\n" +
                    "- Respostas padronizadas\n" +
                    "- Documentação aprimorada\n" +
                    "- Melhor organização das rotas versionadas\n" +
                    "- Aprimoramento de funcionalidade usando ML .NET\n" +
                    "\n" +
                    "*Documentação da versão 2 da API REST Pulse System.*",
                Contact = new OpenApiContact
                {
                    Name = "Pulse",
                    Email = "pulsecontact@pulse.com"
                }
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Insira o token JWT no formato: **Bearer {seu_token}**",
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

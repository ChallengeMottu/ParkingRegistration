using Microsoft.AspNetCore.Http;
using System.Text.Json;
using PulseSystem.Application.Exceptions.Configuration;

namespace PulseSystem.Application.Exceptions
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode;
            string title;
            string detail = ex.Message;

            switch (ex)
            {
                case ResourceNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    title = "Recurso não encontrado";
                    break;
                case InvalidArgumentException:
                    statusCode = StatusCodes.Status400BadRequest;
                    title = "Parâmetros inválidos";
                    break;
                case BusinessRuleException:
                    statusCode = StatusCodes.Status422UnprocessableEntity;
                    title = "Regra de negócio violada";
                    break;
                case InvalidOperationException:
                    statusCode = StatusCodes.Status409Conflict;
                    title = "Conflito de operação";
                    break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    title = "Erro inesperado";
                    break;
            }

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path,
                Type = $"https://httpstatuses.com/{statusCode}"
            };

            var json = JsonSerializer.Serialize(problemDetails);

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(json);
        }

    }
}

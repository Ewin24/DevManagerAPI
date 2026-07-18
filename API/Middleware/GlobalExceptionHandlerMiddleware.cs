namespace API.Middleware;

using Application.Common.Models;
using System.Net;
using System.Text.Json;

/// <summary>
/// Middleware global para manejo centralizado de excepciones
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            TraceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case Application.Common.Exceptions.ApplicationException appEx:
                response.StatusCode = appEx.StatusCode;
                errorResponse.Message = appEx.Message;
                errorResponse.ErrorCode = appEx.ErrorCode;
                errorResponse.Errors = appEx.Errors;

                _logger.LogWarning(appEx,
                    "Business exception: {ErrorCode} - {Message}",
                    appEx.ErrorCode, appEx.Message);
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Acceso no autorizado";
                errorResponse.ErrorCode = "UNAUTHORIZED";

                _logger.LogWarning(exception, "Unauthorized access attempt");
                break;

            case ArgumentNullException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = $"Parámetro requerido: {argEx.ParamName}";
                errorResponse.ErrorCode = "INVALID_ARGUMENT";

                _logger.LogWarning(argEx, "Invalid argument: {ParamName}", argEx.ParamName);
                break;

            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = argEx.Message;
                errorResponse.ErrorCode = "INVALID_ARGUMENT";

                _logger.LogWarning(argEx, "Invalid argument: {Message}", argEx.Message);
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "Ha ocurrido un error interno en el servidor";
                errorResponse.ErrorCode = "INTERNAL_ERROR";

                _logger.LogError(exception,
                    "Unhandled exception: {Message}", exception.Message);
                break;
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var result = JsonSerializer.Serialize(errorResponse, jsonOptions);
        await response.WriteAsync(result);
    }
}

/// <summary>
/// Extensión para registrar el middleware
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}

namespace Application.Common.Models;

/// <summary>
/// Respuesta estandarizada de la API
/// </summary>
/// <typeparam name="T">Tipo de datos de respuesta</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResponse(T data, string message = "Operación exitosa")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> SuccessResponse(string message)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message
        };
    }
}

/// <summary>
/// Respuesta estandarizada de error
/// </summary>
public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }
}

namespace Application.Common.Exceptions;

/// <summary>
/// Excepción base para todas las excepciones de negocio de la aplicación
/// </summary>
public abstract class ApplicationException : Exception
{
    public int StatusCode { get; protected set; }
    public string ErrorCode { get; protected set; }
    public Dictionary<string, string[]>? Errors { get; protected set; }

    protected ApplicationException(string message, int statusCode = 400, string errorCode = "APP_ERROR")
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Excepción cuando una entidad no es encontrada
/// </summary>
public class NotFoundException : ApplicationException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} con identificador '{key}' no fue encontrado", 404, "NOT_FOUND")
    {
    }

    public NotFoundException(string message)
        : base(message, 404, "NOT_FOUND")
    {
    }
}

/// <summary>
/// Excepción cuando hay un conflicto (duplicados, restricciones, etc.)
/// </summary>
public class ConflictException : ApplicationException
{
    public ConflictException(string message)
        : base(message, 409, "CONFLICT")
    {
    }
}

/// <summary>
/// Excepción cuando las credenciales son inválidas
/// </summary>
public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(string message = "Credenciales inválidas")
        : base(message, 401, "UNAUTHORIZED")
    {
    }
}

/// <summary>
/// Excepción cuando no se tiene permiso para realizar una acción
/// </summary>
public class ForbiddenException : ApplicationException
{
    public ForbiddenException(string message = "No tienes permisos para realizar esta acción")
        : base(message, 403, "FORBIDDEN")
    {
    }
}

/// <summary>
/// Excepción cuando hay errores de validación de negocio
/// </summary>
public class BusinessValidationException : ApplicationException
{
    public BusinessValidationException(string message)
        : base(message, 400, "VALIDATION_ERROR")
    {
    }

    public BusinessValidationException(string message, Dictionary<string, string[]> errors)
        : base(message, 400, "VALIDATION_ERROR")
    {
        Errors = errors;
    }
}

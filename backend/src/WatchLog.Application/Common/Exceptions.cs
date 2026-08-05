namespace WatchLog.Application.Common;

/// <summary>Thrown when a requested entity doesn't exist. Mapped to HTTP 404 by the API's exception middleware.</summary>
public class NotFoundException(string entity, object key)
    : Exception($"{entity} with id '{key}' was not found.");

/// <summary>Thrown for business-rule violations. Mapped to HTTP 409.</summary>
public class ConflictException(string message) : Exception(message);

/// <summary>Thrown when the caller is authenticated but not allowed to perform the action. Mapped to HTTP 403.</summary>
public class ForbiddenException(string message = "You do not have permission to perform this action.")
    : Exception(message);

/// <summary>Thrown for FluentValidation failures. Mapped to HTTP 400 with a field->errors dictionary.</summary>
public class AppValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public AppValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}

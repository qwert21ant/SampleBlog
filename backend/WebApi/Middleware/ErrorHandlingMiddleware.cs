using System.Net;
using System.Text.Json;
using WebApi.Exceptions;

namespace WebApi.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred. Path: {Path}, Method: {Method}", 
                context.Request.Path, context.Request.Method);
            
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new ErrorResponse();

        switch (exception)
        {
            case UserNotFoundException:
            case PostNotFoundException:
                response.Message = "Resource not found";
                response.Details = exception.Message;
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            
            case EmailAlreadyExistsException:
                response.Message = "Conflict";
                response.Details = exception.Message;
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                break;
            
            case InvalidCredentialsException:
            case InvalidTokenException:
                response.Message = "Authentication failed";
                response.Details = exception.Message;
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;
            
            case UnauthorizedOperationException:
                response.Message = "Forbidden";
                response.Details = exception.Message;
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                break;
            
            case ArgumentException:
                response.Message = "Invalid request parameters";
                response.Details = exception.Message;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            
            case UnauthorizedAccessException:
                response.Message = "Unauthorized access";
                response.Details = exception.Message;
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;
            
            case KeyNotFoundException:
                response.Message = "Resource not found";
                response.Details = exception.Message;
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            
            case InvalidOperationException:
                response.Message = "Invalid operation";
                response.Details = exception.Message;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            
            default:
                response.Message = "An error occurred while processing your request";
                response.Details = "Please try again later";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        response.StatusCode = context.Response.StatusCode;
        response.Path = context.Request.Path;
        response.Method = context.Request.Method;
        response.Timestamp = DateTime.UtcNow;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
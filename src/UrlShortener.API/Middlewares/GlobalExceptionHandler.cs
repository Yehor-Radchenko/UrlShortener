using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using UrlShortener.Common.Exceptions;

namespace UrlShortener.API.Middlewares;

public class GlobalExceptionHandler(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            SetHttpResponse(httpContext, ex);
            await HandleHttpAsync(httpContext, ex);
        }
    }

    private static void SetHttpResponse(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = ex switch
        {
            ArgumentNullException or InvalidOperationException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            AuthenticationFailedException => (int)HttpStatusCode.Unauthorized,
            EntityNotFoundException => (int)HttpStatusCode.NotFound,
            ConflictException => (int)HttpStatusCode.Conflict,
            DbUpdateException => (int)HttpStatusCode.InternalServerError,
            _ => (int)HttpStatusCode.InternalServerError,
        };
    }

    private static async Task HandleHttpAsync(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            StatusCode = httpContext.Response.StatusCode,
            Message = ex.Message,
        }));
    }
}
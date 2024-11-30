using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UrlShortener.API.Middlewares;

public class ValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<ControllerActionDescriptor>() != null
            && context.Request.HttpContext.Items["ModelState"] is ModelStateDictionary modelState
            && !modelState.IsValid)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(modelState);
            return;
        }

        await next(context);
    }
}
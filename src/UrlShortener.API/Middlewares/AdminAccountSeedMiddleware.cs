using Microsoft.AspNetCore.Identity;
using UrlShortener.DAL.Context;
using UrlShortener.DAL.Entities;

namespace UrlShortener.API.Middlewares;

public class RoleInitializerMiddlwere(
    UrlShortenerDbContext context,
    UserManager<User> userManager,
    RoleManager<IdentityRole<int>> roleManager,
    IConfiguration configuration,
    ILogger<RoleInitializerMiddlwere> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await InitializeAsync(configuration);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
        finally
        {
            await next(context);
        }
    }

    public async Task InitializeAsync(IConfiguration configuration)
    {
        if (await roleManager.FindByNameAsync("Admin") == null)
        {
            await roleManager.CreateAsync(new IdentityRole<int>("Admin"));
        }

        if (await userManager.FindByEmailAsync(configuration["AdminDataSeed:DefaultEmail"]) == null && !context.Users!.Any())
        {
            var admin = new User
            {
                Email = configuration["AdminDataSeed:DefaultEmail"],
                UserName = configuration["AdminDataSeed:DefaultUsername"],
                FirstName = configuration["AdminDataSeed:DefaultFirstName"],
                LastName = configuration["AdminDataSeed:DefaultLastName"]
            };

            var result = await userManager.CreateAsync(admin, configuration["AdminSettings:DefaultPass"]);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}

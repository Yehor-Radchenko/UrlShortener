using Microsoft.AspNetCore.Identity;
using UrlShortener.DAL.Context;
using UrlShortener.DAL.Entities;

namespace UrlShortener.API.Middlewares;

public class AdminAccountSeedMiddleware(
    UrlShortenerDbContext context,
    UserManager<User> userManager,
    RoleManager<IdentityRole<int>> roleManager,
    IConfiguration configuration,
    ILogger<AdminAccountSeedMiddleware> logger) : IMiddleware
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

        if (await roleManager.FindByNameAsync("User") == null)
        {
            await roleManager.CreateAsync(new IdentityRole<int>("User"));
        }

        if (await userManager.FindByEmailAsync(configuration["AdminDataSeed:Email"]) == null && !context.Users!.Any())
        {
            var admin = new User
            {
                Email = configuration["AdminDataSeed:Email"],
                UserName = configuration["AdminDataSeed:Username"],
                FirstName = configuration["AdminDataSeed:FirstName"],
                LastName = configuration["AdminDataSeed:LastName"]
            };

            var result = await userManager.CreateAsync(admin, configuration["AdminDataSeed:Password"]);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}

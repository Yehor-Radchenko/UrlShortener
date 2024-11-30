using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.DAL.Context;
using UrlShortener.DAL.Entities;
using UrlShortener.DAL.Repository;
using UrlShortener.DAL.UoW;

namespace UrlShortener.DAL.Extentions;

public static class ServiceCollectionExtention
{
    public static IServiceCollection AddDalServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UrlShortenerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found.")));

        services.ConfigureIdentity();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IGenericRepository<Url>, GenericRepository<Url>>();

        return services;
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole<int>>(opts =>
        {
            opts.Password.RequireLowercase = true;
            opts.Password.RequireUppercase = false;
            opts.Password.RequireNonAlphanumeric = false;
            opts.Password.RequireDigit = true;
            opts.SignIn.RequireConfirmedAccount = false;
            opts.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<UrlShortenerDbContext>()
        .AddDefaultTokenProviders();
    }
}

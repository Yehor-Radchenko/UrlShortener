using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortener.DAL.Entities;

namespace UrlShortener.DAL.Context;

public class UrlShortenerDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    protected UrlShortenerDbContext()
    {
    }

    public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options) : base(options)
    {
    }

    public override DbSet<User> Users { get; set; }

    public DbSet<Url> URLs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Url>()
            .HasIndex(url => url.FullUrl)
            .IsUnique();

        builder.Entity<Url>()
            .HasIndex(url => url.ShortUrl)
            .IsUnique();
    }
}

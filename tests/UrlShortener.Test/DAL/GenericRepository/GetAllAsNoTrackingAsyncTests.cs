using System.Linq.Expressions;
using MockQueryable.Moq;
using Moq;
using UrlShortener.DAL.Context;
using UrlShortener.DAL.Entities;
using UrlShortener.DAL.Repository;

namespace UrlShortener.Test.DAL.GenericRepository;

public sealed class GetAllAsNoTrackingAsyncTests
{
    [Fact]
    public async Task GetAllAsNoTrackingAsync_ShouldReturnAllEntities_WhenNoFilterIsGiven()
    {
        // Arrange
        var mockContext = new Mock<UrlShortenerDbContext>();
        var urls = new List<Url>
        {
            new Url { Id = 1, FullUrl = "https://example1.com", ShortUrl = "abc123", CreatedDate = DateTime.Now, UserId = 1 },
            new Url { Id = 2, FullUrl = "https://example2.com", ShortUrl = "def456", CreatedDate = DateTime.Now, UserId = 1 },
            new Url { Id = 3, FullUrl = "https://example3.com", ShortUrl = "ghi789", CreatedDate = DateTime.Now, UserId = 2 },
        };
        var mockDbSet = urls.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.Set<Url>()).Returns(mockDbSet.Object);

        var repository = new GenericRepository<Url>(mockContext.Object);

        // Act
        var result = await repository.GetAllAsNoTrackingAsync();

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetAllAsNoTrackingAsync_ShouldApplyFilter_WhenFilterIsGiven()
    {
        // Arrange
        var mockContext = new Mock<UrlShortenerDbContext>();
        var urls = new List<Url>
        {
            new Url { Id = 1, FullUrl = "https://example1.com", ShortUrl = "abc123", CreatedDate = DateTime.Now, UserId = 1 },
            new Url { Id = 2, FullUrl = "https://example2.com", ShortUrl = "def456", CreatedDate = DateTime.Now, UserId = 1 },
            new Url { Id = 3, FullUrl = "https://example3.com", ShortUrl = "ghi789", CreatedDate = DateTime.Now, UserId = 2 },
        };
        var mockDbSet = urls.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.Set<Url>()).Returns(mockDbSet.Object);

        var repository = new GenericRepository<Url>(mockContext.Object);
        Expression<Func<Url, bool>> filter = u => u.UserId == 1;

        // Act
        var result = await repository.GetAllAsNoTrackingAsync(filter);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, u => Assert.Equal(1, u.UserId));
    }

    [Fact]
    public async Task GetAllAsNoTrackingAsync_ShouldIncludeEntities_WhenIncludeOptionsAreGiven()
    {
        // Arrange
        var mockContext = new Mock<UrlShortenerDbContext>();
        var urls = new List<Url>
        {
            new Url { Id = 1, FullUrl = "https://example1.com", ShortUrl = "abc123", CreatedDate = DateTime.Now, UserId = 1, User = new User { Id = 1, Email = "User1" } },
            new Url { Id = 2, FullUrl = "https://example2.com", ShortUrl = "def456", CreatedDate = DateTime.Now, UserId = 1, User = new User { Id = 1, Email = "User1" } },
            new Url { Id = 3, FullUrl = "https://example3.com", ShortUrl = "ghi789", CreatedDate = DateTime.Now, UserId = 2, User = new User { Id = 2, Email = "User2" } },
        };
        var mockDbSet = urls.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.Set<Url>()).Returns(mockDbSet.Object);

        var repository = new GenericRepository<Url>(mockContext.Object);
        string[] includeOptions = [nameof(Url.User)];

        // Act
        var result = await repository.GetAllAsNoTrackingAsync(null, includeOptions);

        // Assert
        Assert.Equal(3, result.Count());
        Assert.All(result, u => Assert.NotNull(u.User));
        Assert.Contains(result, u => u.User.Email == "User1");
        Assert.Contains(result, u => u.User.Email == "User2");
    }
}

using System.Linq.Expressions;
using MockQueryable.Moq;
using Moq;
using UrlShortener.DAL.Context;
using UrlShortener.DAL.Entities;
using UrlShortener.DAL.Repository;

namespace UrlShortener.Test.DAL.GenericRepository;

public sealed class GetAllAsyncTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities_WhenNoFilterIsGiven()
    {
        // Arrange
        var mockContext = new Mock<UrlShortenerDbContext>();
        var urls = new List<Url>
        {
            new Url { Id = 1, FullUrl = "https://example1.com", ShortUrl = "abc123", CreatedDate = DateTime.Now, UserId = 1, User = new User { Id = 1, Email = "User1" } },
            new Url { Id = 2, FullUrl = "https://example2.com", ShortUrl = "def456", CreatedDate = DateTime.Now, UserId = 2, User = new User { Id = 2, Email = "User2" } },
        };
        var mockDbSet = urls.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.Set<Url>()).Returns(mockDbSet.Object);

        var repository = new GenericRepository<Url>(mockContext.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ShouldApplyFilter_WhenFilterIsGiven()
    {
        // Arrange
        var mockContext = new Mock<UrlShortenerDbContext>();
        var urls = new List<Url>
        {
            new Url { Id = 1, FullUrl = "https://example1.com", ShortUrl = "abc123", CreatedDate = DateTime.Now, UserId = 1 },
            new Url { Id = 2, FullUrl = "https://example2.com", ShortUrl = "def456", CreatedDate = DateTime.Now, UserId = 2 },
        };
        var mockDbSet = urls.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.Set<Url>()).Returns(mockDbSet.Object);

        var repository = new GenericRepository<Url>(mockContext.Object);
        Expression<Func<Url, bool>> filter = u => u.UserId == 1;

        // Act
        var result = await repository.GetAllAsync(filter);

        // Assert
        Assert.Single(result);
        Assert.Equal("https://example1.com", result.First().FullUrl);
    }

    [Fact]
    public async Task GetAllAsync_ShouldIncludeEntities_WhenIncludeOptionsAreGiven()
    {
        // Arrange
        var mockContext = new Mock<UrlShortenerDbContext>();
        var urls = new List<Url>
        {
            new Url { Id = 1, FullUrl = "https://example1.com", ShortUrl = "abc123", CreatedDate = DateTime.Now, UserId = 1, User = new User { Id = 1, Email = "User1" } },
            new Url { Id = 2, FullUrl = "https://example2.com", ShortUrl = "def456", CreatedDate = DateTime.Now, UserId = 2, User = new User { Id = 2, Email = "User2" } },
        };
        var mockDbSet = urls.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.Set<Url>()).Returns(mockDbSet.Object);

        var repository = new GenericRepository<Url>(mockContext.Object);
        string[] includeOptions = [nameof(Url.User)];

        // Act
        var result = await repository.GetAllAsync(null, includeOptions);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, url => Assert.NotNull(url.User));
        Assert.Contains(result, url => url.User.Email == "User1");
        Assert.Contains(result, url => url.User.Email == "User2");
    }
}
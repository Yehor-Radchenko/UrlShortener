using System.Linq.Expressions;
using MockQueryable.Moq;
using Moq;
using UrlShortener.DAL.Context;
using UrlShortener.DAL.Entities;
using UrlShortener.DAL.Repository;

namespace UrlShortener.Test.DAL.GenericRepository;

public class GetAsyncTests
{
    [Fact]
    public async Task GetAsync_ReturnsEntity_WhenEntityExists()
    {
        // Arrange
        var urlId = 1;
        var mockContext = new Mock<UrlShortenerDbContext>();
        var urls = new List<Url>
        {
            new Url { Id = urlId, FullUrl = "https://example1.com", ShortUrl = "abc123", CreatedDate = DateTime.Now, UserId = 1 },
            new Url { Id = 2, FullUrl = "https://example2.com", ShortUrl = "def456", CreatedDate = DateTime.Now, UserId = 2 },
        };

        var mockDbSet = urls.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.Set<Url>()).Returns(mockDbSet.Object);

        var repository = new GenericRepository<Url>(mockContext.Object);
        Expression<Func<Url, bool>> filter = u => u.Id == urlId;

        // Act
        var result = await repository.GetAsync(filter);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("https://example1.com", result.FullUrl);
        Assert.Equal("abc123", result.ShortUrl);
    }

    [Fact]
    public async Task GetAsync_IncludesRelatedData_WhenSpecified()
    {
        // Arrange
        var urlId = 1;
        var mockContext = new Mock<UrlShortenerDbContext>();
        var urls = new List<Url>
        {
            new Url
            {
                Id = urlId,
                FullUrl = "https://example1.com",
                ShortUrl = "abc123",
                CreatedDate = DateTime.Now,
                UserId = 1,
                User = new User { Id = 1, Email = "User1" }
            },
        };

        var mockDbSet = urls.AsQueryable().BuildMockDbSet();
        mockContext.Setup(c => c.Set<Url>()).Returns(mockDbSet.Object);

        var repository = new GenericRepository<Url>(mockContext.Object);
        Expression<Func<Url, bool>> filter = u => u.Id == urlId;
        string[] includes = [nameof(Url.User)];

        // Act
        var result = await repository.GetAsync(filter, includes);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.User);
        Assert.Equal("User1", result.User.Email);
    }
}
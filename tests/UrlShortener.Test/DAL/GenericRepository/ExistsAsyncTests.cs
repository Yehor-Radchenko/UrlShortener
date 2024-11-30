using MockQueryable.Moq;
using Moq;
using UrlShortener.DAL.Context;
using UrlShortener.DAL.Entities;
using UrlShortener.DAL.Repository;

namespace UrlShortener.Test.DAL.GenericRepository;
public sealed class ExistsAsyncTests
{
    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_IfEntityExists()
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
        var shortUrlToFind = "abc123";

        // Act
        var exists = await repository.ExistsAsync(u => u.ShortUrl == shortUrlToFind);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_IfEntityDoesNotExist()
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
        var shortUrlToFind = "xyz999";

        // Act
        var exists = await repository.ExistsAsync(u => u.ShortUrl == shortUrlToFind);

        // Assert
        Assert.False(exists);
    }
}

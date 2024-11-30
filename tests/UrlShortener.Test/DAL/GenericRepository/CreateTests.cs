using MockQueryable.Moq;
using Moq;
using UrlShortener.DAL.Context;
using UrlShortener.DAL.Entities;
using UrlShortener.DAL.Repository;

namespace UrlShortener.Test.DAL.GenericRepository;

public sealed class CreateTests
{
    [Fact]
    public void Create_ShouldAddEntity()
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

        mockDbSet.Setup(m => m.Add(It.IsAny<Url>())).Callback<Url>(urls.Add);

        var repository = new GenericRepository<Url>(mockContext.Object);
        var newUrl = new Url
        {
            FullUrl = "https://newexample.com",
            ShortUrl = "new123",
            CreatedDate = DateTime.Now,
            UserId = 3
        };

        var urlsCountBeforeCreate = urls.Count;

        // Act
        repository.Create(newUrl);

        // Assert
        mockDbSet.Verify(u => u.Add(newUrl), Times.Once);
        Assert.Equal(urlsCountBeforeCreate + 1, urls.Count);
    }
}
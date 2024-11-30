using MockQueryable.Moq;
using Moq;
using UrlShortener.DAL.Context;
using UrlShortener.DAL.Entities;
using UrlShortener.DAL.Repository;

namespace UrlShortener.Test.DAL.GenericRepository;

public sealed class DeleteTests
{
    [Fact]
    public void Delete_ShouldRemoveEntity()
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

        mockDbSet.Setup(m => m.Remove(It.IsAny<Url>())).Callback<Url>(u => urls.Remove(u));

        var repository = new GenericRepository<Url>(mockContext.Object);
        var urlToDelete = urls[1];

        // Act
        repository.Delete(urlToDelete);

        // Assert
        mockDbSet.Verify(m => m.Remove(urlToDelete), Times.Once);
        Assert.Equal(2, urls.Count);
        Assert.DoesNotContain(urlToDelete, urls);
    }

}
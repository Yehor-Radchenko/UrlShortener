using System.Linq.Expressions;
using Moq;
using UrlShortener.DAL.Entities;
using UrlShortener.DAL.Repository;
using UrlShortener.DAL.UoW;

namespace UrlShortener.Test.BLL.Services.UrlService;

public class UrlServiceDeleteTests
{
    private readonly Mock<IGenericRepository<Url>> _mockUrlRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly UrlShortener.BLL.Services.UrlService _urlService;

    public UrlServiceDeleteTests()
    {
        _mockUrlRepo = new Mock<IGenericRepository<Url>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _urlService = new UrlShortener.BLL.Services.UrlService(
            null,
            _mockUrlRepo.Object,
            _mockUnitOfWork.Object,
            null);
    }

    [Fact]
    public async Task DeleteAsync_ExistingUrl_DeletesSuccessfully()
    {
        // Arrange
        int id = 1;
        int userId = 1;
        var url = new Url { Id = id, UserId = userId };

        _mockUrlRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Url, bool>>>(), null))
            .ReturnsAsync(url);

        // Act
        await _urlService.DeleteAsync(id, userId);

        // Assert
        _mockUrlRepo.Verify(r => r.Delete(url), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
using System.Linq.Expressions;
using Moq;
using UrlShortener.DAL.Entities;
using UrlShortener.DAL.Repository;
using UrlShortener.DAL.UoW;

namespace UrlShortener.Test.BLL.Services.UrlService;

public class UpdateUrlAppealDataTests
{
    private readonly Mock<IGenericRepository<Url>> _mockUrlRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly UrlShortener.BLL.Services.UrlService _urlService;

    public UpdateUrlAppealDataTests()
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
    public async Task UpdateUrlAppealData_ExistingUrl_UpdatesCorrectly()
    {
        // Arrange
        int urlId = 1;
        var initialDateTime = new DateTime(2023, 1, 1);
        var url = new Url { Id = urlId, NumberOfAppeals = 5, LastAppeal = initialDateTime };

        _mockUrlRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Url, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync(url);

        // Act
        await _urlService.UpdateUrlAppealData(urlId);

        // Assert
        Assert.Equal(6, url.NumberOfAppeals);
        Assert.True(url.LastAppeal > initialDateTime);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateUrlAppealData_NonExistentUrl_DoesNotUpdateOrSave()
    {
        // Arrange
        int urlId = 1;

        _mockUrlRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Url, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync((Url)null);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _urlService.UpdateUrlAppealData(urlId));
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateUrlAppealData_CorrectlyAppliesFilter()
    {
        // Arrange
        int urlId = 1;
        Expression<Func<Url, bool>> capturedFilter = null;

        _mockUrlRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Url, bool>>>(), It.IsAny<string[]>()))
            .Callback<Expression<Func<Url, bool>>, string[]>((filter, _) => capturedFilter = filter)
            .ReturnsAsync(new Url());

        // Act
        await _urlService.UpdateUrlAppealData(urlId);

        // Assert
        Assert.NotNull(capturedFilter);
        var func = capturedFilter.Compile();
        Assert.True(func(new Url { Id = urlId }));
        Assert.False(func(new Url { Id = urlId + 1 }));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task UpdateUrlAppealData_InvalidUrlId_ThrowsException(int urlId)
    {
        // Arrange
        _mockUrlRepo.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Url, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync((Url)null);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _urlService.UpdateUrlAppealData(urlId));
        _mockUrlRepo.Verify(r => r.GetAsync(It.IsAny<Expression<Func<Url, bool>>>(), It.IsAny<string[]>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
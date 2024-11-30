using System.Linq.Expressions;
using AutoMapper;
using Moq;
using UrlShortener.Common.ResponseModels.Url;
using UrlShortener.DAL.Entities;
using UrlShortener.DAL.Repository;

namespace UrlShortener.Test.BLL.Services.UrlService;

public class GetByShortedUrlTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IGenericRepository<Url>> _mockUrlRepo;
    private readonly UrlShortener.BLL.Services.UrlService _urlService;

    public GetByShortedUrlTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Url, UrlViewModel>();
        });
        _mapper = configuration.CreateMapper();

        _mockUrlRepo = new Mock<IGenericRepository<Url>>();

        _urlService = new UrlShortener.BLL.Services.UrlService(
            _mapper,
            _mockUrlRepo.Object,
            null,
            null);
    }

    [Fact]
    public async Task GetByShortedUrlAsync_ExistingUrl_ReturnsCorrectViewModel()
    {
        // Arrange
        string shortedUrl = "abc123";
        int userId = 1;
        var url = new Url { Id = 1, FullUrl = "https://example.com", ShortUrl = shortedUrl, UserId = userId };

        _mockUrlRepo.Setup(r => r.GetAsNoTrackingAsync(It.IsAny<Expression<Func<Url, bool>>>(), null))
            .ReturnsAsync(url);

        // Act
        var result = await _urlService.GetByShortedUrlAsync(shortedUrl, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(url.FullUrl, result.FullUrl);
        Assert.Equal(url.ShortUrl, result.ShortUrl);
    }

    [Fact]
    public async Task GetByShortedUrlAsync_NonExistentUrl_ReturnsNull()
    {
        // Arrange
        string shortedUrl = "nonexistent";
        int userId = 1;

        _mockUrlRepo.Setup(r => r.GetAsNoTrackingAsync(It.IsAny<Expression<Func<Url, bool>>>(), null))
            .ReturnsAsync((Url)null);

        // Act
        var result = await _urlService.GetByShortedUrlAsync(shortedUrl, userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByShortedUrlAsync_CorrectlyAppliesFilter()
    {
        // Arrange
        string shortedUrl = "abc123";
        int userId = 1;
        Expression<Func<Url, bool>> capturedFilter = null;

        _mockUrlRepo.Setup(r => r.GetAsNoTrackingAsync(It.IsAny<Expression<Func<Url, bool>>>(), It.IsAny<string[]>()))
            .Callback<Expression<Func<Url, bool>>, string[]>((filter, _) => capturedFilter = filter)
            .ReturnsAsync(new Url());

        // Act
        await _urlService.GetByShortedUrlAsync(shortedUrl, userId);

        // Assert
        Assert.NotNull(capturedFilter);
        var func = capturedFilter.Compile();
        Assert.True(func(new Url { ShortUrl = shortedUrl }));
        Assert.False(func(new Url { ShortUrl = "different" }));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetByShortedUrlAsync_InvalidShortedUrl_ReturnsNull(string shortedUrl)
    {
        // Arrange
        int userId = 1;

        // Act
        var result = await _urlService.GetByShortedUrlAsync(shortedUrl, userId);

        // Assert
        Assert.Null(result);
        _mockUrlRepo.Verify(r => r.GetAsNoTrackingAsync(It.IsAny<Expression<Func<Url, bool>>>(), null), Times.Once);
    }

    [Fact]
    public async Task GetByShortedUrlAsync_DoesNotUseUserId()
    {
        // Arrange
        string shortedUrl = "abc123";
        int userId = 1;
        Expression<Func<Url, bool>> capturedFilter = null;

        _mockUrlRepo.Setup(r => r.GetAsNoTrackingAsync(It.IsAny<Expression<Func<Url, bool>>>(), It.IsAny<string[]>()))
            .Callback<Expression<Func<Url, bool>>, string[]>((filter, _) => capturedFilter = filter)
            .ReturnsAsync(new Url());

        // Act
        await _urlService.GetByShortedUrlAsync(shortedUrl, userId);

        // Assert
        Assert.NotNull(capturedFilter);
        var func = capturedFilter.Compile();
        Assert.True(func(new Url { ShortUrl = shortedUrl, UserId = userId }));
        Assert.True(func(new Url { ShortUrl = shortedUrl, UserId = userId + 1 }));
    }
}
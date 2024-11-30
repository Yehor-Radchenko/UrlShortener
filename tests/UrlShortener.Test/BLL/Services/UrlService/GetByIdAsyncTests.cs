using System.Linq.Expressions;
using AutoMapper;
using Moq;
using UrlShortener.BLL.Interfaces;
using UrlShortener.Common.ResponseModels.Url;
using UrlShortener.DAL.Entities;
using UrlShortener.DAL.Repository;

namespace UrlShortener.Test.BLL.Services.UrlService;

public class UrlServiceGetByIdTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IGenericRepository<Url>> _mockUrlRepo;
    private readonly Mock<IUrlCastService> _mockUrlCastService;
    private readonly UrlShortener.BLL.Services.UrlService _urlService;

    public UrlServiceGetByIdTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Url, UrlViewModel>();
        });
        _mapper = configuration.CreateMapper();

        _mockUrlRepo = new Mock<IGenericRepository<Url>>();
        _mockUrlCastService = new Mock<IUrlCastService>();

        _urlService = new UrlShortener.BLL.Services.UrlService(
            _mapper,
            _mockUrlRepo.Object,
            null,
            _mockUrlCastService.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingUrl_ReturnsCorrectViewModel()
    {
        // Arrange
        int id = 1;
        int userId = 1;
        var url = new Url { Id = id, FullUrl = "https://example.com", ShortUrl = "abc123", UserId = userId };

        _mockUrlRepo.Setup(r => r.GetAsNoTrackingAsync(It.IsAny<Expression<Func<Url, bool>>>(), null))
            .ReturnsAsync(url);

        _mockUrlCastService.Setup(s => s.CastUrl("abc123")).Returns("https://short.url/abc123");

        // Act
        var result = await _urlService.GetByIdAsync(id, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(url.FullUrl, result.FullUrl);
        Assert.Equal("https://short.url/abc123", result.ShortUrl);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentUrl_ReturnsNull()
    {
        // Arrange
        int id = 1;
        int userId = 1;

        _mockUrlRepo.Setup(r => r.GetAsNoTrackingAsync(It.IsAny<Expression<Func<Url, bool>>>(), null))
            .ReturnsAsync((Url)null);

        // Act
        var result = await _urlService.GetByIdAsync(id, userId);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(0, 0)]
    public async Task GetByIdAsync_InvalidIds_ReturnsNull(int id, int userId)
    {
        // Act
        var result = await _urlService.GetByIdAsync(id, userId);

        // Assert
        Assert.Null(result);
        _mockUrlRepo.Verify(r => r.GetAsNoTrackingAsync(It.IsAny<Expression<Func<Url, bool>>>(), null), Times.Once);
    }
}
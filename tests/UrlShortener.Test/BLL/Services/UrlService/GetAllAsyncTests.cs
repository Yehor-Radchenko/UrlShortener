using System.Linq.Expressions;
using AutoMapper;
using Moq;
using UrlShortener.BLL.Interfaces;
using UrlShortener.Common.ResponseModels.Url;
using UrlShortener.DAL.Entities;
using UrlShortener.DAL.Repository;

namespace UrlShortener.Test.BLL.Services.UrlService;

public class UrlServiceGetAllTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IGenericRepository<Url>> _mockUrlRepo;
    private readonly Mock<IUrlCastService> _mockUrlCastService;
    private readonly UrlShortener.BLL.Services.UrlService _urlService;

    public UrlServiceGetAllTests()
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
    public async Task GetAllAsync_ReturnsCorrectViewModels()
    {
        // Arrange
        int userId = 1;
        var urls = new List<Url>
        {
            new Url { Id = 1, FullUrl = "https://example1.com", ShortUrl = "abc123", UserId = userId },
            new Url { Id = 2, FullUrl = "https://example2.com", ShortUrl = "def456", UserId = userId }
        };

        _mockUrlRepo.Setup(r => r.GetAllAsNoTrackingAsync(It.IsAny<Expression<Func<Url, bool>>>(), null))
            .ReturnsAsync(urls);

        _mockUrlCastService.Setup(s => s.CastUrl("abc123")).Returns("https://short.url/abc123");
        _mockUrlCastService.Setup(s => s.CastUrl("def456")).Returns("https://short.url/def456");

        // Act
        var result = await _urlService.GetAllAsync(userId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, vm => Assert.StartsWith("https://short.url/", vm.ShortUrl));
        Assert.Contains(result, vm => vm.FullUrl == "https://example1.com");
        Assert.Contains(result, vm => vm.FullUrl == "https://example2.com");
    }

    [Fact]
    public async Task GetAllAsync_EmptyList_ReturnsEmptyResult()
    {
        // Arrange
        int userId = 1;
        var urls = new List<Url>();

        _mockUrlRepo.Setup(r => r.GetAllAsNoTrackingAsync(It.IsAny<Expression<Func<Url, bool>>>(), null))
            .ReturnsAsync(urls);

        // Act
        var result = await _urlService.GetAllAsync(userId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_CorrectlyAppliesUserIdFilter()
    {
        // Arrange
        int userId = 1;
        Expression<Func<Url, bool>> capturedFilter = null;

        _mockUrlRepo.Setup(r => r.GetAllAsNoTrackingAsync(It.IsAny<Expression<Func<Url, bool>>>(), null))
            .Callback<Expression<Func<Url, bool>>, string[]>((filter, _) => capturedFilter = filter)
            .ReturnsAsync(new List<Url>());

        // Act
        await _urlService.GetAllAsync(userId);

        // Assert
        Assert.NotNull(capturedFilter);
        var func = capturedFilter.Compile();
        Assert.True(func(new Url { UserId = userId }));
        Assert.False(func(new Url { UserId = userId + 1 }));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetAllAsync_InvalidUserId_ReturnsEmptyResult(int userId)
    {
        // Act
        var result = await _urlService.GetAllAsync(userId);

        // Assert
        Assert.Empty(result);
        _mockUrlRepo.Verify(r => r.GetAllAsNoTrackingAsync(It.IsAny<Expression<Func<Url, bool>>>(), null), Times.Once);
    }
}
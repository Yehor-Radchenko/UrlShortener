using System.Linq.Expressions;
using AutoMapper;
using Moq;
using UrlShortener.BLL.Interfaces;
using UrlShortener.Common.RequestModels.Url;
using UrlShortener.DAL.Entities;
using UrlShortener.DAL.Repository;
using UrlShortener.DAL.UoW;
using UrlShortener.BLL.Profile;

namespace UrlShortener.Test.BLL.Services.UrlService;

public class UrlServiceTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IGenericRepository<Url>> _mockUrlRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUrlCastService> _mockUrlCastService;
    private readonly UrlShortener.BLL.Services.UrlService _urlService;

    public UrlServiceTests()
    {
        // Create a real AutoMapper instance
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>(); // Add your actual mapping profile
        });
        _mapper = configuration.CreateMapper();

        _mockUrlRepo = new Mock<IGenericRepository<Url>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUrlCastService = new Mock<IUrlCastService>();

        _urlService = new UrlShortener.BLL.Services.UrlService(
            _mapper,
            _mockUrlRepo.Object,
            _mockUnitOfWork.Object,
            _mockUrlCastService.Object);
    }

    [Fact]
    public async Task AddAsync_ValidRequest_ReturnsShortUrl()
    {
        // Arrange
        var request = new CreateUrlShortenerRequest { FullUrl = "https://example.com" };
        var userId = 1;
        var castedShortUrl = "https://short.url/abc123";

        _mockUrlRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Url, bool>>>()))
            .ReturnsAsync(false);
        _mockUrlCastService.Setup(s => s.CastUrl(It.IsAny<string>())).Returns(castedShortUrl);

        // Act
        var result = await _urlService.AddAsync(request, userId);

        // Assert
        Assert.Equal(castedShortUrl, result);
        _mockUrlRepo.Verify(r => r.Create(It.Is<Url>(u => u.FullUrl == request.FullUrl)), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        CreateUrlShortenerRequest request = null;
        var userId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _urlService.AddAsync(request, userId));
    }

    [Fact]
    public async Task AddAsync_DuplicateShortUrl_GeneratesNewShortUrl()
    {
        // Arrange
        var request = new CreateUrlShortenerRequest { FullUrl = "https://example.com" };
        var userId = 1;
        var castedShortUrl = "https://short.url/def456";

        _mockUrlRepo.SetupSequence(r => r.ExistsAsync(It.IsAny<Expression<Func<Url, bool>>>()))
            .ReturnsAsync(true)  // First attempt (duplicate)
            .ReturnsAsync(false);  // Second attempt (unique)
        _mockUrlCastService.Setup(s => s.CastUrl(It.IsAny<string>())).Returns(castedShortUrl);

        // Act
        var result = await _urlService.AddAsync(request, userId);

        // Assert
        Assert.Equal(castedShortUrl, result);
        _mockUrlRepo.Verify(r => r.ExistsAsync(It.IsAny<Expression<Func<Url, bool>>>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AddAsync_SetsCorrectProperties()
    {
        // Arrange
        var request = new CreateUrlShortenerRequest { FullUrl = "https://example.com" };
        var userId = 1;

        _mockUrlRepo.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Url, bool>>>()))
            .ReturnsAsync(false);

        // Act
        await _urlService.AddAsync(request, userId);

        // Assert
        _mockUrlRepo.Verify(r => r.Create(It.Is<Url>(u =>
            u.FullUrl == request.FullUrl &&
            u.UserId == userId &&
            u.CreatedDate != default &&
            !string.IsNullOrEmpty(u.ShortUrl)
        )), Times.Once);
    }
}
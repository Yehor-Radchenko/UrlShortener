using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using UrlShortener.BLL.Interfaces;
using UrlShortener.Common.Exceptions;
using UrlShortener.Common.RequestModels.User;
using UrlShortener.DAL.Entities;

namespace UrlShortener.Test.BLL.Services.AccountService;

public class AccountServiceRegisterUserTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<SignInManager<User>> _mockSignInManager;
    private readonly IMapper _mapper;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly UrlShortener.BLL.Services.AccountService _accountService;

    public AccountServiceRegisterUserTests()
    {
        _mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        _mockSignInManager = new Mock<SignInManager<User>>(_mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateUserRequest, User>();
        });
        _mapper = configuration.CreateMapper();

        _mockJwtService = new Mock<IJwtService>();

        _accountService = new UrlShortener.BLL.Services.AccountService(
            _mockUserManager.Object,
            _mockSignInManager.Object,
            _mapper,
            _mockJwtService.Object);
    }

    [Fact]
    public async Task RegisterUser_ExistingEmail_ThrowsConflictException()
    {
        // Arrange
        var model = new CreateUserRequest { Email = "existing@example.com", Password = "Password123!" };
        var existingUser = new User { Email = model.Email };

        _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(existingUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _accountService.RegisterUser(model));
        Assert.Equal($"User with email '{model.Email}' already exists.", exception.Message);
    }

    [Fact]
    public async Task RegisterUser_FailedCreation_ThrowsConflictException()
    {
        // Arrange
        var model = new CreateUserRequest { Email = "test@example.com", Password = "Password123!" };
        var errorDescription = "Password does not meet the requirements.";

        _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((User)null);
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), model.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = errorDescription }));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _accountService.RegisterUser(model));
        Assert.Equal($"Failed to create user: {errorDescription}", exception.Message);
    }

    [Fact]
    public async Task RegisterUser_FailedRoleAssignment_ThrowsConflictException()
    {
        // Arrange
        var model = new CreateUserRequest { Email = "test@example.com", Password = "Password123!" };
        var user = new User { Id = 1, Email = model.Email };
        var errorDescription = "Failed to assign role.";

        _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((User)null);
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), model.Password)).ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.AddToRoleAsync(user, "User"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = errorDescription }));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _accountService.RegisterUser(model));
    }
}
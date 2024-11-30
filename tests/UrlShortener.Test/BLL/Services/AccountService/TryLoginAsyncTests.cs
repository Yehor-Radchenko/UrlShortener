using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using UrlShortener.BLL.Interfaces;
using UrlShortener.Common.Exceptions;
using UrlShortener.Common.RequestModels.User;
using UrlShortener.DAL.Entities;

namespace UrlShortener.Test.BLL.Services.AccountService;

public class AccountServiceTryLoginUserTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<SignInManager<User>> _mockSignInManager;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly UrlShortener.BLL.Services.AccountService _accountService;

    public AccountServiceTryLoginUserTests()
    {
        _mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        _mockSignInManager = new Mock<SignInManager<User>>(_mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
        _mockJwtService = new Mock<IJwtService>();

        _accountService = new UrlShortener.BLL.Services.AccountService(
            _mockUserManager.Object,
            _mockSignInManager.Object,
            null, // IMapper is not used in this method
            _mockJwtService.Object);
    }

    [Fact]
    public async Task TryLoginUser_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var model = new LoginRequest { Email = "test@example.com", Password = "Password123!" };
        var user = new User { Id = 1, Email = model.Email };
        var roles = new List<string> { "User" };
        var token = "generated_token";

        _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);
        _mockSignInManager.Setup(x => x.PasswordSignInAsync(user, model.Password, false, false))
            .ReturnsAsync(SignInResult.Success);
        _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
        _mockJwtService.Setup(x => x.GenerateToken(user.Id.ToString(), user.Email, roles)).Returns(token);

        // Act
        var result = await _accountService.TryLoginUser(model);

        // Assert
        Assert.Equal(token, result);
    }

    [Fact]
    public async Task TryLoginUser_UserNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var model = new LoginRequest { Email = "nonexistent@example.com", Password = "Password123!" };

        _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _accountService.TryLoginUser(model));
    }

    [Fact]
    public async Task TryLoginUser_InvalidPassword_ThrowsAuthenticationFailedException()
    {
        // Arrange
        var model = new LoginRequest { Email = "test@example.com", Password = "WrongPassword" };
        var user = new User { Id = 1, Email = model.Email };

        _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);
        _mockSignInManager.Setup(x => x.PasswordSignInAsync(user, model.Password, false, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _accountService.TryLoginUser(model));
    }

    [Fact]
    public async Task TryLoginUser_LockedOutUser_ThrowsAuthenticationFailedException()
    {
        // Arrange
        var model = new LoginRequest { Email = "test@example.com", Password = "Password123!" };
        var user = new User { Id = 1, Email = model.Email };

        _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);
        _mockSignInManager.Setup(x => x.PasswordSignInAsync(user, model.Password, false, false))
            .ReturnsAsync(SignInResult.LockedOut);

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationFailedException>(() => _accountService.TryLoginUser(model));
    }
}
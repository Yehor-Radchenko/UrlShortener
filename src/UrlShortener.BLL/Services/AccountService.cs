using AutoMapper;
using Microsoft.AspNetCore.Identity;
using UrlShortener.BLL.Interfaces;
using UrlShortener.Common.Exceptions;
using UrlShortener.Common.RequestModels.User;
using UrlShortener.DAL.Entities;

namespace UrlShortener.BLL.Services;

public class AccountService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IMapper mapper,
    IJwtService jwtService) : IAccountService
{
    public async Task<int> RegisterUser(CreateUserRequest model)
    {
        var existingUser = await userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            throw new ConflictException($"User with email '{model.Email}' already exists.");
        }

        var userEntity = mapper.Map<User>(model);
        userEntity.UserName = userEntity.Email;
        var result = await userManager.CreateAsync(userEntity, model.Password);

        if (!result.Succeeded)
        {
            throw new ConflictException($"Failed to create user: {result.Errors.FirstOrDefault()?.Description}");
        }

        var createdUser = await userManager.FindByEmailAsync(userEntity.Email!);

        var userManagerResult = await userManager.AddToRoleAsync(createdUser!, "User");

        return !userManagerResult.Succeeded
            ? throw new ConflictException($"Failed to assign role: {userManagerResult.Errors.FirstOrDefault()?.Description}")
            : createdUser.Id;
    }

    public async Task<string> TryLoginUser(LoginRequest model)
    {
        var user = await userManager.FindByEmailAsync(model.Email)
            ?? throw new EntityNotFoundException($"User with email {model.Email} was not found.");
        var resultOfSignIn = await signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);

        if (!resultOfSignIn.Succeeded)
        {
            throw new AuthenticationFailedException("Invavid login or password.");
        }

        var token = jwtService.GenerateToken(user.Id.ToString(), user.Email!, await signInManager.UserManager.GetRolesAsync(user));

        return token;
    }

    public async Task<bool> ChangePassword(ChangePasswordRequest model)
    {
        var user = await userManager.FindByEmailAsync(model.Email)
            ?? throw new EntityNotFoundException($"User with email {model.Email} was not found.");

        var resultOfChangePass = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

        return resultOfChangePass.Succeeded;
    }
}

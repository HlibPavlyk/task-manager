using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Dtos.Auth;
using TaskManager.Application.Helpers;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IPasswordHasher<User> passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> RegisterAsync(RegisterDto registerDto)
    {
        if (await _unitOfWork.Users.AnyAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
            throw new AuthenticationException("User with this username or email already exists");

        var newUser = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
        };

        if (PasswordValidator.IsPasswordComplex(registerDto.Password))
            throw new AuthenticationException("Password is not complex enough");

        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, registerDto.Password);

        await _unitOfWork.Users.AddAsync(newUser);
        await _unitOfWork.SaveChangesAsync();

        return newUser.Id;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto login)
    {
        var user = await _unitOfWork.Users.GetUserByUsernameOrEmailAsync(login.UsernameOrEmail);

        if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, login.Password) == PasswordVerificationResult.Failed)
            throw new AuthenticationException("Invalid username, email or password");

        var token = _tokenService.CreateToken(user);

        return new LoginResponseDto(user.Username, user.Email, token);
    }
}
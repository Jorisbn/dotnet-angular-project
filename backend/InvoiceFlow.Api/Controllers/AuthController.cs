using System.Security.Cryptography.X509Certificates;
using InvoiceFlow.Api.Contracts.Auth;
using InvoiceFlow.Domain.Entities;
using InvoiceFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoiceFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly InvoiceFlowDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthController(InvoiceFlowDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> RegisterUser(RegisterRequest request)
    {
        var email = request.Email.Trim();

        var emailExists = await _dbContext.Users.AnyAsync(x => x.Email == email);

        if (emailExists)
        {
            return Conflict("A user with this email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync();

        var response = new RegisterResponse
        {
            Id = user.Id,
        };

        return StatusCode(
            StatusCodes.Status201Created,
            response
        );
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> LoginUser(LoginRequest request)
    {
        var email = request.Email.Trim();

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user is null)
        {
            return Unauthorized("Email or password is incorrect");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );

        if (verificationResult is PasswordVerificationResult.Failed)
        {
            return Unauthorized("Email or password is incorrect");
        }

        return Ok(new LoginResponse
        {
            Id = user.Id
        });
    }

    [HttpPatch("reset-password")]
    public async Task<ActionResult<ResetPasswordResponse>> ResetUserPassword(ResetPasswordRequest request)
    {
        var email = request.Email.Trim();

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user is null)
        {
            return Unauthorized("Email is not correct");
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _dbContext.SaveChangesAsync();

        return Ok(new ResetPasswordResponse
        {
            Id = user.Id,
        });
    }
}
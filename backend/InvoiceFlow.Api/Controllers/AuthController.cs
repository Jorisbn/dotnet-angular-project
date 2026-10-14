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
    public async Task<ActionResult<AuthResponse>> RegisterUser(RegisterRequest request)
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

        var response = new AuthResponse
        {
            Id = user.Id,
        };

        return StatusCode(
            StatusCodes.Status201Created,
            response
        );
    }
}
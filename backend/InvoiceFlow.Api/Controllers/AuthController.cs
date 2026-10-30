using System.Security.Claims;
using InvoiceFlow.Api.Contracts.Auth;
using InvoiceFlow.Domain.Entities;
using InvoiceFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InvoiceFlow.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace InvoiceFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly InvoiceFlowDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly PasswordResetTokenGenerator _tokenGenerator;

    public AuthController(InvoiceFlowDbContext dbContext, IPasswordHasher<User> passwordHasher, PasswordResetTokenGenerator tokenGenerator)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
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
            Email = email,
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

        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            await _dbContext.SaveChangesAsync();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        return Ok(new LoginResponse
        {
            Id = user.Id
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return NoContent();
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var email = request.Email.Trim();

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user is null)
        {
            return Ok(new
            {
                Message = "If an account exists with this email a reset link has been send"
            });
        }

        var token = _tokenGenerator.GenerateToken();
        var tokenHash = _tokenGenerator.HashToken(token);
        var resetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = tokenHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        await _dbContext.SaveChangesAsync();

        var resetUrl = $"https://localhost:4200/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

        return Ok(new
        {
            Message = "If an account exists with this email, a reset link has been send",
            Url = resetUrl
        });
    }
    [HttpPatch("reset-password")]
    public async Task<ActionResult<ResetPasswordResponse>> ResetUserPassword(ResetPasswordRequest request)
    {
        var email = request.Email.Trim();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (user is null)
        {
            return BadRequest("Invalid or expired reset token.");
        }

        var tokenHash = _tokenGenerator.HashToken(request.Token);

        var resetToken = await _dbContext.PasswordResetTokens
            .FirstOrDefaultAsync(x =>
                x.UserId == user.Id &&
                x.TokenHash == tokenHash &&
                x.UsedAt == null &&
                x.ExpiresAt > DateTime.UtcNow
            );

        if (resetToken is null)
        {
            return BadRequest("Invalid or expired reset token.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password
        );

        resetToken.UsedAt = DateTime.UtcNow;

        // Invalidate other outstanding tokens.
        var otherTokens = await _dbContext.PasswordResetTokens
            .Where(x =>
                x.UserId == user.Id &&
                x.Id != resetToken.Id &&
                x.UsedAt == null
            )
            .ToListAsync();

        foreach (var otherToken in otherTokens)
        {
            otherToken.UsedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            Message = "Your password has been successfully reset."
        });
    }
}
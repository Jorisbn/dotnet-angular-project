using InvoiceFlow.Api.Contracts.Users;
using InvoiceFlow.Domain.Entities;
using InvoiceFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoiceFlow.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly InvoiceFlowDbContext _dbContext;

    public UsersController(InvoiceFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetUser(Guid id)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .SingleOrDefaultAsync();

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser(
        CreateUserRequest request)
    {
        var emailExists = await _dbContext.Users
            .AnyAsync(x => x.Email == request.Email);

        if (emailExists)
        {
            return Conflict("A user with this email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync();

        var response = new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return CreatedAtAction(
            nameof(GetUser),
            new { id = user.Id },
            response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        UpdateUserRequest request)
    {
        var user = await _dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        var emailExists = await _dbContext.Users
            .AnyAsync(x =>
                x.Email == request.Email &&
                x.Id != id);

        if (emailExists)
        {
            return Conflict("A user with this email already exists.");
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        var hasInvoices = await _dbContext.Invoices
            .AnyAsync(x => x.UserId == id);

        if (hasInvoices)
        {
            return Conflict(
                "This user cannot be deleted because they have invoices.");
        }

        _dbContext.Users.Remove(user);

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
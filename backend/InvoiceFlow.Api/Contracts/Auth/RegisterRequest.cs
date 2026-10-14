using System.ComponentModel.DataAnnotations;

namespace InvoiceFlow.Api.Contracts.Auth;

public class RegisterRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = null!;
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = null!;
    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = null!;
    [Required]
    [MinLength(12)]
    [MaxLength(128)]
    public string Password { get; set; } = null!;
}
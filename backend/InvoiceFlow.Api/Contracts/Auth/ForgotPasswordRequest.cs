using System.ComponentModel.DataAnnotations;

namespace InvoiceFlow.Api.Contracts.Auth;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = string.Empty;
}
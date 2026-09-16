namespace InvoiceFlow.Domain.Entities;

public class AuditEntry
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public string Action { get; set; } = null!;
    public string? Description { get; set; }
    public string PerformedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public Invoice Invoice { get; set; } = null!;
}
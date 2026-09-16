namespace InvoiceFlow.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? PaymentReference { get; set; }
    public Invoice Invoice { get; set; } = null!;
}
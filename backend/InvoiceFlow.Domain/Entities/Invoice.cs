namespace InvoiceFlow.Domain.Entities;

public class Invoice
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public string SupplierName { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ApprovalStep> ApprovalSteps { get; set; } = [];
    public Payment? Payment { get; set; }
    public List<AuditEntry> AuditEntries { get; set; } = [];
}

public enum InvoiceStatus
{
    Draft,
    PendingApproval,
    Approved,
    Rejected,
    Paid
}
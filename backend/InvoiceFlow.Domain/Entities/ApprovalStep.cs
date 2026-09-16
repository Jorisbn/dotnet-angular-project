namespace InvoiceFlow.Domain.Entities;

public class ApprovalStep
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public int StepOrder { get; set; }
    public string ApproverName { get; set; } = null!;
    public ApprovalStatus Status { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Comment { get; set; }
    public Invoice Invoice { get; set; } = null!;
}

public enum ApprovalStatus
{
    Pending,
    Approved,
    Rejected
}
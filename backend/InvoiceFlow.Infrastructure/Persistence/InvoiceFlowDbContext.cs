using InvoiceFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvoiceFlow.Infrastructure.Persistence;

public class InvoiceFlowDbContext : DbContext
{
    public InvoiceFlowDbContext(
        DbContextOptions<InvoiceFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Invoice> Invoices => Set<Invoice>();

    public DbSet<ApprovalStep> ApprovalSteps => Set<ApprovalStep>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(InvoiceFlowDbContext).Assembly);
    }
}
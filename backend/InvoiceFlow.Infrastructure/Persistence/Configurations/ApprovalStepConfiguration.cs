using InvoiceFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceFlow.Infrastructure.Persistence.Configurations;

public class ApprovalStepConfiguration
    : IEntityTypeConfiguration<ApprovalStep>
{
    public void Configure(EntityTypeBuilder<ApprovalStep> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ApproverName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Comment)
            .HasMaxLength(1000);

        builder.HasIndex(x => new
        {
            x.InvoiceId,
            x.StepOrder
        })
        .IsUnique();
    }
}
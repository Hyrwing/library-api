using LibraryApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryApi.Infrastructure.Persistence.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.BorrowerName).IsRequired().HasMaxLength(100);
        builder.Property(l => l.BorrowerEmail).IsRequired().HasMaxLength(150);
        builder.HasIndex(l => l.BookId);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Questigma.Domain.Investigation;

namespace Questigma.Infrastructure.Persistence;

public sealed class CatalogCaseConfiguration : IEntityTypeConfiguration<CatalogCase>
{
    public void Configure(EntityTypeBuilder<CatalogCase> builder)
    {
        builder.ToTable("catalog_cases");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(item => item.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(item => item.Synopsis).HasColumnName("synopsis").HasMaxLength(1000).IsRequired();
        builder.Property(item => item.Introduction).HasColumnName("introduction").IsRequired();
        builder.Property(item => item.IsPublished).HasColumnName("is_published").IsRequired();
        builder.HasIndex(item => item.IsPublished);
    }
}

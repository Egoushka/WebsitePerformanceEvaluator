using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Data.Configurations;

public class LinkConfiguration : IEntityTypeConfiguration<Link>
{
    public void Configure(EntityTypeBuilder<Link> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url)
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("getdate()");

        builder.HasMany(x => x.LinkPerformances)
            .WithOne(x => x.Link)
            .HasForeignKey(x => x.LinkId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
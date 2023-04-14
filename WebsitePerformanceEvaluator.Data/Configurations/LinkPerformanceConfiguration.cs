using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Data.Configurations;

public class LinkPerformanceConfiguration : IEntityTypeConfiguration<LinkPerformance>
{
    public void Configure(EntityTypeBuilder<LinkPerformance> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Link).IsRequired();
        builder.Property(x => x.TimeResponseMs).IsRequired();
        builder.Property(x => x.CrawlingLinkSource).IsRequired();
    }
}
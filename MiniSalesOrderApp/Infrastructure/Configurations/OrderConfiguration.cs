using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniSalesOrderApp.Domain;

namespace MiniSalesOrderApp.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.OrderDate)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

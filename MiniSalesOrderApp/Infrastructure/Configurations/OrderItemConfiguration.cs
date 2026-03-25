using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniSalesOrderApp.Domain;

namespace MiniSalesOrderApp.Infrastructure.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems", tableBuilder =>
            tableBuilder.HasCheckConstraint("CK_OrderItems_Quantity_Positive", "[Quantity] > 0"));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.TotalPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}

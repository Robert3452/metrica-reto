using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pedidos.Domain.Entities;

namespace Pedidos.Infrastructure.Persistence.Configurations
{
    public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>

    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("Pedidos");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.NumeroPedido).HasMaxLength(30).IsRequired();
            builder.HasIndex(p => p.NumeroPedido).IsUnique(); // regla 2: el numero de pedido no debe repetirse
            builder.Property(p => p.Cliente).HasMaxLength(150).IsRequired();
            builder.Property(p => p.Total).HasPrecision(18, 2);
            builder.Property(p => p.Estado).HasConversion<string>().HasMaxLength(20);

        }
    }
}
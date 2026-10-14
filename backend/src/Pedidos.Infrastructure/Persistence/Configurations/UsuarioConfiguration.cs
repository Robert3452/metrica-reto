using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pedidos.Domain.Entities;

namespace Pedidos.Infrastructure.Persistence.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> b)
        {
            b.ToTable("Usuarios");
            b.HasKey(p => p.Id);
            b.Property(p => p.Email).HasMaxLength(50).IsRequired();
            b.Property(p => p.PasswordHash).IsRequired();
            b.Property(p => p.Rol).IsRequired();
        }
    }
}
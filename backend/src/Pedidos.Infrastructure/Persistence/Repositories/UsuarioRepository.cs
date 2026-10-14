using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pedidos.Application.Abstractions;
using Pedidos.Domain.Entities;
using Pedidos.Infrastructure.Persistence.Configurations;

namespace Pedidos.Infrastructure.Persistence.Repositories
{
    public class UsuarioRepository(AppDbContext db) : IUsuarioRepository
    {
        public Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct)
        {
            var usuario = db.Usuarios.AsNoTracking().FirstOrDefaultAsync(el => el.Email == email, ct);
            return usuario;
        }
    }
}
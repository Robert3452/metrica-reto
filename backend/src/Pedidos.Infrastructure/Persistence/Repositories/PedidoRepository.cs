using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pedidos.Application.Abstractions;
using Pedidos.Application.Pedidos;
using Pedidos.Domain.Entities;
using Pedidos.Infrastructure.Persistence.Configurations;

namespace Pedidos.Infrastructure.Persistence.Repositories
{
    public class PedidoRepository(AppDbContext db) : IPedidoRepository
    {
        public Task<Pedido?> GetByIdAsync(int id, CancellationToken ct)
            => db.Pedidos.FirstOrDefaultAsync(p => p.Id == id, ct);

        public Task<List<PedidoDto>> ListAsync(CancellationToken ct)
            => db.Pedidos
                 .AsNoTracking()
                 .OrderByDescending(p => p.Fecha)
                 .Select(p => new PedidoDto(
                     p.Id, p.NumeroPedido, p.Cliente, p.Total,
                     p.Estado.ToString(), p.Fecha))
                 .ToListAsync(ct);

        public async Task AddAsync(Pedido pedido, CancellationToken ct)
            => await db.Pedidos.AddAsync(pedido, ct);

        public void Remove(Pedido pedido)
            => db.Pedidos.Remove(pedido);

        public Task SaveChangesAsync(CancellationToken ct)
            => db.SaveChangesAsync(ct);

       
    }
}
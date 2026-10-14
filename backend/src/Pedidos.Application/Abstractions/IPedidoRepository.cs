using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Application.Pedidos;
using Pedidos.Domain.Entities;

namespace Pedidos.Application.Abstractions
{
    public interface IPedidoRepository
    {
        Task<Pedido?> GetByIdAsync(int id, CancellationToken ct);
        Task<List<PedidoDto>> ListAsync(CancellationToken ct);
        Task AddAsync(Pedido pedido, CancellationToken ct);
        void Remove(Pedido pedido);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
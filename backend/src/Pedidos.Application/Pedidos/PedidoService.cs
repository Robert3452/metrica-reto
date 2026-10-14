using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Application.Abstractions;
using Pedidos.Application.Exceptions;
using Pedidos.Domain.Entities;

namespace Pedidos.Application.Pedidos
{
    public class PedidoService(IPedidoRepository repo, ICurrentUser user)
    {
        public async Task<PedidoDto> UpdateAsync(int id, ActualizarPedidoRequest req, CancellationToken ct)
        {
            var pedido = await GetOwnedAsync(id, ct);
            pedido.Actualizar(req.Cliente, req.Total);
            await repo.SaveChangesAsync(ct);
            return ToDto(pedido);
        }

        private async Task<Pedido> GetOwnedAsync(int id, CancellationToken ct)
        {
            var pedido = await repo.GetByIdAsync(id, ct);
            if (pedido is null || (!user.IsAdmin))
                throw new NotFoundException($"Pedido {id} no encontrado.");
            return pedido;
        }
        public async Task<PedidoDto> CambiarEstadoAsync(int id, CambiarEstadoRequest req, CancellationToken ct)
        {
            var pedido = await GetOrThrowAsync(id, ct);
            pedido.CambiarEstado(req.Estado);
            await repo.SaveChangesAsync(ct);
            return ToDto(pedido);
        }
        private static PedidoDto ToDto(Pedido p) => new(
            p.Id, p.NumeroPedido, p.Cliente, p.Total, p.Estado.ToString(), p.Fecha);
        public async Task<List<PedidoDto>> ListAsync(CancellationToken ct)
            => await repo.ListAsync(ct);


        public async Task<Pedido> GetOrThrowAsync(int id, CancellationToken ct)
            => await repo.GetByIdAsync(id, ct)
               ?? throw new NotFoundException($"Pedido {id} no encontrado.");
        public async Task<PedidoDto> CreateAsync(CrearPedidoRequest req, CancellationToken ct)
        {
            var numero = $"PED-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
            var now = new DateTime();
            var pedido = new Pedido(numero, req.Cliente, req.Total, Domain.Enums.Estado.Registrado, now);

            await repo.AddAsync(pedido, ct);
            await repo.SaveChangesAsync(ct);

            return ToDto(pedido);
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var pedido = await GetOrThrowAsync(id, ct);
            repo.Remove(pedido);
            await repo.SaveChangesAsync(ct);
        }
    }
}
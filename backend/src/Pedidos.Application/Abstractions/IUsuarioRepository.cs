using Pedidos.Domain.Entities;

namespace Pedidos.Application.Abstractions
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct);
    }
}
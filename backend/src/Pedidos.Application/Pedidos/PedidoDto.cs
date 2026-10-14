using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Application.Pedidos
{
    public record PedidoDto(int Id, string NumeroPedido, string Cliente, decimal Total,
                        string Estado, DateTime FechaCreacion);
}
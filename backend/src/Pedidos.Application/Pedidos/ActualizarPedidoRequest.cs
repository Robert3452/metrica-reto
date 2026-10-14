using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Application.Pedidos
{
    public record ActualizarPedidoRequest(string Cliente, decimal Total);
}
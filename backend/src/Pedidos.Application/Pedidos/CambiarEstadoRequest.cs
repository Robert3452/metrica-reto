using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Domain.Enums;

namespace Pedidos.Application.Pedidos
{
    public record CambiarEstadoRequest(Estado Estado);
}
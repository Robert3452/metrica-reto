using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Domain.Enums;

namespace Pedidos.Domain.Entities
{
    public class Pedido
    {
        public int Id { get; private set; }
        public string NumeroPedido { get; private set; } = default!;
        public string Cliente { get; private set; } = default!;
        public DateTime Fecha { get; private set; } = default!;
        public Estado Estado { get; private set; }
        public decimal Total { get; private set; }
        public Pedido(string numeroPedido, string cliente, decimal total, Estado estado, DateTime fecha)
        {
            NumeroPedido = numeroPedido;
            Cliente = cliente;
            Total = total;
            Estado = estado;
            Fecha = fecha;
        }
    }
}
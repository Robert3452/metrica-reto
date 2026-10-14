using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Domain.Enums;
using Pedidos.Domain.Exceptions;

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
        private static readonly Dictionary<Estado, Estado[]> Transiciones = new()
        {
            [Estado.Registrado] = [Estado.EnProceso, Estado.Cancelado],
            [Estado.EnProceso] = [Estado.Enviado, Estado.Cancelado],
            [Estado.Enviado] = [Estado.Entregado],
            [Estado.Entregado] = [],
            [Estado.Cancelado] = []
        };

        public void CambiarEstado(Estado nuevo)
        {
            if (!Transiciones[Estado].Contains(nuevo))
                throw new DomainException($"No se puede pasar de {Estado} a {nuevo}.");

            Estado = nuevo;
        }
        public void Actualizar(string cliente, decimal total)
        {
            if (Estado is Estado.Entregado or Estado.Cancelado)
                throw new DomainException("No se puede modificar un pedido cerrado.");
            if (string.IsNullOrWhiteSpace(cliente))
                throw new DomainException("El cliente es obligatorio.");
            if (total <= 0)
                throw new DomainException("El total debe ser mayor a cero.");

            Cliente = cliente.Trim();
            Total = total;
        }
        public void Cancelar()
        {
            if (Estado == Estado.Entregado)
                throw new DomainException("No se puede cancelar un pedido entregado.");
            Estado = Estado.Cancelado;
        }

    }
}
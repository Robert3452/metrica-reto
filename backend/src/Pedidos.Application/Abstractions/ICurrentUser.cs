using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Application.Abstractions
{
    public interface ICurrentUser
    {
        int Id { get; }
        bool IsAdmin { get; }
    }
}
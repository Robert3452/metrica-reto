using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Domain.Exceptions
{
    public class DomainException(string message) : Exception(message);
}
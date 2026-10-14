using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Application.Exceptions
{
    public class ConflictException(string message) : Exception(message);
}
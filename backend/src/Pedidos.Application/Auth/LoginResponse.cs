using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Application.Auth
{
    public record LoginResponse(string AccessToken, DateTime ExpiresIn, string Rol);
}
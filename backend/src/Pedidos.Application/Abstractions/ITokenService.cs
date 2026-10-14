using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Domain.Entities;

namespace Pedidos.Application.Abstractions
{
    public interface ITokenService
    {
        TokenResult Generate(Usuario usuario);
    }
    public record TokenResult(string AccessToken, DateTime ExpiresIn);
}
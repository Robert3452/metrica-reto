using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Pedidos.Application.Abstractions;
using Pedidos.Domain.Entities;

namespace Pedidos.Infrastructure.Security
{
    public class JwtTokenService(IOptions<JwtSettings> options) : ITokenService
    {
        private readonly JwtSettings _settings = options.Value;
        private readonly JsonWebTokenHandler _handler = new();
        public TokenResult Generate(Usuario usuario)
        {
            var expira = DateTime.UtcNow.AddMinutes(_settings.ExpirationInMinutes);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                Expires = expira,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
                Claims = new Dictionary<string, object>
                {
                    [JwtRegisteredClaimNames.Sub] = usuario.Id.ToString(),
                    [JwtRegisteredClaimNames.Email] = usuario.Email,
                    [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
                    ["role"] = usuario.Rol.ToString()
                }
            };

            return new TokenResult(_handler.CreateToken(descriptor), expira);

        }
    }
}
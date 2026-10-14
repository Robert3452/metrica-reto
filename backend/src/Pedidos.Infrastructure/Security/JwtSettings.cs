using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Infrastructure.Security
{
    public class JwtSettings
    {
        public const string Section = "Jwt";

        public string Issuer { get; init; } = default!;
        public string Audience { get; init; } = default!;
        public string Key { get; init; } = default!;
        public int ExpirationInMinutes { get; init; } = 60;
    }
}
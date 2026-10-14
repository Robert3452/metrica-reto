using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Pedidos.Application.Abstractions;
using Pedidos.Domain.Enums;

namespace Pedidos.Api.Services
{
    public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
    {
        private ClaimsPrincipal User => accessor.HttpContext!.User;

        public int Id => int.Parse(User.FindFirst("sub")!.Value);
        public bool IsAdmin => User.IsInRole(nameof(Rol.Admin));
    }
}
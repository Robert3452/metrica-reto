using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pedidos.Application.Auth;

namespace Pedidos.Api.Controllers
{
    [ApiController]    
    [Route("[controller]")]
    public class AuthController(AuthService authService) : Controller
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct)
      => Ok(await authService.LoginAsync(request, ct));

        [HttpGet("me")]
        public IActionResult Me() => Ok(new
        {
            Id = User.FindFirst("sub")?.Value,
            Email = User.FindFirst("email")?.Value,
            Rol = User.FindFirst("role")?.Value
        });
    }
}
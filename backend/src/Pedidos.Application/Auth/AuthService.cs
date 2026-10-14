using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Pedidos.Application.Abstractions;
using Pedidos.Application.Exceptions;

namespace Pedidos.Application.Auth
{
    public class AuthService(IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct)
        {
            var usuario = await usuarioRepository.GetByEmailAsync(request.Email.Trim().ToLower(), ct);
            if (usuario is null || !passwordHasher.Verify(request.Password, usuario.PasswordHash))
                throw new UnauthorizedException("Credenciales inválidas.");
            var token = tokenService.Generate(usuario);
            return new LoginResponse(token.AccessToken, token.ExpiresIn, usuario.Rol.ToString());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Domain.Enums;
using Pedidos.Domain.Exceptions;


namespace Pedidos.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; private set; }
        public string Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public Rol Rol { get; private set; }

        private Usuario() { }

        public Usuario(string email, string passwordHash, Rol rol)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("El email es obligatorio.");
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("El hash del password es obligatorio.");

            Email = email.Trim().ToLowerInvariant();
            PasswordHash = passwordHash;
            Rol = rol;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pedidos.Application.Abstractions;
using Pedidos.Domain.Entities;
using Pedidos.Domain.Enums;
using Pedidos.Infrastructure.Persistence.Configurations;

namespace Pedidos.Infrastructure.Persistence
{
    public class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            await db.Database.MigrateAsync();

            if (await db.Usuarios.AnyAsync()) return;

            db.Usuarios.AddRange(
                new Usuario("admin@pedidos.com", hasher.Hash("Admin123!"), Rol.Admin),
                new Usuario("user@pedidos.com", hasher.Hash("User123!"), Rol.User));

            await db.SaveChangesAsync();
        }
    }
}
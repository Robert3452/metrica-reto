using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pedidos.Application.Abstractions;
using Pedidos.Application.Auth;
using Pedidos.Application.Pedidos;

namespace Pedidos.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<AuthService>();
            services.AddScoped<PedidoService>();


            return services;
        }
    }
}
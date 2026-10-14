using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pedidos.Application.Exceptions;
using Pedidos.Domain.Exceptions;

namespace Pedidos.Api.Middleware
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {

        public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
        {
            var (status, title) = ex switch
            {
                UnauthorizedException => (StatusCodes.Status401Unauthorized, "No autorizado"),
                NotFoundException => (StatusCodes.Status404NotFound, "No encontrado"),
                ConflictException => (StatusCodes.Status409Conflict, "Conflicto"),
                DomainException => (StatusCodes.Status400BadRequest, "Regla de negocio"),
                _ => (StatusCodes.Status500InternalServerError, "Error interno")
            };

            if (status == 500) logger.LogError(ex, "Error no controlado");

            ctx.Response.StatusCode = status;
            await ctx.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status == 500 ? null : ex.Message   // nunca exponer detalles internos
            }, ct);

            return true;
        }
    }
}
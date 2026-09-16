using Delivery.Core.Interfaces;
using Microsoft.AspNetCore.Http;


namespace Delivery.Core.Implementation;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string ObtenerUsuario()
    {
        string? usuarioId = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
        return usuarioId ?? "Desconocido";
    }
}

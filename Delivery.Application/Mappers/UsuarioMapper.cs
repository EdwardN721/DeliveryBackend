using Delivery.Core.Entities.Identity;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Commands.Identity.Usuarios;

namespace Delivery.Application.Mappers;

public static class UsuarioMapper
{
    public static Usuario MapToEntity(this CreateUsuarioCommand command, string passwordHash)
    {
        return new Usuario
        {
            Nombres = command.Nombres,
            PrimerApellido = command.PrimerApellido,
            SegundoApellido = command.SegundoApellido,
            Telefono = command.Telefono,
            Correo = command.Correo,
            Password = passwordHash
        };
    }

    public static void UpdateEntity(this Usuario usuario, UpdateUsuarioCommand updateCommand, string? passwordHash)
    {
        usuario.Nombres = !string.IsNullOrWhiteSpace(updateCommand.Nombres) ? updateCommand.Nombres : usuario.Nombres;
        usuario.PrimerApellido = !string.IsNullOrWhiteSpace(updateCommand.PrimerApellido) ? updateCommand.PrimerApellido : usuario.PrimerApellido;
        usuario.SegundoApellido = updateCommand.SegundoApellido ?? usuario.SegundoApellido;
        usuario.Telefono = !string.IsNullOrWhiteSpace(updateCommand.Telefono) ? updateCommand.Telefono : usuario.Telefono;
        usuario.Correo = !string.IsNullOrWhiteSpace(updateCommand.Correo) ? updateCommand.Correo : usuario.Correo;

        if (!string.IsNullOrWhiteSpace(passwordHash))
        {
            usuario.Password = passwordHash;
        }
    }

    /// <summary>
    /// Reemplaza el conjunto de roles del usuario por el recibido, sincronizando la tabla UsuarioRoles.
    /// </summary>
    public static void SincronizarRoles(this Usuario usuario, IEnumerable<Rol> roles)
    {
        usuario.Roles.Clear();

        foreach (Rol rol in roles)
        {
            usuario.Roles.Add(rol);
        }
    }

    public static UsuarioDto MapToDto(this Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            Nombres = usuario.Nombres,
            PrimerApellido = usuario.PrimerApellido,
            SegundoApellido = usuario.SegundoApellido,
            NombreCompleto = ConstruirNombreCompleto(usuario),
            Telefono = usuario.Telefono,
            Correo = usuario.Correo,
            Roles = usuario.Roles.MapToDto().ToList(),
            EsActivo = usuario.EsActivo,
            EsEliminado = usuario.EsEliminado
        };
    }

    public static IEnumerable<UsuarioDto> MapToDto(this IEnumerable<Usuario>? usuarios)
    {
        return usuarios?.Select(MapToDto) ?? [];
    }

    private static string ConstruirNombreCompleto(Usuario usuario)
    {
        return string.Join(" ",
            new[] { usuario.Nombres, usuario.PrimerApellido, usuario.SegundoApellido }
                .Where(parte => !string.IsNullOrWhiteSpace(parte)));
    }
}

using Aplicacion.Abstracciones;
using Dominio.Entidades.Usuarios;
using Dominio.Enumeraciones;

namespace Presentacion.Seguridad;

/// <summary>
/// Crea el primer administrador cuando la base todavía no tiene usuarios,
/// con los datos de la sección "AdministradorInicial" de la configuración.
/// Sin ese primer usuario nadie podría iniciar sesión para dar de alta a los demás.
/// </summary>
public static class InicializadorAdministrador
{
    public static async Task CrearAdministradorInicialAsync(this WebApplication app)
    {
        var logger = app.Logger;
        var seccion = app.Configuration.GetSection("AdministradorInicial");
        var email = seccion["Email"];
        var contrasena = seccion["Contrasena"];
        var nombre = seccion["NombreCompleto"] ?? "Administrador";

        try
        {
            using var alcance = app.Services.CreateScope();
            var unitOfWork = alcance.ServiceProvider.GetRequiredService<IUnitOfWork>();

            if (await unitOfWork.Usuarios.HayUsuariosAsync())
                return;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(contrasena))
            {
                logger.LogWarning(
                    "No hay usuarios y falta la sección AdministradorInicial (Email y Contrasena). " +
                    "Nadie podrá iniciar sesión hasta configurarla.");
                return;
            }

            var contrasenas = alcance.ServiceProvider.GetRequiredService<IServicioContrasenas>();
            var administrador = new Usuario
            {
                Email = email.Trim().ToLowerInvariant(),
                NombreCompleto = nombre,
                Rol = RolUsuario.Administrador,
                ContrasenaHash = contrasenas.Cifrar(contrasena)
            };

            await unitOfWork.Usuarios.AgregarAsync(administrador);
            await unitOfWork.GuardarCambiosAsync();

            logger.LogInformation("Se creó el administrador inicial {Email}.", administrador.Email);
        }
        catch (Exception ex)
        {
            // Normalmente significa que falta aplicar la migración de usuarios
            logger.LogError(ex, "No se pudo crear el administrador inicial. ¿Ya aplicaste las migraciones?");
        }
    }
}

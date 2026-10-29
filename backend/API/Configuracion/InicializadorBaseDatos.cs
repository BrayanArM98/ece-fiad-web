using Aplicacion.Abstracciones;
using Infraestructura.Data;
using Microsoft.EntityFrameworkCore;
using Presentacion.Seguridad;

namespace Presentacion.Configuracion;

/// <summary>
/// Prepara la base de datos al arrancar la API, según la configuración:
/// 1. BaseDeDatos:AplicarMigraciones = true  → aplica las migraciones pendientes (útil en el servidor).
/// 2. Crea el administrador inicial si no hay usuarios (sección AdministradorInicial).
/// 3. DatosDemostracion:Habilitar = true     → crea datos de ejemplo en una base vacía
///    y las cuentas recepcion@, medico@ y paciente@ecefiad.mx con DatosDemostracion:Contrasena.
/// </summary>
public static class InicializadorBaseDatos
{
    public static async Task PrepararBaseDeDatosAsync(this WebApplication app)
    {
        var configuracion = app.Configuration;
        var logger = app.Logger;

        if (configuracion.GetValue<bool>("BaseDeDatos:AplicarMigraciones"))
        {
            try
            {
                using var alcance = app.Services.CreateScope();
                var contexto = alcance.ServiceProvider.GetRequiredService<ContextoECE>();
                await contexto.Database.MigrateAsync();
                logger.LogInformation("Migraciones aplicadas.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "No se pudieron aplicar las migraciones. Revisa la cadena de conexión.");
                return;
            }
        }

        await app.CrearAdministradorInicialAsync();

        if (!configuracion.GetValue<bool>("DatosDemostracion:Habilitar"))
            return;

        try
        {
            using var alcance = app.Services.CreateScope();
            var contexto = alcance.ServiceProvider.GetRequiredService<ContextoECE>();
            var contrasenas = alcance.ServiceProvider.GetRequiredService<IServicioContrasenas>();

            if (await SembradorDatosDemostracion.SembrarCatalogoAsync(contexto, DateTime.Now))
                logger.LogInformation("Se crearon los datos de demostración.");

            var contrasena = configuracion["DatosDemostracion:Contrasena"];
            if (string.IsNullOrWhiteSpace(contrasena))
            {
                logger.LogWarning("Falta DatosDemostracion:Contrasena; no se crearon las cuentas de demostración.");
                return;
            }

            var cuentas = await SembradorDatosDemostracion.SembrarUsuariosAsync(contexto, contrasenas.Cifrar, contrasena);
            if (cuentas.Count > 0)
                logger.LogInformation("Cuentas de demostración creadas: {Cuentas}", string.Join(", ", cuentas));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "No se pudieron crear los datos de demostración.");
        }
    }
}

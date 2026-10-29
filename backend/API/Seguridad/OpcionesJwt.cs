using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Presentacion.Seguridad;

/// <summary>
/// Configuración de los tokens (sección "Jwt").
/// La clave es secreta: va en appsettings.Development.json (no se sube al repositorio)
/// o en una variable de entorno (Jwt__Clave) en el servidor.
/// </summary>
public class OpcionesJwt
{
    public const string Seccion = "Jwt";
    private const int LongitudMinimaClave = 32;

    public string Clave { get; set; } = string.Empty;
    public string Emisor { get; set; } = "ece-fiad-api";
    public string Audiencia { get; set; } = "ece-fiad-clientes";
    public int MinutosExpiracion { get; set; } = 480;

    public SymmetricSecurityKey ObtenerLlave() => new(Encoding.UTF8.GetBytes(Clave));

    /// <summary>Detiene el arranque con un mensaje claro si falta la clave.</summary>
    public void Validar()
    {
        if (Encoding.UTF8.GetByteCount(Clave) < LongitudMinimaClave)
        {
            throw new InvalidOperationException(
                $"Falta la clave de los tokens. Configura 'Jwt:Clave' con al menos {LongitudMinimaClave} caracteres " +
                "en appsettings.Development.json o en la variable de entorno Jwt__Clave.");
        }
    }
}

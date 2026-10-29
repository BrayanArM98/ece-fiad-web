using Aplicacion.Abstracciones;
using Dominio.Entidades.Usuarios;
using Microsoft.AspNetCore.Identity;

namespace Presentacion.Seguridad;

/// <summary>
/// Cifra contraseñas con el PasswordHasher de ASP.NET Core (PBKDF2 con sal aleatoria e iteraciones).
/// El mismo texto produce hashes distintos cada vez, así que solo se puede verificar, nunca descifrar.
/// </summary>
public sealed class ServicioContrasenas : IServicioContrasenas
{
    private static readonly Usuario UsuarioSinDatos = new();
    private readonly PasswordHasher<Usuario> _hasher = new();

    public string Cifrar(string contrasena) =>
        _hasher.HashPassword(UsuarioSinDatos, contrasena);

    public bool Verificar(string hash, string contrasena)
    {
        var resultado = _hasher.VerifyHashedPassword(UsuarioSinDatos, hash, contrasena);
        return resultado != PasswordVerificationResult.Failed;
    }
}

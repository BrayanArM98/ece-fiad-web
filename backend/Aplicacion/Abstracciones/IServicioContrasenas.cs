namespace Aplicacion.Abstracciones
{
    /// <summary>
    /// Cifrado y verificación de contraseñas. La implementación vive en la capa de presentación
    /// porque usa el PasswordHasher de ASP.NET Core.
    /// </summary>
    public interface IServicioContrasenas
    {
        string Cifrar(string contrasena);
        bool Verificar(string hash, string contrasena);
    }
}

namespace Presentacion.Seguridad;

/// <summary>
/// Nombres de los datos (claims) que viajan dentro del token.
/// Se usan los nombres cortos estándar de JWT para que cualquier cliente los lea igual.
/// </summary>
public static class ClaimsJwt
{
    public const string Sujeto = "sub";
    public const string Email = "email";
    public const string Nombre = "name";
    public const string Rol = "role";
    public const string IdDoctor = "idDoctor";
    public const string IdPaciente = "idPaciente";
}

namespace Dominio.Enumeraciones
{
    /// <summary>
    /// Roles del sistema. Cada usuario tiene exactamente uno.
    /// El nombre del valor es el que viaja en el token y en [Authorize(Roles = ...)].
    /// </summary>
    public enum RolUsuario
    {
        Administrador = 1,
        Recepcionista = 2,
        Medico = 3,
        Paciente = 4
    }
}

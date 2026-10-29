namespace Aplicacion.Seguridad
{
    /// <summary>
    /// Nombres de rol para [Authorize(Roles = ...)].
    /// Coinciden con los valores del enum RolUsuario.
    /// Las combinaciones se separan con coma: el usuario necesita cualquiera de ellos.
    /// </summary>
    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Recepcionista = "Recepcionista";
        public const string Medico = "Medico";
        public const string Paciente = "Paciente";

        public const string AdministradorORecepcionista = Administrador + "," + Recepcionista;
        public const string AdministradorOMedico = Administrador + "," + Medico;

        /// <summary>Personal de la clínica: todos menos el paciente.</summary>
        public const string Personal = Administrador + "," + Recepcionista + "," + Medico;
    }
}

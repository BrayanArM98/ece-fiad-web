namespace Aplicacion.DTOs.HistoriasClinicas
{
    public class ActualizarHistoriaDTO
    {
        // El Id es necesario para identificar qué historia clínica se va a modificar.
        public int Id { get; set; }

        // Llave foránea: aunque la práctica pide mostrar el paciente como
        // solo lectura en la edición (regla 20), conservamos la propiedad
        // para que el binding del formulario funcione correctamente.
        public int IdPaciente { get; set; }

        // Datos editables de la historia clínica.
        public DateTime FechaApertura { get; set; }
        public string Alergias { get; set; } = string.Empty;
        public string AntecedentesFamiliares { get; set; } = string.Empty;
        public string AntecedentesPersonales { get; set; } = string.Empty;

        // El estado se puede cambiar al editar (activar/desactivar la historia).
        public bool Activo { get; set; }
    }
}
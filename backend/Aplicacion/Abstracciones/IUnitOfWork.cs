namespace Aplicacion.Abstracciones
{
    public interface IUnitOfWork : IDisposable
    {
        IPacienteRepositorio Pacientes { get; }
        IEspecialidadRepositorio Especialidades { get; }
        IDoctorRepositorio Doctores { get; }
        ICitaRepositorio Citas { get; }
        IHistoriaClinicaRepositorio HistoriasClinicas { get; }
        IEvolucionRepositorio Evoluciones { get; }

        Task<int> GuardarCambiosAsync();
    }
}
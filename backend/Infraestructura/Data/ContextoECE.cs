using Dominio.Entidades.Citas;
using Dominio.Entidades.Doctores;
using Dominio.Entidades.Especialidades;
using Dominio.Entidades.Evoluciones;
using Dominio.Entidades.HistoriasClinicas;
using Dominio.Entidades.Pacientes;
using Infraestructura.Data.Configuraciones;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Data
{
    public class ContextoECE : DbContext
    {
        public ContextoECE(DbContextOptions<ContextoECE> options) : base(options)
        {
        }

        // DbSets: cada uno representa una tabla en la base de datos
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Doctor> Doctores { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Evolucion> Evoluciones { get; set; }
        public DbSet<HistoriaClinica> HistoriasClinicas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar configuraciones Fluent API de cada entidad
            modelBuilder.ApplyConfiguration(new PacienteConfiguracion());
            modelBuilder.ApplyConfiguration(new DoctorConfiguracion());
            modelBuilder.ApplyConfiguration(new EspecialidadConfiguracion());
            modelBuilder.ApplyConfiguration(new CitaConfiguracion());
            modelBuilder.ApplyConfiguration(new EvolucionConfiguracion());
            modelBuilder.ApplyConfiguration(new HistoriaClinicaConfiguracion());
        }
    }
}
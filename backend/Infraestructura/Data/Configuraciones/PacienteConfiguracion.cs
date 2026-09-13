using Dominio.Entidades.HistoriasClinicas;
using Dominio.Entidades.Pacientes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Data.Configuraciones
{
    public class PacienteConfiguracion : IEntityTypeConfiguration<Paciente>
    {
        public void Configure(EntityTypeBuilder<Paciente> builder)
        {
            builder.ToTable("Pacientes");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nombres).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Apellidos).IsRequired().HasMaxLength(100);
            builder.Property(p => p.NumeroDocumento).IsRequired().HasMaxLength(20);
            builder.Property(p => p.Telefono).HasMaxLength(15);
            builder.Property(p => p.Email).HasMaxLength(100);
            builder.Property(p => p.Direccion).HasMaxLength(200);

            // Índice único para NumeroDocumento
            builder.HasIndex(p => p.NumeroDocumento).IsUnique();

            // Relación uno a uno con HistoriaClinica
            builder.HasOne(p => p.HistoriaClinica)
                .WithOne(h => h.Paciente)
                .HasForeignKey<HistoriaClinica>(h => h.IdPaciente)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
using Dominio.Entidades.Citas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Data.Configuraciones
{
    public class CitaConfiguracion : IEntityTypeConfiguration<Cita>
    {
        public void Configure(EntityTypeBuilder<Cita> builder)
        {
            builder.ToTable("Citas");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Motivo).HasMaxLength(500);
            builder.Property(c => c.Notas).HasMaxLength(1000);

            builder.HasOne(c => c.Paciente)
                .WithMany(p => p.Citas)
                .HasForeignKey(c => c.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Doctor)
                .WithMany(d => d.Citas)
                .HasForeignKey(c => c.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
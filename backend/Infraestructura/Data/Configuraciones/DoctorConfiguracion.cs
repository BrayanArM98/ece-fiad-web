using Dominio.Entidades.Doctores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Data.Configuraciones
{
    public class DoctorConfiguracion : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.ToTable("Doctores");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Nombres).IsRequired().HasMaxLength(100);
            builder.Property(d => d.Apellidos).IsRequired().HasMaxLength(100);
            builder.Property(d => d.Email).IsRequired().HasMaxLength(100);

            builder.HasIndex(d => d.Email).IsUnique();

            builder.HasOne(d => d.Especialidad)
                .WithMany(e => e.Doctores)
                .HasForeignKey(d => d.IdEspecialidad)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
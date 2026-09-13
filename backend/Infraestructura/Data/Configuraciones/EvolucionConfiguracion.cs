using Dominio.Entidades.Evoluciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Data.Configuraciones
{
    public class EvolucionConfiguracion : IEntityTypeConfiguration<Evolucion>
    {
        public void Configure(EntityTypeBuilder<Evolucion> builder)
        {
            builder.ToTable("Evoluciones");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Diagnostico).HasMaxLength(500);
            builder.Property(e => e.Tratamiento).HasMaxLength(500);
            builder.Property(e => e.Notas).HasMaxLength(1000);

            builder.HasOne(e => e.HistoriaClinica)
                .WithMany(h => h.Evoluciones)
                .HasForeignKey(e => e.IdHistoriaClinica)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Doctor)
                .WithMany()
                .HasForeignKey(e => e.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
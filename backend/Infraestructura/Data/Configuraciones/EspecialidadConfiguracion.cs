using Dominio.Entidades.Especialidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Data.Configuraciones
{
    public class EspecialidadConfiguracion : IEntityTypeConfiguration<Especialidad>
    {
        public void Configure(EntityTypeBuilder<Especialidad> builder)
        {
            builder.ToTable("Especialidades");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            builder.HasIndex(e => e.Nombre).IsUnique();
        }
    }
}
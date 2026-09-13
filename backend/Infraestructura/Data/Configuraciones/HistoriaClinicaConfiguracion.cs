using Dominio.Entidades.HistoriasClinicas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Data.Configuraciones
{
    public class HistoriaClinicaConfiguracion : IEntityTypeConfiguration<HistoriaClinica>
    {
        public void Configure(EntityTypeBuilder<HistoriaClinica> builder)
        {
            builder.ToTable("HistoriasClinicas");
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Alergias).HasMaxLength(500);
            builder.Property(h => h.AntecedentesFamiliares).HasMaxLength(500);
            builder.Property(h => h.AntecedentesPersonales).HasMaxLength(500);
        }
    }
}
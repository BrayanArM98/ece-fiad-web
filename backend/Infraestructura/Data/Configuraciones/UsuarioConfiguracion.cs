using Dominio.Entidades.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Data.Configuraciones
{
    public class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
            builder.Property(u => u.ContrasenaHash).IsRequired().HasMaxLength(500);
            builder.Property(u => u.NombreCompleto).IsRequired().HasMaxLength(150);

            // El rol se guarda como texto para que la tabla se lea sin consultar el enum
            builder.Property(u => u.Rol).IsRequired().HasConversion<string>().HasMaxLength(20);

            builder.HasIndex(u => u.Email).IsUnique();

            // Un doctor y un paciente tienen como máximo una cuenta (índices únicos filtrados)
            builder.HasIndex(u => u.IdDoctor).IsUnique().HasFilter("[IdDoctor] IS NOT NULL");
            builder.HasIndex(u => u.IdPaciente).IsUnique().HasFilter("[IdPaciente] IS NOT NULL");

            builder.HasOne(u => u.Doctor)
                .WithMany()
                .HasForeignKey(u => u.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.Paciente)
                .WithMany()
                .HasForeignKey(u => u.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

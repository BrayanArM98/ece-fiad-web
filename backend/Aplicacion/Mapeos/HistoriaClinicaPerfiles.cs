using Aplicacion.DTOs.HistoriasClinicas;
using AutoMapper;
using Dominio.Entidades.HistoriasClinicas;

namespace Aplicacion.Mapeos
{
    public class HistoriaClinicaPerfiles : Profile
    {
        public HistoriaClinicaPerfiles()
        {
            // De DTO a Entidad (para crear)
            CreateMap<CrearHistoriaDTO, HistoriaClinica>().ReverseMap();

            // Para actualización
            CreateMap<ActualizarHistoriaDTO, HistoriaClinica>().ReverseMap();

            // Para consultar — mapea NombrePaciente desde la propiedad calculada
            // NombreCompleto de la relación Paciente.
            CreateMap<HistoriaClinica, HistoriaClinicaDTO>()
                .ForMember(
                    dest => dest.NombrePaciente,
                    opt => opt.MapFrom(src => src.Paciente != null ? src.Paciente.NombreCompleto : string.Empty));
        }
    }
}
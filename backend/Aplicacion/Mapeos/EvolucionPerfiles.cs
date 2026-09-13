using Aplicacion.DTOs.Evoluciones;
using AutoMapper;
using Dominio.Entidades.Evoluciones;

namespace Aplicacion.Mapeos
{
    public class EvolucionPerfiles : Profile
    {
        public EvolucionPerfiles()
        {
            // De DTO a Entidad (para crear)
            CreateMap<CrearEvolucionDTO, Evolucion>().ReverseMap();

            // Para actualización
            CreateMap<ActualizarEvolucionDTO, Evolucion>().ReverseMap();

            // Para consultar — mapea los tres campos derivados que viajan a la vista.
            CreateMap<Evolucion, EvolucionDTO>()
                .ForMember(
                    dest => dest.NombrePaciente,
                    opt => opt.MapFrom(src =>
                        src.HistoriaClinica != null && src.HistoriaClinica.Paciente != null
                            ? src.HistoriaClinica.Paciente.NombreCompleto
                            : string.Empty))
                .ForMember(
                    dest => dest.NombreDoctor,
                    opt => opt.MapFrom(src => src.Doctor != null
                        ? src.Doctor.NombreCompleto
                        : string.Empty))
                .ForMember(
                    dest => dest.NombreEspecialidad,
                    opt => opt.MapFrom(src =>
                        src.Doctor != null && src.Doctor.Especialidad != null
                            ? src.Doctor.Especialidad.Nombre
                            : string.Empty));
        }
    }
}
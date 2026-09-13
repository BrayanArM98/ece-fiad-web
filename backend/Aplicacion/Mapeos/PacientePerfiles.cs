using Aplicacion.DTOs.Pacientes;
using AutoMapper;
using Dominio.Entidades.Pacientes;

namespace Aplicacion.Mapeos
{
    public class PacientePerfiles : Profile
    {
        public PacientePerfiles()
        {
            // De DTO a Entidad (para crear)
            CreateMap<CrearPacienteDTO, Paciente>().ReverseMap();

            // Para actualización
            CreateMap<ActualizarPacienteDTO, Paciente>().ReverseMap();

            // Para consultar
            CreateMap<Paciente, PacienteDTO>().ReverseMap();
        }
    }
}
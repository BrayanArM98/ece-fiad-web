using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Doctores;
using Aplicacion.DTOs.Especialidades;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Doctores
{
    public partial class EditarDoctor : ComponentBase
    {
        [Inject] private IDoctorService serviciosDoctor { get; set; } = null!;
        [Inject] private IEspecialidadService serviciosEspecialidad { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        [Parameter] public int id { get; set; }

        private DoctorDTO? doctorOriginal;
        private ActualizarDoctorDTO doctorEditar = new();
        private List<EspecialidadDTO>? especialidades;

        protected override async Task OnInitializedAsync()
        {
            await CargarDoctor();
            await CargarEspecialidades();
        }

        private async Task CargarDoctor()
        {
            var resultado = await serviciosDoctor.ObtenerPorIdAsync(id);
            if (resultado.Exitoso && resultado.Datos != null)
            {
                doctorOriginal = resultado.Datos;

                // Precargar el DTO de actualización con los datos actuales
                doctorEditar = new ActualizarDoctorDTO
                {
                    Id = doctorOriginal.Id,
                    Nombres = doctorOriginal.Nombres,
                    Apellidos = doctorOriginal.Apellidos,
                    Telefono = doctorOriginal.Telefono,
                    Email = doctorOriginal.Email,
                    HorarioAtencion = doctorOriginal.HorarioAtencion,
                    IdEspecialidad = doctorOriginal.IdEspecialidad
                };
            }
            else
            {
                await Toastr.MsgError("No se pudo cargar el doctor: " + resultado.Mensaje);
                Navigation.NavigateTo("/doctores");
            }
        }

        private async Task CargarEspecialidades()
        {
            var resultado = await serviciosEspecialidad.ObtenerTodosAsync();
            if (resultado.Exitoso && resultado.Datos != null)
            {
                especialidades = resultado.Datos.ToList();
            }
            else
            {
                await Toastr.MsgError("Error al cargar las especialidades: " + resultado.Mensaje);
                especialidades = new List<EspecialidadDTO>();
            }
        }

        protected async Task GrabarDoctor()
        {
            var resultado = await serviciosDoctor.ActualizarAsync(doctorEditar);
            if (resultado.Exitoso)
            {
                await Toastr.MsgExito("Doctor actualizado exitosamente.");
                Navigation.NavigateTo("/doctores");
            }
            else
            {
                await Toastr.MsgError(resultado.Mensaje);
            }
        }

        protected void Cancelar() => Navigation.NavigateTo("/doctores");
    }
}
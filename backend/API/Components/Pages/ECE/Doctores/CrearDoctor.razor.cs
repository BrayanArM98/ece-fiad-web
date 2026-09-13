using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Doctores;
using Aplicacion.DTOs.Especialidades;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Doctores
{
    public partial class CrearDoctor : ComponentBase
    {
        [Inject] private IDoctorService serviciosDoctor { get; set; } = null!;
        [Inject] private IEspecialidadService serviciosEspecialidad { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        protected CrearDoctorDTO doctorDto = new();
        protected List<EspecialidadDTO>? especialidades;

        protected override async Task OnInitializedAsync()
        {
            await CargarEspecialidades();
        }

        // Carga la lista de especialidades activas para llenar el dropdown
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
            var resultado = await serviciosDoctor.CrearAsync(doctorDto);
            if (resultado.Exitoso)
            {
                await Toastr.MsgExito("Doctor creado exitosamente.");
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
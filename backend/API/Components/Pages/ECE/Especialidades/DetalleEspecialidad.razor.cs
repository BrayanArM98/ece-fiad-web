using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Especialidades;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Especialidades
{
    public partial class DetalleEspecialidad : ComponentBase
    {
        [Inject] private IEspecialidadService serviciosEspecialidad { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        [Parameter] public int id { get; set; }

        private EspecialidadDTO? especialidad;

        protected override async Task OnInitializedAsync()
        {
            await CargarEspecialidad();
        }

        private async Task CargarEspecialidad()
        {
            var resultado = await serviciosEspecialidad.ObtenerPorIdAsync(id);
            if (resultado.Exitoso && resultado.Datos != null)
            {
                especialidad = resultado.Datos;
            }
            else
            {
                await Toastr.MsgError("Especialidad no encontrada o error al cargar.");
                Navigation.NavigateTo("/especialidades");
            }
        }

        private void Volver() => Navigation.NavigateTo("/especialidades");
        private void Editar() => Navigation.NavigateTo($"/editar-especialidad/{id}");
    }
}
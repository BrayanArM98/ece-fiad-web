using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Doctores;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Doctores
{
    public partial class DetalleDoctor : ComponentBase
    {
        [Inject] private IDoctorService serviciosDoctor { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        [Parameter] public int id { get; set; }

        private DoctorDTO? doctor;

        protected override async Task OnInitializedAsync()
        {
            await CargarDoctor();
        }

        private async Task CargarDoctor()
        {
            var resultado = await serviciosDoctor.ObtenerPorIdAsync(id);
            if (resultado.Exitoso && resultado.Datos != null)
            {
                doctor = resultado.Datos;
            }
            else
            {
                await Toastr.MsgError("No se pudo cargar el doctor: " + resultado.Mensaje);
                Navigation.NavigateTo("/doctores");
            }
        }

        protected void Volver() => Navigation.NavigateTo("/doctores");
        protected void Editar() => Navigation.NavigateTo($"/editar-doctor/{id}");
    }
}
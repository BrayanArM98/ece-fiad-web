using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Pacientes
{
    public partial class DetallePaciente : ComponentBase
    {
        [Inject] private IPacienteService serviciosPaciente { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        [Parameter] public int id { get; set; }

        private PacienteDTO? paciente;

        protected override async Task OnInitializedAsync()
        {
            await CargarPaciente();
        }

        private async Task CargarPaciente()
        {
            var resultado = await serviciosPaciente.ObtenerPorIdAsync(id);
            if (resultado.Exitoso && resultado.Datos != null)
            {
                paciente = resultado.Datos;
            }
            else
            {
                await Toastr.MsgError("Paciente no encontrado o error al cargar.");
                Navigation.NavigateTo("/pacientes");
            }
        }

        private void Volver() => Navigation.NavigateTo("/pacientes");
        private void Editar() => Navigation.NavigateTo($"/editar-paciente/{id}");
    }
}
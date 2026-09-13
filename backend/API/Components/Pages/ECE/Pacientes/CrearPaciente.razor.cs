using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Pacientes
{
    public partial class CrearPaciente : ComponentBase
    {
        [Inject] private IPacienteService serviciosPaciente { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        protected CrearPacienteDTO pacienteDto = new();

        protected async Task GrabarPaciente()
        {
            var resultado = await serviciosPaciente.CrearAsync(pacienteDto);
            if (resultado.Exitoso)
            {
                await Toastr.MsgExito("Paciente creado exitosamente.");
                Navigation.NavigateTo("/pacientes");
            }
            else
            {
                await Toastr.MsgError("Error al crear: " + resultado.Mensaje);
            }
        }

        protected void Cancelar() => Navigation.NavigateTo("/pacientes");
    }
}
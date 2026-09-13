using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Pacientes
{
    public partial class ActualizarPaciente : ComponentBase
    {
        [Inject] private IPacienteService serviciosPaciente { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        [Parameter] public int id { get; set; }

        private PacienteDTO? pacienteOriginal;
        private ActualizarPacienteDTO pacienteEditar = new();

        protected override async Task OnInitializedAsync()
        {
            await CargarPaciente();
        }

        private async Task CargarPaciente()
        {
            var resultado = await serviciosPaciente.ObtenerPorIdAsync(id);
            if (resultado.Exitoso && resultado.Datos != null)
            {
                pacienteOriginal = resultado.Datos;

                // Mapear solo los campos editables al DTO de actualización
                pacienteEditar.Id = pacienteOriginal.Id;
                pacienteEditar.Nombres = pacienteOriginal.Nombres;
                pacienteEditar.Apellidos = pacienteOriginal.Apellidos;
                pacienteEditar.Telefono = pacienteOriginal.Telefono ?? string.Empty;
                pacienteEditar.Email = pacienteOriginal.Email ?? string.Empty;
                pacienteEditar.Direccion = pacienteOriginal.Direccion ?? string.Empty;
            }
            else
            {
                await Toastr.MsgInformacion("Paciente no encontrado o error al cargar.");
                Navigation.NavigateTo("/pacientes");
            }
        }

        private async Task GrabarPaciente()
        {
            var resultado = await serviciosPaciente.ActualizarAsync(pacienteEditar);
            if (resultado.Exitoso)
            {
                await Toastr.MsgExito("Paciente actualizado exitosamente.");
                Navigation.NavigateTo("/pacientes");
            }
            else
            {
                await Toastr.MsgError("Error al actualizar: " + resultado.Mensaje);
            }
        }

        private void Cancelar() => Navigation.NavigateTo("/pacientes");
    }
}
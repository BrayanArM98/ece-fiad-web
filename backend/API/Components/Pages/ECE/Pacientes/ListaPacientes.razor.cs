using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Pacientes
{
    public partial class ListaPacientes : ComponentBase
    {
        // Inyección de dependencias
        [Inject] private IPacienteService serviciosPaciente { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        // Controla el estilo de visualización (card o tabla)
        protected bool EstiloCard = true;
        protected List<PacienteDTO>? pacientes;

        protected override async Task OnInitializedAsync()
        {
            await CargarPacientes();
        }

        protected async Task CargarPacientes()
        {
            var resultado = await serviciosPaciente.ObtenerTodosAsync();
            if (resultado.Exitoso && resultado.Datos != null)
            {
                pacientes = resultado.Datos.ToList();
            }
            else
            {
                await Toastr.MsgError("Error al cargar los pacientes: " + resultado.Mensaje);
                pacientes = new List<PacienteDTO>();
            }
        }

        protected void CambiaEstilo()
        {
            EstiloCard = !EstiloCard;
        }

        // Métodos de navegación
        protected void NavegarACrear() => Navigation.NavigateTo("/crear-paciente");
        protected void VerDetalles(int id) => Navigation.NavigateTo($"/detalles-paciente/{id}");
        protected void Editar(int id) => Navigation.NavigateTo($"/editar-paciente/{id}");

        protected async Task ConfirmarEliminar(int id, string nombres, string apellidos)
        {
            string nombreCompleto = $"{nombres} {apellidos}";
            string mensajeConfirmacion = $"Los datos del paciente: {nombreCompleto} se eliminarán de forma permanente del sistema.";

            var confirmado = await JSRuntime.InvokeAsync<bool>("SweetAlert.showConfirm",
                "¿Estás seguro de borrar?",
                mensajeConfirmacion,
                "warning",
                "Sí, eliminar");

            if (confirmado)
            {
                var resultado = await serviciosPaciente.EliminarAsync(id);
                if (resultado.Exitoso)
                {
                    await Toastr.MsgExito($"Paciente {nombreCompleto} eliminado correctamente.");
                    await CargarPacientes();
                    StateHasChanged();
                }
                else
                {
                    await Toastr.MsgError("Error al eliminar: " + resultado.Mensaje);
                }
            }
            else
            {
                await Toastr.MsgInformacion($"Acción cancelada: Los datos del paciente {nombreCompleto} fueron conservados.");
            }
        }
    }
}
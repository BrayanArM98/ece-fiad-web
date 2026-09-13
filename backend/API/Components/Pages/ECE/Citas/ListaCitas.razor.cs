using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Aplicacion.DTOs.Citas;
using Aplicacion.Servicios.Interfaces;
using Dominio.Enumeraciones;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Citas
{
    public partial class ListaCitas : ComponentBase
    {
        [Inject] private ICitaService serviciosCita { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        protected List<CitaDTO>? citas;

        protected override async Task OnInitializedAsync()
        {
            await CargarCitas();
        }

        protected async Task CargarCitas()
        {
            var resultado = await serviciosCita.ObtenerTodasAsync();
            if (resultado.Exitoso && resultado.Datos != null)
            {
                citas = resultado.Datos.ToList();
            }
            else
            {
                await Toastr.MsgError("Error al cargar las citas: " + resultado.Mensaje);
                citas = new List<CitaDTO>();
            }
        }

        // Métodos de navegación
        protected void NavegarACrear() => Navigation.NavigateTo("/crear-cita");
        protected void VerDetalles(int id) => Navigation.NavigateTo($"/detalles-cita/{id}");
        protected void Editar(int id) => Navigation.NavigateTo($"/editar-cita/{id}");

        // Devuelve la clase CSS del badge según el estado de la cita
        protected string ObtenerColorEstado(EstadoCita estado) => estado switch
        {
            EstadoCita.Pendiente => "bg-warning text-dark",
            EstadoCita.Confirmada => "bg-primary",
            EstadoCita.Completada => "bg-success",
            EstadoCita.Cancelada => "bg-danger",
            EstadoCita.NoAsistio => "bg-secondary",
            _ => "bg-light text-dark"
        };

        // Devuelve el texto legible del estado
        protected string ObtenerTextoEstado(EstadoCita estado) => estado switch
        {
            EstadoCita.Pendiente => "Pendiente",
            EstadoCita.Confirmada => "Confirmada",
            EstadoCita.Completada => "Completada",
            EstadoCita.Cancelada => "Cancelada",
            EstadoCita.NoAsistio => "No asistió",
            _ => estado.ToString()
        };

        protected async Task ConfirmarEliminar(int id, string nombrePaciente, DateTime fechaHora)
        {
            string mensajeConfirmacion = $"La cita de '{nombrePaciente}' programada para el " +
                                         $"{fechaHora:dd/MM/yyyy HH:mm} se eliminará del sistema.";

            var confirmado = await JSRuntime.InvokeAsync<bool>("SweetAlert.showConfirm",
                "¿Estás seguro de borrar?",
                mensajeConfirmacion,
                "warning",
                "Sí, eliminar");

            if (confirmado)
            {
                var resultado = await serviciosCita.EliminarAsync(id);
                if (resultado.Exitoso)
                {
                    await Toastr.MsgExito($"Cita de '{nombrePaciente}' eliminada correctamente.");
                    await CargarCitas();
                    StateHasChanged();
                }
                else
                {
                    await Toastr.MsgError("Error al eliminar: " + resultado.Mensaje);
                }
            }
            else
            {
                await Toastr.MsgInformacion($"Acción cancelada: la cita fue conservada.");
            }
        }
    }
}
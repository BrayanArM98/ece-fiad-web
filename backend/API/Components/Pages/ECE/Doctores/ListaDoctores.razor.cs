using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Aplicacion.DTOs.Doctores;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Doctores
{
    public partial class ListaDoctores : ComponentBase
    {
        [Inject] private IDoctorService serviciosDoctor { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        protected List<DoctorDTO>? doctores;

        protected override async Task OnInitializedAsync()
        {
            await CargarDoctores();
        }

        protected async Task CargarDoctores()
        {
            var resultado = await serviciosDoctor.ObtenerTodosAsync();
            if (resultado.Exitoso && resultado.Datos != null)
            {
                doctores = resultado.Datos.ToList();
            }
            else
            {
                await Toastr.MsgError("Error al cargar los doctores: " + resultado.Mensaje);
                doctores = new List<DoctorDTO>();
            }
        }

        // Métodos de navegación
        protected void NavegarACrear() => Navigation.NavigateTo("/crear-doctor");
        protected void VerDetalles(int id) => Navigation.NavigateTo($"/detalles-doctor/{id}");
        protected void Editar(int id) => Navigation.NavigateTo($"/editar-doctor/{id}");

        protected async Task ConfirmarEliminar(int id, string nombreCompleto, int cantidadCitas)
        {
            // Validación previa: si tiene citas asociadas, no permitir eliminar
            if (cantidadCitas > 0)
            {
                await Toastr.MsgAdvertencia(
                    $"No se puede eliminar al doctor '{nombreCompleto}' porque tiene {cantidadCitas} " +
                    $"cita(s) asociada(s). Primero reasigna o cancela las citas."
                );
                return;
            }

            string mensajeConfirmacion = $"El doctor '{nombreCompleto}' se eliminará de forma permanente del sistema.";

            var confirmado = await JSRuntime.InvokeAsync<bool>("SweetAlert.showConfirm",
                "¿Estás seguro de borrar?",
                mensajeConfirmacion,
                "warning",
                "Sí, eliminar");

            if (confirmado)
            {
                var resultado = await serviciosDoctor.EliminarAsync(id);
                if (resultado.Exitoso)
                {
                    await Toastr.MsgExito($"Doctor '{nombreCompleto}' eliminado correctamente.");
                    await CargarDoctores();
                    StateHasChanged();
                }
                else
                {
                    await Toastr.MsgError("Error al eliminar: " + resultado.Mensaje);
                }
            }
            else
            {
                await Toastr.MsgInformacion($"Acción cancelada: el doctor '{nombreCompleto}' fue conservado.");
            }
        }
    }
}
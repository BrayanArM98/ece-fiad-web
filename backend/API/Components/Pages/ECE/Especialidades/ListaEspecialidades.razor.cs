using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Aplicacion.DTOs.Especialidades;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Especialidades
{
    public partial class ListaEspecialidades : ComponentBase
    {
        [Inject] private IEspecialidadService serviciosEspecialidad { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        protected List<EspecialidadDTO>? especialidades;

        protected override async Task OnInitializedAsync()
        {
            await CargarEspecialidades();
        }

        protected async Task CargarEspecialidades()
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

        // Métodos de navegación
        protected void NavegarACrear() => Navigation.NavigateTo("/crear-especialidad");
        protected void VerDetalles(int id) => Navigation.NavigateTo($"/detalles-especialidad/{id}");
        protected void Editar(int id) => Navigation.NavigateTo($"/editar-especialidad/{id}");

        protected async Task ConfirmarEliminar(int id, string nombre, int cantidadDoctores)
        {
            // Validación previa: si tiene doctores asociados, no permitir eliminar
            if (cantidadDoctores > 0)
            {
                await Toastr.MsgAdvertencia(
                    $"No se puede eliminar la especialidad '{nombre}' porque tiene {cantidadDoctores} " +
                    $"doctor(es) asociado(s). Primero reasigna o elimina los doctores."
                );
                return;
            }

            string mensajeConfirmacion = $"La especialidad '{nombre}' se eliminará de forma permanente del sistema.";

            var confirmado = await JSRuntime.InvokeAsync<bool>("SweetAlert.showConfirm",
                "¿Estás seguro de borrar?",
                mensajeConfirmacion,
                "warning",
                "Sí, eliminar");

            if (confirmado)
            {
                var resultado = await serviciosEspecialidad.EliminarAsync(id);
                if (resultado.Exitoso)
                {
                    await Toastr.MsgExito($"Especialidad '{nombre}' eliminada correctamente.");
                    await CargarEspecialidades();
                    StateHasChanged();
                }
                else
                {
                    await Toastr.MsgError("Error al eliminar: " + resultado.Mensaje);
                }
            }
            else
            {
                await Toastr.MsgInformacion($"Acción cancelada: la especialidad '{nombre}' fue conservada.");
            }
        }
    }
}
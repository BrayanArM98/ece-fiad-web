using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.HistoriasClinicas
{
    public partial class ListaHistorias : ComponentBase
    {
        [Inject] private IHistoriaClinicaService serviciosHistoria { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        protected List<HistoriaClinicaDTO>? historias;

        protected override async Task OnInitializedAsync()
        {
            await CargarHistorias();
        }

        private async Task CargarHistorias()
        {
            var resultado = await serviciosHistoria.ObtenerTodasAsync();
            if (resultado.Exitoso && resultado.Datos != null)
            {
                historias = resultado.Datos.ToList();
            }
            else
            {
                await Toastr.MsgError("Error al cargar las historias clínicas: " + resultado.Mensaje);
                historias = new List<HistoriaClinicaDTO>();
            }
        }

        // Trunca un texto largo para mostrarlo cortado en la tabla.
        protected string Truncar(string texto, int max = 40)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "—";
            return texto.Length <= max ? texto : texto.Substring(0, max) + "...";
        }

        protected async Task ConfirmarEliminar(HistoriaClinicaDTO historia)
        {
            var resultadoSwal = await JSRuntime.InvokeAsync<SwalResult>("Swal.fire", new
            {
                title = "¿Estás seguro de borrar?",
                text = $"La historia clínica de '{historia.NombrePaciente}' se eliminará del sistema.",
                icon = "warning",
                showCancelButton = true,
                confirmButtonColor = "#d33",
                cancelButtonColor = "#3085d6",
                confirmButtonText = "Sí, eliminar",
                cancelButtonText = "Cancelar"
            });

            if (resultadoSwal != null && resultadoSwal.IsConfirmed)
            {
                var resultado = await serviciosHistoria.EliminarAsync(historia.Id);
                if (resultado.Exitoso)
                {
                    await Toastr.MsgExito("Historia clínica eliminada exitosamente.");
                    await CargarHistorias();
                }
                else
                {
                    await Toastr.MsgError("Error al eliminar: " + resultado.Mensaje);
                }
            }
            else
            {
                await Toastr.MsgExito("Acción cancelada: la historia clínica fue conservada.");
            }
        }

        protected void NavegarACrear() => Navigation.NavigateTo("/crear-historia");
        protected void NavegarADetalle(int id) => Navigation.NavigateTo($"/detalles-historia/{id}");
        protected void NavegarAEditar(int id) => Navigation.NavigateTo($"/editar-historia/{id}");

        // Clase auxiliar para deserializar la respuesta de SweetAlert2
        public class SwalResult
        {
            public bool IsConfirmed { get; set; }
            public bool IsDismissed { get; set; }
        }
    }
}
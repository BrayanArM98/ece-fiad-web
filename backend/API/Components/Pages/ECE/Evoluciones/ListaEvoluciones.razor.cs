using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Evoluciones
{
    public partial class ListaEvoluciones : ComponentBase
    {
        [Inject] private IEvolucionService serviciosEvolucion { get; set; } = null!;
        [Inject] private IHistoriaClinicaService serviciosHistoria { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        protected List<EvolucionDTO>? evoluciones;
        protected int? IdHistoriaClinica;
        protected string nombrePacienteFiltro = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            // Detectar si llegamos con un filtro por historia clínica (regla 27)
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var query = QueryHelpers.ParseQuery(uri.Query);

            if (query.TryGetValue("idHistoria", out var valor) && int.TryParse(valor, out int id))
            {
                IdHistoriaClinica = id;
                await CargarNombrePaciente(id);
            }

            await CargarEvoluciones();
        }

        private async Task CargarEvoluciones()
        {
            // Si llegamos con filtro, usamos el método específico; si no, traemos todas.
            var resultado = IdHistoriaClinica.HasValue
                ? await serviciosEvolucion.ObtenerPorHistoriaClinicaAsync(IdHistoriaClinica.Value)
                : await serviciosEvolucion.ObtenerTodasAsync();

            if (resultado.Exitoso && resultado.Datos != null)
            {
                evoluciones = resultado.Datos.ToList();
            }
            else
            {
                await Toastr.MsgError("Error al cargar las evoluciones: " + resultado.Mensaje);
                evoluciones = new List<EvolucionDTO>();
            }
        }

        private async Task CargarNombrePaciente(int idHistoria)
        {
            var resultado = await serviciosHistoria.ObtenerPorIdAsync(idHistoria);
            if (resultado.Exitoso && resultado.Datos != null)
            {
                nombrePacienteFiltro = resultado.Datos.NombrePaciente;
            }
        }

        // Trunca un texto largo para mostrarlo cortado en la tabla.
        protected string Truncar(string texto, int max = 40)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "—";
            return texto.Length <= max ? texto : texto.Substring(0, max) + "...";
        }

        protected async Task ConfirmarEliminar(EvolucionDTO evolucion)
        {
            var resultadoSwal = await JSRuntime.InvokeAsync<SwalResult>("Swal.fire", new
            {
                title = "¿Estás seguro de borrar?",
                text = $"La evolución de '{evolucion.NombrePaciente}' del {evolucion.Fecha:dd/MM/yyyy} se eliminará del sistema.",
                icon = "warning",
                showCancelButton = true,
                confirmButtonColor = "#d33",
                cancelButtonColor = "#3085d6",
                confirmButtonText = "Sí, eliminar",
                cancelButtonText = "Cancelar"
            });

            if (resultadoSwal != null && resultadoSwal.IsConfirmed)
            {
                var resultado = await serviciosEvolucion.EliminarAsync(evolucion.Id);
                if (resultado.Exitoso)
                {
                    await Toastr.MsgExito("Evolución eliminada exitosamente.");
                    await CargarEvoluciones();
                }
                else
                {
                    await Toastr.MsgError("Error al eliminar: " + resultado.Mensaje);
                }
            }
            else
            {
                await Toastr.MsgExito("Acción cancelada: la evolución fue conservada.");
            }
        }

        protected void VerTodas()
        {
            // Limpiar el filtro y recargar
            IdHistoriaClinica = null;
            nombrePacienteFiltro = string.Empty;
            Navigation.NavigateTo("/evoluciones", forceLoad: true);
        }

        protected void NavegarACrear() => Navigation.NavigateTo("/crear-evolucion");
        protected void NavegarADetalle(int id) => Navigation.NavigateTo($"/detalles-evolucion/{id}");
        protected void NavegarAEditar(int id) => Navigation.NavigateTo($"/editar-evolucion/{id}");

        // Clase auxiliar para deserializar la respuesta de SweetAlert2
        public class SwalResult
        {
            public bool IsConfirmed { get; set; }
            public bool IsDismissed { get; set; }
        }
    }
}
using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.HistoriasClinicas
{
    public partial class DetalleHistoria : ComponentBase
    {
        [Inject] private IHistoriaClinicaService serviciosHistoria { get; set; } = null!;
        [Inject] private IEvolucionService serviciosEvolucion { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        [Parameter] public int Id { get; set; }

        protected HistoriaClinicaDTO? historia;
        protected int cantidadEvoluciones = 0;
        protected bool cargando = true;

        protected override async Task OnInitializedAsync()
        {
            await CargarHistoria();
            if (historia != null)
            {
                await CargarCantidadEvoluciones();
            }
            cargando = false;
        }

        private async Task CargarHistoria()
        {
            var resultado = await serviciosHistoria.ObtenerPorIdAsync(Id);
            if (resultado.Exitoso && resultado.Datos != null)
            {
                historia = resultado.Datos;
            }
            else
            {
                await Toastr.MsgError("No se pudo cargar la historia clínica: " + resultado.Mensaje);
                historia = null;
            }
        }

        private async Task CargarCantidadEvoluciones()
        {
            if (historia == null) return;

            // Reporte resumido (regla 29 del manual): cantidad de evoluciones por paciente
            var resultado = await serviciosEvolucion.ContarPorPacienteAsync(historia.IdPaciente);
            if (resultado.Exitoso)
            {
                cantidadEvoluciones = resultado.Datos;
            }
        }

        // Muestra el texto si tiene contenido, o un placeholder si está vacío.
        protected string MostrarOPlaceholder(string texto)
        {
            return string.IsNullOrWhiteSpace(texto) ? "Sin información registrada" : texto;
        }

        protected void Volver() => Navigation.NavigateTo("/historias-clinicas");
        protected void Editar() => Navigation.NavigateTo($"/editar-historia/{Id}");

        // Navega a la vista filtrada de evoluciones (regla 28 del manual)
        protected void VerEvoluciones() => Navigation.NavigateTo($"/evoluciones?idHistoria={Id}");
    }
}
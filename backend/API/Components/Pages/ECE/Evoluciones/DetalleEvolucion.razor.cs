using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Evoluciones
{
    public partial class DetalleEvolucion : ComponentBase
    {
        [Inject] private IEvolucionService serviciosEvolucion { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        [Parameter] public int Id { get; set; }

        protected EvolucionDTO? evolucion;
        protected bool cargando = true;

        protected override async Task OnInitializedAsync()
        {
            await CargarEvolucion();
            cargando = false;
        }

        private async Task CargarEvolucion()
        {
            var resultado = await serviciosEvolucion.ObtenerPorIdAsync(Id);
            if (resultado.Exitoso && resultado.Datos != null)
            {
                evolucion = resultado.Datos;
            }
            else
            {
                await Toastr.MsgError("No se pudo cargar la evolución: " + resultado.Mensaje);
                evolucion = null;
            }
        }

        // Muestra el texto si tiene contenido, o un placeholder si está vacío.
        protected string MostrarOPlaceholder(string texto)
        {
            return string.IsNullOrWhiteSpace(texto) ? "Sin notas adicionales" : texto;
        }

        protected void Volver() => Navigation.NavigateTo("/evoluciones");
        protected void Editar() => Navigation.NavigateTo($"/editar-evolucion/{Id}");
    }
}
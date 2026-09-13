using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Evoluciones
{
    public partial class EditarEvolucion : ComponentBase
    {
        [Inject] private IEvolucionService serviciosEvolucion { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        [Parameter] public int Id { get; set; }

        protected ActualizarEvolucionDTO? evolucionEditar;
        protected string nombrePaciente = string.Empty;
        protected string nombreDoctor = string.Empty;
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
                var e = resultado.Datos;

                evolucionEditar = new ActualizarEvolucionDTO
                {
                    Id = e.Id,
                    IdHistoriaClinica = e.IdHistoriaClinica,
                    IdDoctor = e.IdDoctor,
                    Fecha = e.Fecha,
                    Diagnostico = e.Diagnostico,
                    Tratamiento = e.Tratamiento,
                    Notas = e.Notas,
                    Activo = e.Activo
                };

                // Los nombres se muestran como solo lectura (regla 20)
                nombrePaciente = e.NombrePaciente;
                nombreDoctor = $"{e.NombreDoctor} ({e.NombreEspecialidad})";
            }
            else
            {
                await Toastr.MsgError("No se pudo cargar la evolución: " + resultado.Mensaje);
                evolucionEditar = null;
            }
        }

        protected async Task GrabarEvolucion()
        {
            if (evolucionEditar == null) return;

            var resultado = await serviciosEvolucion.ActualizarAsync(evolucionEditar);
            if (resultado.Exitoso)
            {
                await Toastr.MsgExito("Evolución actualizada exitosamente.");
                Navigation.NavigateTo("/evoluciones");
            }
            else
            {
                await Toastr.MsgError("Error al actualizar: " + resultado.Mensaje);
            }
        }

        protected void Cancelar() => Navigation.NavigateTo("/evoluciones");
    }
}
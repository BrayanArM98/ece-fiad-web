using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.HistoriasClinicas
{
    public partial class EditarHistoria : ComponentBase
    {
        [Inject] private IHistoriaClinicaService serviciosHistoria { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        [Parameter] public int Id { get; set; }

        protected ActualizarHistoriaDTO? historiaEditar;
        protected string nombrePaciente = string.Empty;
        protected bool cargando = true;

        protected override async Task OnInitializedAsync()
        {
            await CargarHistoria();
            cargando = false;
        }

        private async Task CargarHistoria()
        {
            var resultado = await serviciosHistoria.ObtenerPorIdAsync(Id);
            if (resultado.Exitoso && resultado.Datos != null)
            {
                var h = resultado.Datos;

                historiaEditar = new ActualizarHistoriaDTO
                {
                    Id = h.Id,
                    IdPaciente = h.IdPaciente,
                    FechaApertura = h.FechaApertura,
                    Alergias = h.Alergias,
                    AntecedentesFamiliares = h.AntecedentesFamiliares,
                    AntecedentesPersonales = h.AntecedentesPersonales,
                    Activo = h.Activo
                };

                // El nombre del paciente se muestra como solo lectura (regla 20)
                nombrePaciente = h.NombrePaciente;
            }
            else
            {
                await Toastr.MsgError("No se pudo cargar la historia clínica: " + resultado.Mensaje);
                historiaEditar = null;
            }
        }

        protected async Task GrabarHistoria()
        {
            if (historiaEditar == null) return;

            var resultado = await serviciosHistoria.ActualizarAsync(historiaEditar);
            if (resultado.Exitoso)
            {
                await Toastr.MsgExito("Historia clínica actualizada exitosamente.");
                Navigation.NavigateTo("/historias-clinicas");
            }
            else
            {
                await Toastr.MsgError("Error al actualizar: " + resultado.Mensaje);
            }
        }

        protected void Cancelar() => Navigation.NavigateTo("/historias-clinicas");
    }
}
using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Doctores;
using Aplicacion.DTOs.Evoluciones;
using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Evoluciones
{
    public partial class CrearEvolucion : ComponentBase
    {
        [Inject] private IEvolucionService serviciosEvolucion { get; set; } = null!;
        [Inject] private IHistoriaClinicaService serviciosHistoria { get; set; } = null!;
        [Inject] private IDoctorService serviciosDoctor { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        protected CrearEvolucionDTO evolucionDto = new();
        protected List<HistoriaClinicaDTO>? historias;
        protected List<DoctorDTO>? doctores;

        // Indicador de carga mientras se traen las historias clínicas y los doctores
        protected bool cargando = true;

        protected override async Task OnInitializedAsync()
        {
            await CargarHistorias();
            await CargarDoctores();
            cargando = false;
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

        private async Task CargarDoctores()
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

        protected async Task GrabarEvolucion()
        {
            var resultado = await serviciosEvolucion.CrearAsync(evolucionDto);
            if (resultado.Exitoso)
            {
                await Toastr.MsgExito("Evolución creada exitosamente.");
                Navigation.NavigateTo("/evoluciones");
            }
            else
            {
                await Toastr.MsgError("Error al crear: " + resultado.Mensaje);
            }
        }

        protected void Cancelar() => Navigation.NavigateTo("/evoluciones");
    }
}
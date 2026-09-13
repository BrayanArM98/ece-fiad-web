using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.HistoriasClinicas
{
    public partial class CrearHistoria : ComponentBase
    {
        [Inject] private IHistoriaClinicaService serviciosHistoria { get; set; } = null!;
        [Inject] private IPacienteService serviciosPaciente { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        protected CrearHistoriaDTO historiaDto = new();
        protected List<PacienteDTO>? pacientes;

        // Indicador de carga mientras se traen los pacientes sin historia clínica
        protected bool cargando = true;

        protected override async Task OnInitializedAsync()
        {
            await CargarPacientesSinHistoria();
            cargando = false;
        }

        private async Task CargarPacientesSinHistoria()
        {
            // Solo traemos pacientes que NO tienen historia clínica activa (regla 22)
            var resultado = await serviciosPaciente.ObtenerSinHistoriaClinicaAsync();
            if (resultado.Exitoso && resultado.Datos != null)
            {
                pacientes = resultado.Datos.ToList();
            }
            else
            {
                await Toastr.MsgError("Error al cargar los pacientes: " + resultado.Mensaje);
                pacientes = new List<PacienteDTO>();
            }
        }

        protected async Task GrabarHistoria()
        {
            var resultado = await serviciosHistoria.CrearAsync(historiaDto);
            if (resultado.Exitoso)
            {
                await Toastr.MsgExito("Historia clínica creada exitosamente.");
                Navigation.NavigateTo("/historias-clinicas");
            }
            else
            {
                await Toastr.MsgError("Error al crear: " + resultado.Mensaje);
            }
        }

        protected void Cancelar() => Navigation.NavigateTo("/historias-clinicas");
    }
}
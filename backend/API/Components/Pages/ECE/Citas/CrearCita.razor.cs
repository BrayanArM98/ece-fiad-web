using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Citas;
using Aplicacion.DTOs.Doctores;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Servicios.Interfaces;
using Dominio.Entidades.Citas;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Citas
{
    public partial class CrearCita : ComponentBase
    {
        [Inject] private ICitaService serviciosCita { get; set; } = null!;
        [Inject] private IPacienteService serviciosPaciente { get; set; } = null!;
        [Inject] private IDoctorService serviciosDoctor { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        protected CrearCitaDTO citaDto = new();
        protected List<PacienteDTO>? pacientes;
        protected List<DoctorDTO>? doctores;

        protected bool cargando = true;

        // Variables auxiliares para separar fecha y hora en el formulario.
        // Se sincronizan en citaDto.FechaHora cada vez que cambian.
        protected DateTime fechaSeleccionada = DateTime.Today.AddDays(1);
        protected DateTime horaSeleccionada = DateTime.Today.AddHours(9);

        protected override async Task OnInitializedAsync()
        {
            await CargarPacientes();
            await CargarDoctores();
            SincronizarFechaHora();
            cargando = false;
        }

        private async Task CargarPacientes()
        {
            var resultado = await serviciosPaciente.ObtenerTodosAsync();
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

        protected void SincronizarFechaHora()
        {
            citaDto.FechaHora = fechaSeleccionada.Date.Add(horaSeleccionada.TimeOfDay);
        }

        protected async Task GrabarCita()
        {
            SincronizarFechaHora();

            var resultado = await serviciosCita.CrearAsync(citaDto);
            if (resultado.Exitoso)
            {
                await Toastr.MsgExito("Cita creada exitosamente.");
                Navigation.NavigateTo("/citas");
            }
            else
            {
                await Toastr.MsgError("Error al crear: " + resultado.Mensaje);
            }
        }

        protected void Cancelar() => Navigation.NavigateTo("/citas");
    }
}
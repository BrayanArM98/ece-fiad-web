using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Citas;
using Aplicacion.DTOs.Doctores;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Servicios.Interfaces;
using Dominio.Entidades.Citas;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Citas
{
    public partial class EditarCita : ComponentBase
    {
        [Parameter] public int Id { get; set; }

        [Inject] private ICitaService serviciosCita { get; set; } = null!;
        [Inject] private IPacienteService serviciosPaciente { get; set; } = null!;
        [Inject] private IDoctorService serviciosDoctor { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        protected ActualizarCitaDTO? citaEditar;
        protected List<PacienteDTO>? pacientes;
        protected List<DoctorDTO>? doctores;

        protected bool cargando = true;

        protected DateTime fechaSeleccionada = DateTime.Today;
        protected DateTime horaSeleccionada = DateTime.Today;

        protected override async Task OnInitializedAsync()
        {
            await CargarPacientes();
            await CargarDoctores();
            await CargarCita();
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

        private async Task CargarCita()
        {
            var resultado = await serviciosCita.ObtenerPorIdAsync(Id);
            if (resultado.Exitoso && resultado.Datos != null)
            {
                var c = resultado.Datos;
                citaEditar = new ActualizarCitaDTO
                {
                    Id = c.Id,
                    IdPaciente = c.IdPaciente,
                    IdDoctor = c.IdDoctor,
                    FechaHora = c.FechaHora,
                    Motivo = c.Motivo,
                    Notas = c.Notas,
                    Estado = c.Estado
                };

                fechaSeleccionada = c.FechaHora.Date;
                horaSeleccionada = DateTime.Today.Add(c.FechaHora.TimeOfDay);
            }
            else
            {
                await Toastr.MsgError("No se pudo cargar la cita: " + resultado.Mensaje);
                citaEditar = null;
            }
        }

        protected void SincronizarFechaHora()
        {
            if (citaEditar != null)
            {
                citaEditar.FechaHora = fechaSeleccionada.Date.Add(horaSeleccionada.TimeOfDay);
            }
        }

        protected async Task GrabarCita()
        {
            if (citaEditar == null) return;

            SincronizarFechaHora();

            var resultado = await serviciosCita.ActualizarAsync(citaEditar);
            if (resultado.Exitoso)
            {
                await Toastr.MsgExito("Cita actualizada exitosamente.");
                Navigation.NavigateTo("/citas");
            }
            else
            {
                await Toastr.MsgError("Error al actualizar: " + resultado.Mensaje);
            }
        }

        protected void Cancelar() => Navigation.NavigateTo("/citas");
    }
}
using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Citas;
using Aplicacion.Servicios.Interfaces;
using Dominio.Enumeraciones;
using Presentacion.Servicios;

namespace Presentacion.Components.Pages.ECE.Citas
{
    public partial class DetalleCita : ComponentBase
    {
        [Inject] private ICitaService serviciosCita { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        [Parameter] public int Id { get; set; }

        protected CitaDTO? cita;
        protected bool cargando = true;

        protected override async Task OnInitializedAsync()
        {
            await CargarCita();
            cargando = false;
        }

        private async Task CargarCita()
        {
            var resultado = await serviciosCita.ObtenerPorIdAsync(Id);
            if (resultado.Exitoso && resultado.Datos != null)
            {
                cita = resultado.Datos;
            }
            else
            {
                await Toastr.MsgError("No se pudo cargar la cita: " + resultado.Mensaje);
                Navigation.NavigateTo("/citas");
            }
        }

        // Devuelve la clase CSS del badge según el estado
        protected string ObtenerColorEstado(EstadoCita estado) => estado switch
        {
            EstadoCita.Pendiente => "bg-warning text-dark",
            EstadoCita.Confirmada => "bg-primary",
            EstadoCita.Completada => "bg-success",
            EstadoCita.Cancelada => "bg-danger",
            EstadoCita.NoAsistio => "bg-secondary",
            _ => "bg-light text-dark"
        };

        // Devuelve el texto legible del estado
        protected string ObtenerTextoEstado(EstadoCita estado) => estado switch
        {
            EstadoCita.Pendiente => "Pendiente",
            EstadoCita.Confirmada => "Confirmada",
            EstadoCita.Completada => "Completada",
            EstadoCita.Cancelada => "Cancelada",
            EstadoCita.NoAsistio => "No asistió",
            _ => estado.ToString()
        };

        protected void Volver() => Navigation.NavigateTo("/citas");
        protected void Editar() => Navigation.NavigateTo($"/editar-cita/{Id}");
    }
}
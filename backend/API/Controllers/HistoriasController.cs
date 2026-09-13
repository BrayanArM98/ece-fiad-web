using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers;

/// <summary>
/// Controlador REST para la gestión de historias clínicas.
/// Cada historia clínica está asociada a un único paciente.
/// Aplica la regla 22: un paciente solo puede tener UNA historia clínica activa.
/// </summary>
public class HistoriasController : ControladorBase
{
    private readonly IHistoriaClinicaService _servicioHistorias;

    public HistoriasController(IHistoriaClinicaService servicioHistorias)
    {
        _servicioHistorias = servicioHistorias;
    }

    /// <summary>
    /// Obtiene la lista completa de historias clínicas con el paciente asociado.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var resultado = await _servicioHistorias.ObtenerTodasAsync();
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene una historia clínica específica por su ID.
    /// </summary>
    /// <param name="id">Identificador de la historia clínica.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await _servicioHistorias.ObtenerPorIdAsync(id);
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Verifica si un paciente ya tiene una historia clínica activa.
    /// Útil para deshabilitar la opción de crear historia en el frontend.
    /// </summary>
    /// <param name="idPaciente">ID del paciente a verificar.</param>
    /// <param name="idExcluir">ID de historia a excluir (útil al editar).</param>
    [HttpGet("existe-paciente/{idPaciente:int}")]
    public async Task<IActionResult> ExisteParaPaciente(int idPaciente, [FromQuery] int? idExcluir = null)
    {
        var existe = await _servicioHistorias.ExisteHistoriaParaPacienteAsync(idPaciente, idExcluir);
        return Ok(new
        {
            exitoso = true,
            mensaje = existe
                ? "El paciente ya tiene una historia clínica activa."
                : "El paciente no tiene historia clínica.",
            datos = new { existe }
        });
    }

    /// <summary>
    /// Crea una nueva historia clínica.
    /// Valida que el paciente no tenga ya una historia activa (regla 22).
    /// </summary>
    /// <param name="dto">Datos de la historia a crear.</param>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearHistoriaDTO dto)
    {
        if (dto == null)
            return BadRequest(new { exitoso = false, mensaje = "El cuerpo de la petición es requerido." });

        var resultado = await _servicioHistorias.CrearAsync(dto);
        return MapearCreacion(resultado);
    }

    /// <summary>
    /// Actualiza una historia clínica existente.
    /// </summary>
    /// <param name="id">ID de la historia a actualizar.</param>
    /// <param name="dto">Datos actualizados.</param>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarHistoriaDTO dto)
    {
        if (dto == null)
            return BadRequest(new { exitoso = false, mensaje = "El cuerpo de la petición es requerido." });

        if (id != dto.Id)
            return BadRequest(new
            {
                exitoso = false,
                mensaje = "El ID de la ruta no coincide con el ID del cuerpo."
            });

        var resultado = await _servicioHistorias.ActualizarAsync(dto);
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Elimina una historia clínica (borrado lógico).
    /// </summary>
    /// <param name="id">ID de la historia a eliminar.</param>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _servicioHistorias.EliminarAsync(id);
        return MapearResultado(resultado);
    }
}
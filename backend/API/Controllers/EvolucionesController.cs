using Aplicacion.DTOs.Evoluciones;
using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers;

/// <summary>
/// Controlador REST para la gestión de evoluciones clínicas.
/// Cada evolución pertenece a una historia clínica y registra los avances
/// de un paciente: diagnóstico, tratamiento y notas en una fecha específica.
/// </summary>
public class EvolucionesController : ControladorBase
{
    private readonly IEvolucionService _servicioEvoluciones;
    private readonly IHistoriaClinicaService _servicioHistorias;

    public EvolucionesController(
        IEvolucionService servicioEvoluciones,
        IHistoriaClinicaService servicioHistorias)
    {
        _servicioEvoluciones = servicioEvoluciones;
        _servicioHistorias = servicioHistorias;
    }

    /// <summary>
    /// Obtiene la lista completa de evoluciones con sus relaciones (paciente, doctor, especialidad).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var resultado = await _servicioEvoluciones.ObtenerTodasAsync();
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene una evolución específica por su ID.
    /// </summary>
    /// <param name="id">Identificador de la evolución.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await _servicioEvoluciones.ObtenerPorIdAsync(id);
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene todas las evoluciones de una historia clínica específica.
    /// </summary>
    /// <param name="idHistoria">ID de la historia clínica.</param>
    [HttpGet("historia/{idHistoria:int}")]
    public async Task<IActionResult> ObtenerPorHistoria(int idHistoria)
    {
        var resultado = await _servicioEvoluciones.ObtenerPorHistoriaClinicaAsync(idHistoria);
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Cuenta cuántas evoluciones activas tiene un paciente (regla 29).
    /// </summary>
    /// <param name="idPaciente">ID del paciente.</param>
    [HttpGet("paciente/{idPaciente:int}/contar")]
    public async Task<IActionResult> ContarPorPaciente(int idPaciente)
    {
        var resultado = await _servicioEvoluciones.ContarPorPacienteAsync(idPaciente);
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Obtiene el historial clínico completo de un paciente: su historia clínica
    /// junto con todas sus evoluciones ordenadas por fecha descendente.
    /// Este endpoint es el que consume el módulo de Historial del frontend (PDF, sección 2).
    /// </summary>
    /// <param name="idPaciente">ID del paciente.</param>
    [HttpGet("/api/historial/paciente/{idPaciente:int}")]
    public async Task<IActionResult> ObtenerHistorialPorPaciente(int idPaciente)
    {
        // 1. Buscar la historia clínica del paciente
        var resultadoHistorias = await _servicioHistorias.ObtenerTodasAsync();
        if (!resultadoHistorias.Exitoso)
            return MapearResultado(resultadoHistorias);

        var historiaPaciente = resultadoHistorias.Datos?
            .FirstOrDefault(h => h.IdPaciente == idPaciente && h.Activo);

        // 2. Si el paciente no tiene historia activa, devolver respuesta vacía
        if (historiaPaciente == null)
        {
            return Ok(new
            {
                exitoso = true,
                mensaje = "El paciente no tiene historia clínica activa.",
                datos = new
                {
                    tieneHistoria = false,
                    historia = (HistoriaClinicaDTO?)null,
                    evoluciones = Array.Empty<EvolucionDTO>()
                }
            });
        }

        // 3. Obtener las evoluciones de esa historia
        var resultadoEvoluciones = await _servicioEvoluciones
            .ObtenerPorHistoriaClinicaAsync(historiaPaciente.Id);

        if (!resultadoEvoluciones.Exitoso)
            return MapearResultado(resultadoEvoluciones);

        // 4. Ordenar por fecha descendente (regla H3 del PDF)
        var evolucionesOrdenadas = (resultadoEvoluciones.Datos ?? Enumerable.Empty<EvolucionDTO>())
            .OrderByDescending(e => e.Fecha)
            .ToList();

        // 5. Componer la respuesta final
        return Ok(new
        {
            exitoso = true,
            mensaje = "Historial obtenido correctamente.",
            datos = new
            {
                tieneHistoria = true,
                historia = historiaPaciente,
                evoluciones = evolucionesOrdenadas,
                totalEvoluciones = evolucionesOrdenadas.Count
            }
        });
    }

    /// <summary>
    /// Crea una nueva evolución.
    /// </summary>
    /// <param name="dto">Datos de la evolución a crear.</param>
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEvolucionDTO dto)
    {
        if (dto == null)
            return BadRequest(new { exitoso = false, mensaje = "El cuerpo de la petición es requerido." });

        var resultado = await _servicioEvoluciones.CrearAsync(dto);
        return MapearCreacion(resultado);
    }

    /// <summary>
    /// Actualiza una evolución existente.
    /// </summary>
    /// <param name="id">ID de la evolución a actualizar.</param>
    /// <param name="dto">Datos actualizados.</param>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarEvolucionDTO dto)
    {
        if (dto == null)
            return BadRequest(new { exitoso = false, mensaje = "El cuerpo de la petición es requerido." });

        if (id != dto.Id)
            return BadRequest(new
            {
                exitoso = false,
                mensaje = "El ID de la ruta no coincide con el ID del cuerpo."
            });

        var resultado = await _servicioEvoluciones.ActualizarAsync(dto);
        return MapearResultado(resultado);
    }

    /// <summary>
    /// Elimina una evolución (borrado lógico).
    /// </summary>
    /// <param name="id">ID de la evolución a eliminar.</param>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _servicioEvoluciones.EliminarAsync(id);
        return MapearResultado(resultado);
    }
}
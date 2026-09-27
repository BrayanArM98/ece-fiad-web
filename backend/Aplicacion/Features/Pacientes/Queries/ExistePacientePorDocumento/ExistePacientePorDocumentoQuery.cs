using MediatR;

namespace Aplicacion.Features.Pacientes.Queries.ExistePacientePorDocumento
{
    /// <summary>
    /// Query que verifica si ya existe un paciente con el número de documento dado.
    /// Se usa en el frontend para avisar de duplicados antes de enviar el formulario.
    /// </summary>
    public record ExistePacientePorDocumentoQuery(string NumeroDocumento)
        : IRequest<bool>;
}
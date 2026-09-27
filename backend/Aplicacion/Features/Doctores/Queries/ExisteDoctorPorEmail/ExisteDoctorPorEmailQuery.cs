using MediatR;

namespace Aplicacion.Features.Doctores.Queries.ExisteDoctorPorEmail
{
    /// <summary>
    /// Query que verifica si ya existe un doctor registrado con el correo dado.
    /// IdExcluir permite ignorar al propio doctor al momento de editarlo.
    /// </summary>
    public record ExisteDoctorPorEmailQuery(string Email, int? IdExcluir = null)
        : IRequest<bool>;
}
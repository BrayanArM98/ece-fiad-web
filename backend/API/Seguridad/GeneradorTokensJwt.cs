using System.Security.Claims;
using Aplicacion.Abstracciones;
using Dominio.Entidades.Usuarios;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Presentacion.Seguridad;

/// <summary>
/// Genera tokens JWT firmados con HMAC-SHA256.
/// El token lleva el id, el correo, el nombre, el rol y, según el rol, el doctor o paciente vinculado.
/// </summary>
public sealed class GeneradorTokensJwt : IGeneradorTokens
{
    private readonly OpcionesJwt _opciones;
    private readonly JsonWebTokenHandler _manejador = new();

    public GeneradorTokensJwt(IOptions<OpcionesJwt> opciones)
    {
        _opciones = opciones.Value;
    }

    public TokenGenerado Generar(Usuario usuario)
    {
        var expira = DateTime.UtcNow.AddMinutes(_opciones.MinutosExpiracion);

        var claims = new List<Claim>
        {
            new(ClaimsJwt.Sujeto, usuario.Id.ToString()),
            new(ClaimsJwt.Email, usuario.Email),
            new(ClaimsJwt.Nombre, usuario.NombreCompleto),
            new(ClaimsJwt.Rol, usuario.Rol.ToString())
        };

        if (usuario.IdDoctor is int idDoctor)
            claims.Add(new Claim(ClaimsJwt.IdDoctor, idDoctor.ToString()));

        if (usuario.IdPaciente is int idPaciente)
            claims.Add(new Claim(ClaimsJwt.IdPaciente, idPaciente.ToString()));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _opciones.Emisor,
            Audience = _opciones.Audiencia,
            Subject = new ClaimsIdentity(claims),
            Expires = expira,
            SigningCredentials = new SigningCredentials(_opciones.ObtenerLlave(), SecurityAlgorithms.HmacSha256)
        };

        return new TokenGenerado(_manejador.CreateToken(descriptor), expira);
    }
}

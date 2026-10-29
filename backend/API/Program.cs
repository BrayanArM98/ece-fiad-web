using System.Threading.RateLimiting;
using Aplicacion.Abstracciones;
using Aplicacion.Inyecciones;
using Infraestructura.Data;
using Infraestructura.Reportes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Presentacion.Configuracion;
using Presentacion.Middleware;
using Presentacion.Seguridad;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// SERVICIOS
// ============================================

// Registrar DbContext con SQL Server
builder.Services.AddDbContext<ContextoECE>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionBD")));

// Registrar UnitOfWork
builder.Services.AddScoped<IUnitOfWork, Infraestructura.UnitOfWork.UnitOfWork>();

// Reportes: consultas de solo lectura y exportación a PDF y Excel
builder.Services.AddScoped<IConsultasReportes, ConsultasReportes>();
builder.Services.AddSingleton<IExportadorReportes, ExportadorReportes>();

// Registrar servicios de la capa de Aplicación.
// La licencia de AutoMapper es opcional y se lee de la configuración (AutoMapper:LicenseKey).
builder.Services.AgregarAplicacion(builder.Configuration["AutoMapper:LicenseKey"]);

// ============================================
// SEGURIDAD: USUARIOS, TOKENS Y ROLES
// ============================================

// Usuario de la petición (lo leen los handlers para aplicar reglas por rol)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioActual, UsuarioActual>();
builder.Services.AddSingleton<IServicioContrasenas, ServicioContrasenas>();

// Configuración de los tokens; si falta la clave, la API no arranca y dice por qué
builder.Services.Configure<OpcionesJwt>(builder.Configuration.GetSection(OpcionesJwt.Seccion));
var opcionesJwt = builder.Configuration.GetSection(OpcionesJwt.Seccion).Get<OpcionesJwt>() ?? new OpcionesJwt();
opcionesJwt.Validar();
builder.Services.AddSingleton<IGeneradorTokens, GeneradorTokensJwt>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Se conservan los nombres cortos de los claims ("sub", "role", ...)
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = opcionesJwt.Emisor,
            ValidateAudience = true,
            ValidAudience = opcionesJwt.Audiencia,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = opcionesJwt.ObtenerLlave(),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = ClaimsJwt.Nombre,
            RoleClaimType = ClaimsJwt.Rol
        };

        // 401 y 403 con la misma forma que el resto de la API
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async contexto =>
            {
                contexto.HandleResponse();
                contexto.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await contexto.Response.WriteAsJsonAsync(new
                {
                    exitoso = false,
                    mensaje = "Tu sesión no es válida o expiró. Inicia sesión de nuevo.",
                    errores = Array.Empty<string>()
                });
            },
            OnForbidden = async contexto =>
            {
                contexto.Response.StatusCode = StatusCodes.Status403Forbidden;
                await contexto.Response.WriteAsJsonAsync(new
                {
                    exitoso = false,
                    mensaje = "No tienes permiso para realizar esta acción.",
                    errores = Array.Empty<string>()
                });
            }
        };
    });

// Todos los endpoints requieren sesión, salvo los marcados con [AllowAnonymous] (el login)
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Máximo 5 intentos de inicio de sesión por minuto por dirección IP
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", contexto => RateLimitPartition.GetFixedWindowLimiter(
        contexto.Connection.RemoteIpAddress?.ToString() ?? "desconocido",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
    options.OnRejected = async (contexto, token) =>
    {
        await contexto.HttpContext.Response.WriteAsJsonAsync(new
        {
            exitoso = false,
            mensaje = "Demasiados intentos de inicio de sesión. Espera un minuto e intenta de nuevo.",
            errores = Array.Empty<string>()
        }, token);
    };
});

// ============================================
// SERVICIOS DE API REST
// ============================================

// Habilitar controladores REST
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Los enums se serializan como strings ("Programada") en lugar de números (0)
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());

        // Evita ciclos infinitos en la serialización
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // Cuando el cuerpo no se puede leer (JSON mal formado, enum desconocido, fecha inválida),
        // se responde con la misma forma que el resto de la API y en español.
        options.InvalidModelStateResponseFactory = contexto =>
        {
            var errores = contexto.ModelState
                .Where(campo => campo.Value?.Errors.Count > 0)
                .Select(campo => DescribirCampoInvalido(campo.Key))
                .Distinct()
                .ToList();

            return new BadRequestObjectResult(new
            {
                exitoso = false,
                mensaje = "La solicitud tiene datos con un formato inválido.",
                errores
            });
        };
    });

// Errores no controlados: se registran en el log y se responde 500 sin detalles internos
builder.Services.AddExceptionHandler<ManejadorGlobalExcepciones>();
builder.Services.AddProblemDetails();

// CORS: en el servidor solo se permiten los orígenes configurados (Cors:OrigenesPermitidos),
// por ejemplo el dominio del frontend en Vercel. Sin la lista, se permite cualquier origen (desarrollo).
// La app móvil no usa CORS: esa restricción solo aplica a navegadores.
var origenesPermitidos = builder.Configuration.GetSection("Cors:OrigenesPermitidos").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        if (origenesPermitidos.Length > 0)
            policy.WithOrigins(origenesPermitidos);
        else
            policy.AllowAnyOrigin();

        // Content-Disposition trae el nombre del archivo al descargar un reporte
        policy.AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("Content-Disposition");
    });
});

// Swagger / OpenAPI para documentación automática, con el botón "Authorize" para pegar el token
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Pega el token que devuelve POST /api/Auth/login (sin la palabra Bearer)."
    });
    options.AddSecurityRequirement(documento => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", documento)] = []
    });
});

var app = builder.Build();

// Debe ir primero para atrapar errores de todo el pipeline
app.UseExceptionHandler();

// ============================================
// PIPELINE HTTP
// ============================================

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Swagger en desarrollo, y en el servidor solo si Swagger:Habilitar = true
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Habilitar"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ECE-FIAD API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// Habilitar CORS (debe ir antes de MapControllers)
app.UseCors("Frontend");

// Quién es el usuario (token) y qué puede hacer (roles)
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// Endpoints de API REST
app.MapControllers();

// Verificación de que la API está en línea (la usa el servidor de despliegue)
app.MapGet("/salud", () => Results.Ok(new { estado = "en línea" })).AllowAnonymous();

// Migraciones, administrador inicial y datos de demostración según la configuración
await app.PrepararBaseDeDatosAsync();

app.Run();

// Convierte la clave del ModelState ("$.fechaNacimiento", "dto") en un mensaje legible
static string DescribirCampoInvalido(string clave)
{
    var campo = clave.Replace("$.", string.Empty).Replace("$", string.Empty).Trim('.');
    return string.IsNullOrEmpty(campo) || campo.Equals("dto", StringComparison.OrdinalIgnoreCase)
        ? "El cuerpo de la petición es requerido o no es un JSON válido."
        : $"El campo '{campo}' tiene un valor con formato inválido.";
}

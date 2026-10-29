using Aplicacion.Abstracciones;
using Aplicacion.Inyecciones;
using Infraestructura.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentacion.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// SERVICIOS
// ============================================

// Registrar DbContext con SQL Server
builder.Services.AddDbContext<ContextoECE>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionBD")));

// Registrar UnitOfWork
builder.Services.AddScoped<IUnitOfWork, Infraestructura.UnitOfWork.UnitOfWork>();

// Registrar servicios de la capa de Aplicación.
// La licencia de AutoMapper es opcional y se lee de la configuración (AutoMapper:LicenseKey).
builder.Services.AgregarAplicacion(builder.Configuration["AutoMapper:LicenseKey"]);

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

// CORS abierto para que cualquier frontend pueda consumir la API
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger / OpenAPI para documentación automática
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
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
app.UseCors("PermitirTodo");

// Endpoints de API REST
app.MapControllers();

app.Run();

// Convierte la clave del ModelState ("$.fechaNacimiento", "dto") en un mensaje legible
static string DescribirCampoInvalido(string clave)
{
    var campo = clave.Replace("$.", string.Empty).Replace("$", string.Empty).Trim('.');
    return string.IsNullOrEmpty(campo) || campo.Equals("dto", StringComparison.OrdinalIgnoreCase)
        ? "El cuerpo de la petición es requerido o no es un JSON válido."
        : $"El campo '{campo}' tiene un valor con formato inválido.";
}

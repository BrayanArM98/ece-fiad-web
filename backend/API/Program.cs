using Aplicacion.Abstracciones;
using Aplicacion.Inyecciones;
using Infraestructura.Data;
using Microsoft.EntityFrameworkCore;
using Presentacion.Components;
using Presentacion.Servicios;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// SERVICIOS DE BLAZOR SERVER
// ============================================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registrar DbContext con SQL Server
builder.Services.AddDbContext<ContextoECE>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionBD")));

// Registrar UnitOfWork
builder.Services.AddScoped<IUnitOfWork, Infraestructura.UnitOfWork.UnitOfWork>();

// Registrar servicios de la capa de Aplicación
builder.Services.AgregarAplicacion();

// Toastr
builder.Services.AddScoped<IToastrService, ToastrService>();

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
    });

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

// ============================================
// PIPELINE HTTP
// ============================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
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

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// Habilitar CORS (debe ir ANTES de UseAntiforgery y MapControllers)
app.UseCors("PermitirTodo");

app.UseAntiforgery();

app.MapStaticAssets();

// Endpoints de Blazor Server
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Endpoints de API REST
app.MapControllers();

app.Run();
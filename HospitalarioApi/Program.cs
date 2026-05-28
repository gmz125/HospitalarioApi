using HospitalarioApi;
using HospitalarioApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//base de datos (PostgreSQL/Supabase)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddNpgsql<AppDbContext>(connectionString);

//CONFIGURACIÓN DE CORS (Permite que la App de Flutter se conecte)
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

//Habilitar CORS en la aplicación
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ENDPOINTS DE CONSULTA (GET)

// Obtener todos los medicamentos
app.MapGet("/api/medicamentos", async (AppDbContext db) => {
    var lista = await db.Medicamentos.ToListAsync();
    return Results.Ok(lista);
});

// Obtener todos los usuarios (con manejo de errores)
app.MapGet("/api/usuarios", async (AppDbContext db) =>
{
    try
    {
        var lista = await db.Usuarios.ToListAsync();
        return Results.Ok(lista);
    }
    catch (Exception ex)
    {
        return Results.Problem("Error de base de datos: " + ex.Message);
    }
});

// Obtener todas las ventas
app.MapGet("/api/ventas", async (AppDbContext db) => {
    var lista = await db.Ventas.ToListAsync();
    return Results.Ok(lista);
});

// Obtener los lotes de un medicamento específico
app.MapGet("/api/medicamentos/{id}/lotes", async (Guid id, AppDbContext db) =>
{
    var lista = await db.Lotes.Where(l => l.MedicamentoId == id).ToListAsync();
    return Results.Ok(lista);
});

// ENDPOINTS DE GUARDADO (POST)

// Registrar un nuevo medicamento
app.MapPost("/api/medicamentos", async (Medicamento med, AppDbContext db) => {
    db.Medicamentos.Add(med);
    await db.SaveChangesAsync();
    return Results.Created($"/api/medicamentos/{med.Id}", med);
});

// Registrar un usuario (Sincronización desde Flutter)
app.MapPost("/api/usuarios", async (Usuario user, AppDbContext db) => {
    // Verificación para no duplicar si el usuario ya existe
    var existe = await db.Usuarios.AnyAsync(u => u.Id == user.Id);
    if (existe) return Results.Ok(user);

    db.Usuarios.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/api/usuarios/{user.Id}", user);
});

// Registrar una nueva venta
app.MapPost("/api/ventas", async (Venta venta, AppDbContext db) => {
    db.Ventas.Add(venta);
    await db.SaveChangesAsync();
    return Results.Created($"/api/ventas/{venta.Id}", venta);
});

// Registrar un nuevo lote
app.MapPost("/api/lotes", async (Lote lote, AppDbContext db) => {
    db.Lotes.Add(lote);
    await db.SaveChangesAsync();
    return Results.Created($"/api/lotes/{lote.Id}", lote);
});

app.Run();
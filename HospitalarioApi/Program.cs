using HospitalarioApi;
using HospitalarioApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
// Obtener todos los medicamentos 
app.MapGet("/api/medicamentos", async (AppDbContext db) => {
    try
    {
        var lista = await db.Medicamentos.ToListAsync();
        var todosLosLotes = await db.Lotes.ToListAsync();
        foreach (var med in lista)
        {
            med.Lotes = todosLosLotes
                .Where(l => l.MedicamentoId == med.Id)
                .ToList();
        }

        return Results.Ok(lista);
    }
    catch (Exception ex)
    {
        // Esto imprimirá el error real en los logs de Render
        Console.WriteLine($"ERROR CRÍTICO EN GET MEDICAMENTOS: {ex.Message}");
        if (ex.InnerException != null) Console.WriteLine($"INNER ERROR: {ex.InnerException.Message}");

        return Results.Problem("Error interno: " + ex.Message);
    }
});


// Este lo puedes mantener por si necesitas consultar lotes por separado
app.MapGet("/api/medicamentos/{id}/lotes", async (Guid id, AppDbContext db) =>
{
    var lista = await db.Lotes.Where(l => l.MedicamentoId == id).ToListAsync();
    return Results.Ok(lista);
});

// ENDPOINTS DE GUARDADO (POST)
app.MapPost("/api/medicamentos", async (Medicamento med, AppDbContext db) => {
    try
    {
        if (med.Id == Guid.Empty) med.Id = Guid.NewGuid();

        db.Database.SetCommandTimeout(30);

        db.Medicamentos.Add(med);
        await db.SaveChangesAsync(); 

        if (med.Lotes != null && med.Lotes.Any())
        {
            foreach (var lote in med.Lotes)
            {
                lote.Id = Guid.NewGuid();
                lote.MedicamentoId = med.Id;
                lote.FechaCaducidad = DateTime.SpecifyKind(lote.FechaCaducidad, DateTimeKind.Utc);
                db.Lotes.Add(lote);
            }
            await db.SaveChangesAsync(); 
        }
        return Results.Created($"/api/medicamentos/{med.Id}", med);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.InnerException?.Message ?? ex.Message}");
        return Results.Problem("Error: " + (ex.InnerException?.Message ?? ex.Message));
    }
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
    try
    {
        venta.Fecha = DateTime.SpecifyKind(venta.Fecha, DateTimeKind.Utc);
        if (venta.ClienteId == "mostrador") venta.ClienteId = null;

        db.Ventas.Add(venta);
        await db.SaveChangesAsync();
        return Results.Created($"/api/ventas/{venta.Id}", venta);
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR VENTA: " + ex.Message);
        return Results.Problem(ex.Message);
    }
});
//Registrar lotes independientes
app.MapPost("/api/lotes", async (Lote lote, AppDbContext db) => {
    try
    {
        lote.FechaCaducidad = DateTime.SpecifyKind(lote.FechaCaducidad, DateTimeKind.Utc);

        db.Lotes.Add(lote);
        await db.SaveChangesAsync();
        return Results.Created($"/api/lotes/{lote.Id}", lote);
    }
    catch (Exception ex)
    {
        return Results.Problem("Error al guardar lote: " + ex.Message);
    }
});
app.Run();
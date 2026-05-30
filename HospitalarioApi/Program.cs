using HospitalarioApi;
using HospitalarioApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

//Configuración de la base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddNpgsql<AppDbContext>(connectionString);

//Configuración de CORS
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Comentado para evitar lentitud en Render
// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ENDPOINTS DE CONSULTA (GET) - Optimizados

app.MapGet("/api/usuarios", async (AppDbContext db) =>
{
    try
    {
        var lista = await db.Usuarios.AsNoTracking().ToListAsync();
        return Results.Ok(lista);
    }
    catch (Exception ex)
    {
        return Results.Problem("Error base de datos: " + ex.Message);
    }
});

app.MapGet("/api/ventas", async (AppDbContext db) => {
    try
    {
        var lista = await db.Ventas.AsNoTracking().ToListAsync();
        return Results.Ok(lista);
    }
    catch (Exception ex) { return Results.Problem(ex.Message); }
});

app.MapGet("/api/medicamentos", async (AppDbContext db) => {
    try
    {
        // Traemos los medicamentos y sus lotes en una sola pasada ultra rápida
        var lista = await db.Medicamentos
            .AsNoTracking()
            .Select(m => new {
                m.Id,
                m.Nombre,
                m.Descripcion,
                m.Precio,
                m.RequiereReceta,
                m.Categoria,
                m.Subcategoria,
                m.UrlImagen,
                m.NombreNegocio,
                m.UsuarioId,
                m.Telefono,
                Lotes = db.Lotes.Where(l => l.MedicamentoId == m.Id).ToList()
            })
            .ToListAsync();

        return Results.Ok(lista);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"FALLO GET: {ex.Message}");
        return Results.Problem("El servidor de base de datos está despertando. Por favor, refresca en 10 segundos.");
    }
});

app.MapGet("/api/medicamentos/{id}/lotes", async (Guid id, AppDbContext db) =>
{
    var lista = await db.Lotes.AsNoTracking().Where(l => l.MedicamentoId == id).ToListAsync();
    return Results.Ok(lista);
});

app.MapGet("/api/lotes", async (AppDbContext db) =>
{
    try
    {
        var lista = await db.Lotes.AsNoTracking().ToListAsync();
        return Results.Ok(lista);
    }
    catch (Exception ex)
    {
        return Results.Problem("Error al consultar lotes: " + ex.Message);
    }
});

// ENDPOINTS DE GUARDADO (POST)

app.MapPost("/api/medicamentos", async (Medicamento med, AppDbContext db) => {
    try
    {
        if (med.Id == Guid.Empty) med.Id = Guid.NewGuid();

        db.Medicamentos.Add(med);
        await db.SaveChangesAsync();

        if (med.Lotes != null && med.Lotes.Any())
        {
            foreach (var lote in med.Lotes)
            {
                if (lote.Id == Guid.Empty) lote.Id = Guid.NewGuid();
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
        return Results.Problem("Error: " + (ex.InnerException?.Message ?? ex.Message));
    }
});

app.MapPost("/api/usuarios", async (Usuario user, AppDbContext db) => {
    try
    {
        var existe = await db.Usuarios.AnyAsync(u => u.Id == user.Id);
        if (existe) return Results.Ok(user);

        db.Usuarios.Add(user);
        await db.SaveChangesAsync();
        return Results.Created($"/api/usuarios/{user.Id}", user);
    }
    catch (Exception ex) { return Results.Problem(ex.Message); }
});

app.MapPost("/api/ventas", async (Venta venta, AppDbContext db) => {
    try
    {
        venta.Fecha = DateTime.SpecifyKind(venta.Fecha, DateTimeKind.Utc);
        if (venta.ClienteId == "mostrador" || string.IsNullOrEmpty(venta.ClienteId)) venta.ClienteId = null;

        db.Ventas.Add(venta);
        await db.SaveChangesAsync();
        return Results.Created($"/api/ventas/{venta.Id}", venta);
    }
    catch (Exception ex) { return Results.Problem(ex.Message); }
});

app.MapPost("/api/lotes", async (Lote lote, AppDbContext db) => {
    try
    {
        if (lote.Id == Guid.Empty) lote.Id = Guid.NewGuid();
        lote.FechaCaducidad = DateTime.SpecifyKind(lote.FechaCaducidad, DateTimeKind.Utc);
        db.Lotes.Add(lote);
        await db.SaveChangesAsync();
        return Results.Created($"/api/lotes/{lote.Id}", lote);
    }
    catch (Exception ex) { return Results.Problem(ex.Message); }
});

app.Run();
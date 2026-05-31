using Microsoft.EntityFrameworkCore;
using HospitalarioApi;
using HospitalarioApi.Models;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddNpgsql<AppDbContext>(connectionString);

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

app.UseAuthorization();
app.MapControllers();

// ENDPOINTS DE CONSULTA (GET)

// Obtener Usuarios
app.MapGet("/api/usuarios", async (AppDbContext db) =>
    Results.Ok(await db.Usuarios.AsNoTracking().ToListAsync()));

// Obtener Ventas
app.MapGet("/api/ventas", async (AppDbContext db) =>
    Results.Ok(await db.Ventas.AsNoTracking().ToListAsync()));

// Obtener Medicamentos con Lotes 
app.MapGet("/api/medicamentos", async (AppDbContext db) => {
    try
    {
        var listaMed = await db.Medicamentos.AsNoTracking().ToListAsync();
        var listaLotes = await db.Lotes.AsNoTracking().ToListAsync();

        foreach (var med in listaMed)
        {
            med.Lotes = listaLotes.Where(l => l.MedicamentoId == med.Id).ToList();
        }
        return Results.Ok(listaMed);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});
// Obtener todos los lotes
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

// Guardar Usuario (Sincronización)
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

// Guardar Medicamento con sus Lotes
app.MapPost("/api/medicamentos", async (Medicamento med, AppDbContext db) => {
    try
    {
        if (string.IsNullOrEmpty(med.Id)) med.Id = Guid.NewGuid().ToString();

        db.Medicamentos.Add(med);
        await db.SaveChangesAsync();

        if (med.Lotes != null && med.Lotes.Count > 0)
        {
            foreach (var lote in med.Lotes)
            {
                if (string.IsNullOrEmpty(lote.Id)) lote.Id = Guid.NewGuid().ToString();
                lote.MedicamentoId = med.Id;
                lote.FechaCaducidad = DateTime.SpecifyKind(lote.FechaCaducidad, DateTimeKind.Utc);
                db.Lotes.Add(lote);
            }
            await db.SaveChangesAsync();
        }
        return Results.Created($"/api/medicamentos/{med.Id}", med);
    }
    catch (Exception ex) { return Results.Problem(ex.Message); }
});

// Guardar Venta
app.MapPost("/api/ventas", async (Venta venta, AppDbContext db) => {
    try
    {
        if (string.IsNullOrEmpty(venta.Id)) venta.Id = Guid.NewGuid().ToString();
        venta.Fecha = DateTime.SpecifyKind(venta.Fecha, DateTimeKind.Utc);
        if (venta.ClienteId == "mostrador" || string.IsNullOrEmpty(venta.ClienteId))
        {
            venta.ClienteId = null;
        }
        else
        {
            var existe = await db.Usuarios.AnyAsync(u => u.Id == venta.ClienteId);
            if (!existe) venta.ClienteId = null;
        }

        db.Ventas.Add(venta);
        await db.SaveChangesAsync();
        return Results.Created($"/api/ventas/{venta.Id}", venta);
    }
    catch (Exception ex) { return Results.Problem(ex.Message); }
});

// EDITAR medicamento y sus lotes asociados
app.MapPut("/api/medicamentos/{id}", async (string id, Medicamento med, AppDbContext db) => {
    var existente = await db.Medicamentos.FindAsync(id);
    if (existente == null) return Results.NotFound();

    existente.Nombre = med.Nombre;
    existente.Descripcion = med.Descripcion;
    existente.Stock = med.Stock;
    existente.Precio = med.Precio;
    existente.Categoria = med.Categoria;
    existente.Subcategoria = med.Subcategoria;
    existente.RequiereReceta = med.RequiereReceta;
    existente.UrlImagen = med.UrlImagen;
    existente.Telefono = med.Telefono;

        if (med.Lotes != null)
        {
            var lotesViejos = db.Lotes.Where(l => l.MedicamentoId == id);
            db.Lotes.RemoveRange(lotesViejos);
            foreach (var lote in med.Lotes)
            {
                if (string.IsNullOrEmpty(lote.Id)) lote.Id = Guid.NewGuid().ToString();
                lote.MedicamentoId = id;
                lote.FechaCaducidad = DateTime.SpecifyKind(lote.FechaCaducidad, DateTimeKind.Utc);
                db.Lotes.Add(lote);
            }
        }
        await db.SaveChangesAsync();
        return Results.NoContent();
});

// ELIMINAR un medicamento
app.MapDelete("/api/medicamentos/{id}", async (string id, AppDbContext db) => {
    var med = await db.Medicamentos.FindAsync(id);
    if (med == null) return Results.NotFound();

    db.Medicamentos.Remove(med);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Medicamento eliminado" });
});

// EDITAR un lote
app.MapPut("/api/lotes/{id}", async (string id, Lote lote, AppDbContext db) => {
    var existente = await db.Lotes.FindAsync(id);
    if (existente == null) return Results.NotFound();

    existente.Cantidad = lote.Cantidad;
    existente.Codigo = lote.Codigo;
    existente.FechaCaducidad = DateTime.SpecifyKind(lote.FechaCaducidad, DateTimeKind.Utc);

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ELIMINAR un lote
app.MapDelete("/api/lotes/{id}", async (string id, AppDbContext db) => {
    var lote = await db.Lotes.FindAsync(id);
    if (lote == null) return Results.NotFound();

    db.Lotes.Remove(lote);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Lote eliminado" });
});

app.Run();
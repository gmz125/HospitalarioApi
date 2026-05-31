using HospitalarioApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalarioApi
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
//registro de tablas
        public DbSet<Medicamento> Medicamentos => Set<Medicamento>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Lote> Lotes => Set<Lote>();
        public DbSet<Venta> Ventas => Set<Venta>();
    }
}
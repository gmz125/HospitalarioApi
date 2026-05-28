using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalarioApi.Models
{
    [Table("ventas")]
    public class Venta
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("cliente_id")]
        public string? ClienteId { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("nombre_negocio")]
        public string NombreNegocio { get; set; } = string.Empty;

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
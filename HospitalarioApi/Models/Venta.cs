using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HospitalarioApi.Models
{
    [Table("ventas")]
    public class Venta
    {
        [Column("id")]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [Column("cliente_id")]
        [JsonPropertyName("cliente_id")]
        public string? ClienteId { get; set; }

        [Column("medicamento_id")]
        [JsonPropertyName("medicamento_id")]
        public Guid? MedicamentoId { get; set; }

        [Column("nombre_medicamento")]
        [JsonPropertyName("nombre_medicamento")]
        public string? NombreMedicamento { get; set; }

        [Column("cantidad")]
        [JsonPropertyName("cantidad")]
        public int Cantidad { get; set; }

        [Column("total")]
        [JsonPropertyName("total")]
        public decimal Total { get; set; }

        [Column("nombre_negocio")]
        [JsonPropertyName("nombre_negocio")]
        public string NombreNegocio { get; set; } = string.Empty;

        [Column("fecha")]
        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
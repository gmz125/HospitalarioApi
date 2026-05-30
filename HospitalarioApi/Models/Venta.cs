using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HospitalarioApi.Models
{
    [Table("ventas")]
    public class Venta
    {
        [Column("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [Column("cliente_id")]
        [JsonPropertyName("cliente_id")]
        public string? ClienteId { get; set; }

        [Column("medicamento_id")]
        [JsonPropertyName("medicamento_id")]
        public string? MedicamentoId { get; set; }

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

        [Column("requiere_receta")]
        [JsonPropertyName("requiere_receta")]
        public bool RequiereReceta { get; set; }

        [Column("fecha")]
        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}

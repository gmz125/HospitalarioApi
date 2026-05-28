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

        [Column("total")]
        [JsonPropertyName("total")]
        public decimal Total { get; set; }

        [Column("nombre_negocio")]
        [JsonPropertyName("nombre_negocio")]
        public string? NombreNegocio { get; set; }

        [Column("fecha")]
        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; }
    }
}
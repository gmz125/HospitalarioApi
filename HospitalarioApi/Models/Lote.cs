using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HospitalarioApi.Models
{
    [Table("lotes")]
    public class Lote
    {
        [Column("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [Column("medicamento_id")]
        [JsonPropertyName("medicamento_id")]
        public string MedicamentoId { get; set; } = string.Empty;

        [Column("codigo")]
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = "S/N";

        [Column("fecha_caducidad")]
        [JsonPropertyName("fechaCaducidad")]
        public DateTime FechaCaducidad { get; set; }

        [Column("cantidad")]
        [JsonPropertyName("cantidad")]
        public int Cantidad { get; set; }
    }
}
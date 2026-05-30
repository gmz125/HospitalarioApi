using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HospitalarioApi.Models
{
    [Table("medicamentos")]
    public class Medicamento
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [Column("usuario_id")]
        [JsonPropertyName("usuario_id")]
        public string? UsuarioId { get; set; }

        [Column("nombre")]
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }

        [Column("precio")]
        [JsonPropertyName("precio")]
        public decimal Precio { get; set; }

        [Column("requiere_receta")]
        [JsonPropertyName("requiere_receta")]
        public bool RequiereReceta { get; set; }

        [Column("categoria")]
        [JsonPropertyName("categoria")]
        public string? Categoria { get; set; }

        [Column("subcategoria")]
        [JsonPropertyName("subcategoria")]
        public string? Subcategoria { get; set; }

        [Column("nombre_negocio")]
        [JsonPropertyName("nombre_negocio")]
        public string? NombreNegocio { get; set; }

        [Column("url_imagen")]
        [JsonPropertyName("url_imagen")]
        public string? UrlImagen { get; set; }

        [Column("telefono_contacto")]
        [JsonPropertyName("telefono_contacto")]
        public string? Telefono { get; set; }

        [NotMapped]
        [JsonPropertyName("lotes")]
        public List<Lote> Lotes { get; set; } = new();
    }
}

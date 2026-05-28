using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalarioApi.Models
{
    [Table("medicamentos")]
    public class Medicamento
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("usuario_id")]
        public string? UsuarioId { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }

        [Column("categoria")]
        public string? Categoria { get; set; }

        [Column("nombre_negocio")]
        public string? NombreNegocio { get; set; }
        [Column("subcategoria")] public string? Subcategoria { get; set; }
        [Column("url_imagen")] public string? UrlImagen { get; set; }
        [Column("telefono_contacto")] public string? Telefono { get; set; }
        [NotMapped]
        public List<Lote> Lotes { get; set; } = new();
    }
}

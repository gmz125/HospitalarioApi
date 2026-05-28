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
    }
}

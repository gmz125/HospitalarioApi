using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalarioApi.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Column("id")]
        public string Id { get; set; } = string.Empty;

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("rol")]
        public string? Rol { get; set; }
    }
}
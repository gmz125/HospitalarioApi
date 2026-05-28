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

        [Column("primer_apellido")] 
        public string PrimerApellido { get; set; } = string.Empty;

        [Column("segundo_apellido")]
        public string? SegundoApellido { get; set; }

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("rol")]
        public string? Rol { get; set; }

        [Column("nombre_negocio")]
        public string? NombreNegocio { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("direccion")]
        public string? Direccion { get; set; }
    }
}
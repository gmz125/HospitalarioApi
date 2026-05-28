using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalarioApi.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;

    namespace HospitalarioApi.Models
    {
        [Table("usuarios")]
        public class Usuario
        {
            [Column("id")]
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [Column("nombre")]
            [JsonPropertyName("nombre")]
            public string Nombre { get; set; } = string.Empty;

            [Column("primer_apellido")]
            [JsonPropertyName("primer_apellido")]
            public string PrimerApellido { get; set; } = string.Empty;

            [Column("segundo_apellido")]
            [JsonPropertyName("segundo_apellido")]
            public string? SegundoApellido { get; set; }

            [Column("email")]
            [JsonPropertyName("email")]
            public string Email { get; set; } = string.Empty;

            [Column("rol")]
            [JsonPropertyName("rol")]
            public string? Rol { get; set; }

            [Column("nombre_negocio")]
            [JsonPropertyName("nombre_negocio")]
            public string? NombreNegocio { get; set; }

            [Column("telefono")]
            [JsonPropertyName("telefono")]
            public string? Telefono { get; set; }

            [Column("direccion")]
            [JsonPropertyName("direccion")]
            public string? Direccion { get; set; }
        }
    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalarioApi.Models
{
    [Table("lotes")]
    public class Lote
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("medicamento_id")] // Relación con el medicamento
        public Guid MedicamentoId { get; set; }

        [Column("codigo")]
        public string Codigo { get; set; } = "S/N";

        [Column("fecha_caducidad")]
        public DateTime FechaCaducidad { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }
    }
}
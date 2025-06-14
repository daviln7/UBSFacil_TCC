using SQLite;

namespace UBSFacil.Models
{
    [Table("Agendamentos")]
    public class Agendamento
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int UsuarioId { get; set; } 

        public DateTime DataAgendamento { get; set; }

        [MaxLength(100)]
        public string UnidadeSaude { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Observacoes { get; set; } = string.Empty;
    }
}


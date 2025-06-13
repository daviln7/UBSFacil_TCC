// Models/UnidadeSaude.cs
using SQLite;
namespace UBSFacil.Models
{
    [Table("UnidadesSaude")]
    public class UnidadeSaude
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Endereco { get; set; } = string.Empty;

        public string Imagem { get; set; } = string.Empty;
    }
}

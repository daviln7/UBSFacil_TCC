using SQLite;

namespace UBSFacil.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        public string NomeCompleto { get; set; } = string.Empty;

        [MaxLength(100), Unique]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefone { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Senha { get; set; } = string.Empty;

    }
}




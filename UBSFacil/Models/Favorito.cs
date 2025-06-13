using SQLite;

namespace UBSFacil.Models
{
    [Table("Favoritos")]
    public class Favorito
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed(Name = "IX_FavoritoUnique", Order = 1, Unique = true)]
        public int UsuarioId { get; set; }

        [Indexed(Name = "IX_FavoritoUnique", Order = 2, Unique = true)]
        public int UnidadeId { get; set; }
    }
}

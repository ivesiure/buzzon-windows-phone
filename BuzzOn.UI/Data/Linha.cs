namespace BuzzOn.UI.Data
{
    public partial class Linha
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Cor { get; set; }

        public string Nome { get; set; }

        public string Categoria { get; set; }

        public bool Favorita { get; set; }
    }
}

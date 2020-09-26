namespace BuzzOn.UI.Data
{
    public partial class Versao
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        public string Tabela { get; set; }

        public string Codigo { get; set; }

        public string Valor { get; set; }
    }
}

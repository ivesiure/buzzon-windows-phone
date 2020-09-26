namespace BuzzOn.UI.Data
{
    public class Parametro
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Valor { get; set; }
    }
}

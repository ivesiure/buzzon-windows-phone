namespace BuzzOn.UI.Data
{
    public class Ponto
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Nome { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Sentido { get; set; }
    }
}

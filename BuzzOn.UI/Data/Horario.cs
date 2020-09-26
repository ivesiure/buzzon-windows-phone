namespace BuzzOn.UI.Data
{
    public class Horario
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Ponto { get; set; }

        public string Dia { get; set; }

        public string Hora { get; set; }
    }
}

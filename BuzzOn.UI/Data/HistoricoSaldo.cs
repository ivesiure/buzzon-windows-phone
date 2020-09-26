using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Data
{
    public class HistoricoSaldo
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        public int IdPerfilSaldo { get; set; }

        public string Local { get; set; }

        public DateTime? DtConsumo { get; set; }

        public decimal? ValorConsumo { get; set; }
    }
}

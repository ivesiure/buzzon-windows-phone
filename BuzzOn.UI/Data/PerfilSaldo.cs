using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Data
{
    public class PerfilSaldo
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        public string Nome{ get; set; }

        public string CPF { get; set; }

        public long NrCartao { get; set; }

        public decimal? UltimoSaldo { get; set; }

        public DateTime? DtUltimaAtualizacao { get; set; }
    }
}

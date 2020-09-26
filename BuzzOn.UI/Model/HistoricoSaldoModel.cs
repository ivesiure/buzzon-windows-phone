using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BuzzOn.UI.Model
{
    public class HistoricoSaldoModel
    {
        public String Local { get; set; }

        private String _dtConsumo;
        public String DtConsumo
        {
            get { return _dtConsumo; }
            set { _dtConsumo = value; }
        }

        private String _valorConsumo;
        public String ValorConsumo
        {
            get { return _valorConsumo; }
            set { _valorConsumo = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.ViewModels
{
    public class HistoricoSaldoViewModel
    {
        private String _result;
        public String result
        {
            get { return _result ?? ""; }
            set { _result = value; }
        }

        private String _msg;
        public String msg
        {
            get { return _msg ?? ""; }
            set { _msg = value; }
        }

        public List<ItemSaldoViewModel> extrato { get; set; }

        public HistoricoSaldoViewModel()
        {
            this.extrato = new List<ItemSaldoViewModel>();
        }

        public class ItemSaldoViewModel
        {
            private String _local;
            public String local
            {
                get { return _local ?? ""; }
                set { _local = value; }
            }

            private String _data;
            public String data
            {
                get { return _data ?? ""; }
                set { _data = value; }
            }

            private String _valor;
            public String valor
            {
                get { return _valor ?? ""; }
                set { _valor = value; }
            }

            private String _saldo;
            public String saldo
            {
                get { return _saldo ?? ""; }
                set { _saldo = value; }
            }
        }
    }
}

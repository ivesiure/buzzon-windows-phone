using BuzzOn.UI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Model
{
    public class ParametroModel
    {
        public Int32 Id { get; set; }
        public String Nome { get; set; }
        public String Valor { get; set; }

        public ParametroModel()
        {

        }

        public ParametroModel(Parametro parametro)
        {
            this.Id = parametro.Id;
            this.Nome = parametro.Nome;
            this.Valor = parametro.Valor;
        }
    }
}

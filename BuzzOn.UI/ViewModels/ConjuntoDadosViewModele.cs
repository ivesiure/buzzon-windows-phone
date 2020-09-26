using BuzzOn.UI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.ViewModels
{
    public class ConjuntoDadosViewModel
    {
        public String Token { get; set; }
        public List<LinhaModel> Linhas { get; set; }
        public List<HorarioModel> Horarios { get; set; }
        public ConjuntoDadosVersaoViewModel Versoes { get; set; }

        public class ConjuntoDadosVersaoViewModel
        {
            public String Linhas { get; set; }
            public List<CodigoValorViewModel> Horarios { get; set; }

            public ConjuntoDadosVersaoViewModel()
            {
                this.Horarios = new List<CodigoValorViewModel>();
            }

            public class CodigoValorViewModel
            {
                public String Codigo { get; set; }
                public String Valor { get; set; }
            }
        }
    }
}

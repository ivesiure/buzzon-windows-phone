using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.ViewModels
{
    public class DiaSemanaViewModel
    {
        private String _nomeDiaSemana;
        public String NomeDiaSemana
        {
            get { return _nomeDiaSemana; }
            set
            {
                var diaSemana = DateTime.Today.DayOfWeek;
                switch (value)
                {
                    case "U":
                        _nomeDiaSemana = "Dias Úteis";
                        this.Prioridade = (diaSemana == DayOfWeek.Monday || diaSemana == DayOfWeek.Tuesday || diaSemana == DayOfWeek.Wednesday || diaSemana == DayOfWeek.Thursday || diaSemana == DayOfWeek.Friday) ? 1 : 0;
                        break;
                    case "S": 
                        _nomeDiaSemana = "Sábado";
                        this.Prioridade = diaSemana == DayOfWeek.Saturday ? 1 : 0;
                        break;
                    case "D": 
                        _nomeDiaSemana = "Domingo";
                        this.Prioridade = diaSemana == DayOfWeek.Sunday ? 1 : 0;
                        break;
                    case "F": _nomeDiaSemana = "Feriado";
                        this.Prioridade = 0;
                        break;
                    default: _nomeDiaSemana = ""; break;
                }
            }
        }
        public int Prioridade { get; set; }
        public List<PontoViewModel> PONTOS { get; set; }

        public DiaSemanaViewModel()
        {
            this.PONTOS = new List<PontoViewModel>();
        }
    }

    public class PontoViewModel
    {
        public String Nome { get; set; }

        public String Horarios { get; set; }
    }
}

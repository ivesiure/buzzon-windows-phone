using BuzzOn.UI.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Model
{
    public class HorarioModel
    {
        public String Codigo { get; set; }
        public List<PontoModel> Pontos { get; set; }

        public HorarioModel()
        {
            this.Pontos = new List<PontoModel>();
        }

        public HorarioModel(String Codigo, JObject firebaseObject)
        {
            this.Pontos = new List<PontoModel>();
            this.Codigo = Codigo;

            this.Pontos = firebaseObject.Properties().Select(i => new PontoModel()
            {
                Nome = i.Name,
                Dias = i.Children<JObject>().Properties().Select(j => new DiaModel
                {
                    DiaSemana = j.Name,
                    Horarios = j.First().Children().Select(k => k.ToString()).ToList()
                }).ToList()
            }).ToList();

        }

        #region Internal Classes

        public class PontoModel
        {
            public String Nome { get; set; }
            public List<DiaModel> Dias { get; set; }

            public PontoModel()
            {
                this.Dias = new List<DiaModel>();
            }
        }

        public class DiaModel
        {
            public String DiaSemana { get; set; }
            public List<String> Horarios { get; set; }
            public string HorarioCompacto
            {
                get
                {
                    return string.Join("-", Horarios);
                }
            }

            public DiaModel()
            {
                this.Horarios = new List<string>();
            }
        }

        #endregion
    }
}

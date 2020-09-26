using BuzzOn.UI.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Model
{
    public class VersaoModel
    {
        public Int32 Id { get; set; }
        public String Tabela { get; set; }
        public String Codigo { get; set; }
        public String Valor { get; set; }


        public String Linhas { get; set; }
        public Dictionary<String, String> Horarios { get; set; }
        public Dictionary<String, String> Pontos { get; set; }
        public Dictionary<String, String> Rotas { get; set; }

        public VersaoModel()
        {
            Horarios = new Dictionary<String, String>();
            Pontos = new Dictionary<String, String>();
            Rotas = new Dictionary<String, String>();
            Linhas = String.Empty;
        }

        public VersaoModel(JObject jObject)
        {
            this.Linhas = jObject["linhas"].ToString();
            this.Horarios = jObject["horarios"] != null ? jObject["horarios"].ToDictionary(i => i.Path.Replace("horarios.", ""), i => i.Children().First().ToString()) : new Dictionary<string, string>(); ;
            this.Pontos = jObject["pontos"] != null ? jObject["pontos"].ToDictionary(i => i.Path.Replace("pontos.", ""), i => i.Children().First().ToString()) : new Dictionary<string, string>();
            this.Rotas = jObject["rotas"] != null ? jObject["rotas"].ToDictionary(i => i.Path.Replace("rotas.", ""), i => i.Children().First().ToString()) : new Dictionary<string, string>();
        }

        public VersaoModel(Versao versao)
        {
            this.Tabela = versao.Tabela;
            this.Codigo = versao.Codigo;
            this.Valor = versao.Valor;
            this.Id = versao.Id;
        }
    }
}

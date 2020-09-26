using BuzzOn.UI.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Model
{
    public class PontoModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Nome { get; set; }
        public string Sentido { get; set; }
        public string Tipo { get; set; }

        public PontoModel() { }

        public PontoModel(JToken jToken)
        {
            this.Latitude = jToken["latitude"].ToString();
            this.Longitude = jToken["longitude"].ToString();
            this.Nome = jToken["nome"].ToString();
            this.Sentido = jToken["sentido"].ToString();
            this.Tipo = jToken["tipo"].ToString();
        }

        public PontoModel(Ponto ponto)
        {
            this.Latitude = ponto.Latitude;
            this.Longitude = ponto.Longitude;
            this.Nome = ponto.Nome;
            this.Sentido = ponto.Sentido;
            this.Id = ponto.Id;
        }
    }
}

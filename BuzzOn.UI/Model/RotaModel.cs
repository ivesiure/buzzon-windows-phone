using BuzzOn.UI.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Model
{
    public class RotaModel
    {
        public Int32 Id { get; set; }
        public string Codigo { get; set; }
        public String Latitude { get; set; }
        public String Longitude { get; set; }

        public RotaModel() { }

        public RotaModel(JToken jToken)
        {
            this.Latitude = jToken["latitude"].Value<String>();
            this.Longitude = jToken["longitude"].Value<String>();
        }

        public RotaModel(Rota rota)
        {
            this.Latitude = rota.Latitude;
            this.Longitude = rota.Longitude;
            this.Codigo = rota.Codigo;
            this.Id = rota.Id;
        }
    }
}

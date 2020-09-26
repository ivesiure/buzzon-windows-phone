using BuzzOn.UI.Data;
using BuzzOn.UI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Facade
{
    public class RotaFacade
    {
        public static RotaFacade Instance { get { return new RotaFacade(); } }

        public List<RotaModel> ListarRotaPorLinha(string pCodigo)
        {
            return BuzzOnConnection.Instance.List<Rota>(i => i.Codigo == pCodigo).Select(i => new RotaModel(i)).ToList();
        }

        public Boolean PossuiRotas(string pCodigo)
        {
            return BuzzOnConnection.Instance.Any<Rota>(i => i.Codigo == pCodigo);
        }

        public void AdicionarLista(List<RotaModel> rotas)
        {
            var lista = rotas.Select(r => new Rota { Latitude = r.Latitude, Longitude = r.Longitude, Codigo = r.Codigo });
            BuzzOnConnection.Instance.InsertAll<Rota>(lista);
        }

        public void RemoverRotaDaLinha(string pCodigo)
        {
            BuzzOnConnection.Instance.DeleteAll<Rota>(i => i.Codigo == pCodigo);
        }
    }
}

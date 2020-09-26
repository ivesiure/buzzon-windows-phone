using BuzzOn.UI.Data;
using BuzzOn.UI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Facade
{
    public class PontoFacade
    {
        public static PontoFacade Instance { get { return new PontoFacade(); } }

        public List<PontoModel> ListarPontosPorLinha(String pCodigo)
        {
            return BuzzOnConnection.Instance.List<Ponto>(i => i.Codigo == pCodigo).Select(i => new PontoModel(i)).ToList();
        }

        public void AdicionarLista(List<PontoModel> pontos)
        {
            var listaPontos = pontos.Select(i => new Ponto { Codigo = i.Codigo, Latitude = i.Latitude, Longitude = i.Longitude, Nome = i.Nome, Sentido = i.Sentido }).ToList();
            BuzzOnConnection.Instance.InsertAll<Ponto>(listaPontos);
        }

        public void RemoverPontosDaLinha(string pCodigo)
        {
            BuzzOnConnection.Instance.DeleteAll<Ponto>(i => i.Codigo == pCodigo);
        }
    }
}

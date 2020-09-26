using BuzzOn.UI.Model;
using BuzzOn.UI.Model.TableEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Business
{
    public class PontoBusiness
    {
        private BuzzOnContext buzzOnContext;

        public PontoBusiness(BuzzOnContext pBuzzOnDbContext)
        {
            this.buzzOnContext = pBuzzOnDbContext;
        }

        public List<Ponto> ListarPontosPorLinha(Int32 pLinhaId)
        {
            return buzzOnContext.Pontos.Where(i => i.LinhaId == pLinhaId).OrderBy(i => i.Nome).ToList();
        }

        public void AdicionarLista(List<Ponto> pontos)
        {
            buzzOnContext.Pontos.InsertAllOnSubmit(pontos);
        }
    }
}

using BuzzOn.UI.Model;
using BuzzOn.UI.Model.TableEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Business
{
    public class RotaBusiness
    {
        private BuzzOnContext buzzOnContext;

        public RotaBusiness(BuzzOnContext pBuzzOnDbContext)
        {
            this.buzzOnContext = pBuzzOnDbContext;
        }

        public List<Rota> ListarRotaPorLinha(Int32 pLinhaId)
        {
            return buzzOnContext.Rotas.Where(i => i.LinhaId == pLinhaId).ToList();
        }

        public void AdicionarLista(List<Rota> rotas)
        {
            buzzOnContext.Rotas.InsertAllOnSubmit(rotas);
        }

        public Boolean PossuiRotas(Int32 pLinhaId)
        {
            return buzzOnContext.Rotas.Any(i => i.LinhaId == pLinhaId);
        }
    }
}

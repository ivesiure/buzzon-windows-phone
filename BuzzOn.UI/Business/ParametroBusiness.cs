using BuzzOn.UI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Business
{
    public class ParametroBusiness
    {
        private BuzzOnContext buzzOnContext;

        public ParametroBusiness(BuzzOnContext pBuzzOnDbContext)
        {
            this.buzzOnContext = pBuzzOnDbContext;
        }

        public Parametro ObterPorNome(String pNome)
        {
            return buzzOnContext.Parametro.FirstOrDefault(i => i.Nome == pNome);
        }

        public void Inserir(Parametro parametro)
        {
            buzzOnContext.Parametro.InsertOnSubmit(parametro);
        }
    }
}

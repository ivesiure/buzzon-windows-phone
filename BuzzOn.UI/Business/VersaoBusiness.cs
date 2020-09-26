using BuzzOn.UI.Model;
using BuzzOn.UI.Model.TableEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Business
{
    public class VersaoBusiness
    {
        private BuzzOnContext buzzOnContext;

        public VersaoBusiness(BuzzOnContext pBuzzOnDbContext)
        {
            this.buzzOnContext = pBuzzOnDbContext;
        }

        public void AdicionarLista(List<Versao> versoes)
        {
            buzzOnContext.Versao.InsertAllOnSubmit(versoes);
        }

        public bool VerificaExistemAtualizacoes(List<Versao> versoes)
        {
            var todasAsVersoes = buzzOnContext.Versao.ToList();

            //var linhasDiferentes_1 = todasAsVersoes.Where(i => versoes.Any(j => j.NomeTabela == i.NomeTabela && j.CodigoLinha == i.CodigoLinha && j.ValorVersao != i.ValorVersao)).ToList();

            foreach (var item in versoes)
            {
                var ver = todasAsVersoes.FirstOrDefault(i => i.CodigoLinha == item.CodigoLinha && i.NomeTabela == item.NomeTabela);
                if (ver != null && item.ValorVersao != ver.ValorVersao)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

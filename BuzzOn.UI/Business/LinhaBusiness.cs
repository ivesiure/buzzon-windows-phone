using BuzzOn.UI.Model;
using BuzzOn.UI.Model.TableEntities;
using BuzzOn.UI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace BuzzOn.UI.Business
{
    public class LinhaBusiness
    {
        private BuzzOnContext buzzOnContext;

        public LinhaBusiness(BuzzOnContext pBuzzOnContext)
        {
            this.buzzOnContext = pBuzzOnContext;
        }

        public Linha ObterPorId(Int32 pLinhaOnibusId)
        {
            return buzzOnContext.Linhas.FirstOrDefault(i => i.Id == pLinhaOnibusId);
        }

        public List<Linha> ListarTodas()
        {
            return buzzOnContext.Linhas.OrderBy(i => i.Nome).ToList();
        }

        public List<Linha> ListarPorFiltro(Expression<Func<Linha, bool>> filter)
        {
            return buzzOnContext.Linhas.Where(filter).OrderBy(i => i.Nome).ToList();
        }

        public void Favoritar(String pCodigo)
        {
            var linha = buzzOnContext.Linhas.FirstOrDefault(i => i.Codigo == pCodigo);
            linha.Favorita = true;
        }

        public void Desfavoritar(String pCodigo)
        {
            var linha = buzzOnContext.Linhas.FirstOrDefault(i => i.Codigo == pCodigo);
            linha.Favorita = false;
        }

        public void AdicionarLista(List<Linha> lista)
        {
            buzzOnContext.Linhas.InsertAllOnSubmit(lista);
        }

        public void AdicionarListaPrimeiraInicializacao(List<BuzzOn.Model.Linha> lista)
        {
            var listaTbEntity = lista.Select(i => new Linha
            {
                Nome = i.Nome,
                Codigo = i.Codigo,
                Cor = i.Cor,
                Categoria = i.Categoria,
                Favorita = false
            }).ToList();
            buzzOnContext.Linhas.InsertAllOnSubmit(listaTbEntity);
        }

        public void AtualizarListaDeLinhas(List<BuzzOn.Model.Linha> listaLinhas, Versao versaoLinhas)
        {
            var todasAsLinhas = buzzOnContext.Linhas.ToList();

            var versao = buzzOnContext.Versao.FirstOrDefault(i => i.NomeTabela == "Linha");
            if (versao != null && versao.ValorVersao != versaoLinhas.ValorVersao)
            {
                var linhasNaoListadas = todasAsLinhas.Where(i => !listaLinhas.Any(j => j.Codigo == i.Codigo)).ToList();
                foreach (var item in listaLinhas)
                {
                    var contextObject = buzzOnContext.Linhas.FirstOrDefault(i => i.Codigo == item.Codigo);
                    if (contextObject != null)
                    {
                        contextObject.Codigo = item.Codigo;
                        contextObject.Cor = item.Cor;
                        contextObject.Categoria = item.Categoria;
                        contextObject.Nome = item.Nome;
                    }
                    else
                    {
                        var novaLinha = new Linha
                        {
                            Codigo = item.Codigo,
                            Cor = item.Cor,
                            Categoria = item.Categoria,
                            Nome = item.Nome,
                        };
                        buzzOnContext.Linhas.InsertOnSubmit(novaLinha);
                    }
                }
                if (linhasNaoListadas.Any())
                {
                    foreach (var item in linhasNaoListadas)
                    {
                        var contextObject = buzzOnContext.Linhas.FirstOrDefault(i => i.Codigo == item.Codigo);
                        if (contextObject != null)
                            buzzOnContext.Linhas.DeleteOnSubmit(contextObject);
                    }
                }

                versao.ValorVersao = versaoLinhas.ValorVersao;
                
                buzzOnContext.SubmitChanges();

            }
        }
    }
}

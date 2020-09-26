using BuzzOn.UI.Data;
using BuzzOn.UI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Facade
{
    public class LinhaFacade
    {
        public static LinhaFacade Instance { get { return new LinhaFacade(); } }

        public LinhaModel ObterPorId(Int32 IdLinha)
        {
            return new LinhaModel(BuzzOnConnection.Instance.Get<Linha>(i => i.Id == IdLinha));
        }

        public List<LinhaModel> ListarTodas()
        {
            return BuzzOnConnection.Instance.List<Linha>().Select(i => new LinhaModel(i)).ToList();
        }

        public List<LinhaModel> ListarPorFiltro(Func<Linha, bool> filter)
        {
            return BuzzOnConnection.Instance.List<Linha>(filter).Select(i => new LinhaModel(i)).ToList();
        }

        public void InserirTodos(List<LinhaModel> listaLinhas)
        {
            var listaData = listaLinhas.Select(i => new Linha
            {
                Nome = i.Nome,
                Codigo = i.Codigo,
                Cor = i.Cor,
                Categoria = i.Categoria,
                Favorita = false
            }).ToList();
            BuzzOnConnection.Instance.InsertAll<Linha>(listaData);
        }

        public void AtualizarListaDeLinhas(List<LinhaModel> list)
        {
            var conn = BuzzOnConnection.Instance;
            var favoritas = conn.List<Linha>(i => i.Favorita);
            conn.DropAndRecreate<Linha>();
            var linhas = list.Select(i => new Linha { Id = i.Id, Codigo = i.Codigo, Nome = i.Nome, Cor = i.Cor, Categoria = i.Categoria, Favorita = favoritas.Any(j => j.Codigo == i.Codigo) }).ToList();
            conn.InsertAll<Linha>(linhas);
        }

        public void AtualizarLinha(LinhaModel linhaModel)
        {
            var linha = new Linha{ Id = linhaModel.Id, Codigo = linhaModel.Codigo, Nome = linhaModel.Nome, Cor = linhaModel.Cor, Categoria = linhaModel.Categoria, Favorita = linhaModel.Favorita };
            BuzzOnConnection.Instance.Update<Linha>(linha);
        }
    }
}

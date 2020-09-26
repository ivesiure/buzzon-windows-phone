using BuzzOn.UI.Data;
using BuzzOn.UI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Facade
{
    public class VersaoFacade
    {
        public static VersaoFacade Instance { get { return new VersaoFacade(); } }

        public void InserirLista(ViewModels.ConjuntoDadosViewModel.ConjuntoDadosVersaoViewModel dados)
        {
            var listaVersoes = new List<Versao> { new Versao { Tabela = "Linha", Codigo = null, Valor = dados.Linhas } };
            listaVersoes.AddRange(dados.Horarios.Select(i => new Versao { Tabela = "Horario", Codigo = i.Codigo, Valor = i.Valor }).ToList());
            BuzzOnConnection.Instance.InsertAll<Versao>(listaVersoes);
        }

        public bool VerificaExistemAtualizacoes(List<VersaoModel> listaVersoes)
        {
            var todasAsVersoes = BuzzOnConnection.Instance.List<Versao>();

            foreach (var item in listaVersoes)
            {
                var ver = todasAsVersoes.FirstOrDefault(i => i.Codigo == item.Codigo && i.Tabela == item.Tabela);
                if (ver != null && item.Valor != ver.Valor)
                {
                    return true;
                }
            }
            return false;
        }

        public VersaoModel ObterVersaoLinha()
        {
            var versao = BuzzOnConnection.Instance.Get<Versao>(i => i.Tabela == "Linha");
            if (versao != null)
            {
                return new VersaoModel(versao);
            }
            return null;
        }

        public VersaoModel ObterVersaoRota(string codigo)
        {
            var versao = BuzzOnConnection.Instance.Get<Versao>(i => i.Tabela == "Rota" && i.Codigo == codigo);
            if (versao != null)
            {
                return new VersaoModel(versao);
            }
            return null;
        }

        public VersaoModel ObterVersaoPonto(string codigo)
        {
            var versao = BuzzOnConnection.Instance.Get<Versao>(i => i.Tabela == "Ponto" && i.Codigo == codigo);
            if (versao != null)
            {
                return new VersaoModel(versao);
            }
            return null;
        }

        public void AtualizarVersoesLinhasHorarios(ViewModels.ConjuntoDadosViewModel.ConjuntoDadosVersaoViewModel conjuntoDadosVersaoViewModel)
        {
            var versoes = new List<VersaoModel>() { new VersaoModel { Tabela = "Linha", Valor = conjuntoDadosVersaoViewModel.Linhas } };
            versoes.AddRange(conjuntoDadosVersaoViewModel.Horarios.Select(i => new VersaoModel { Tabela = "Horario", Codigo = i.Codigo, Valor = i.Valor }).ToList());

            var conn = BuzzOnConnection.Instance;
            var versoesDb = conn.List<Versao>(i => new string[] { "Linha", "Horario" }.Contains(i.Tabela));
            foreach (var item in versoes)
            {
                var entVersao = versoesDb.FirstOrDefault(i => i.Tabela == item.Tabela && i.Codigo == item.Codigo);
                if (entVersao != null)
                {
                    if (entVersao.Valor.Equals(item.Valor) == false)
                    {
                        entVersao.Valor = item.Valor;
                        conn.Update<Versao>(entVersao);
                    }
                }
                else
                {
                    conn.Insert<Versao>(new Versao { Codigo = item.Codigo, Tabela = item.Tabela, Valor = item.Valor });
                }
            }
        }

        public void Inserir(Versao versao)
        {
            BuzzOnConnection.Instance.Insert<Versao>(versao);
        }

        public void AtualizarVersaoPonto(string codigo, string vPontos)
        {
            var conn = BuzzOnConnection.Instance;
            var versao = conn.Get<Versao>(i => i.Codigo == codigo && i.Tabela == "Ponto");
            if (versao != null)
            {
                versao.Valor = vPontos;
                conn.Update<Versao>(versao);
            }
            else
            {
                versao = new Versao { Codigo = codigo, Tabela = "Ponto", Valor = vPontos };
                conn.Insert<Versao>(versao);
            }

        }

        public void AtualizarVersaoRotas(string codigo, string vRotas)
        {
            var conn = BuzzOnConnection.Instance;
            var versao = conn.Get<Versao>(i => i.Codigo == codigo && i.Tabela == "Rota");
            if (versao != null)
            {
                versao.Valor = vRotas;
                conn.Update<Versao>(versao);
            }
            else
            {
                versao = new Versao { Codigo = codigo, Tabela = "Rota", Valor = vRotas };
                conn.Insert<Versao>(versao);
            }
        }
    }
}

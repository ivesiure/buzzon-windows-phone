using BuzzOn.UI.Model;
using BuzzOn.UI.Model.TableEntities;
using BuzzOn.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Business
{
    public class HorarioBusiness
    {
        private BuzzOnContext buzzOnContext;

        public HorarioBusiness(BuzzOnContext pBuzzOnDbContext)
        {
            this.buzzOnContext = pBuzzOnDbContext;
        }

        public List<Horario> ListarHorariosPorCodigo(String pCodigo)
        {
            return buzzOnContext.Horarios.Where(i => i.Codigo == pCodigo).ToList();
        }

        public List<DiaSemanaViewModel> MontarTabelaHorarios(String pCodigo)
        {
            var tabelaHorarios = new List<DiaSemanaViewModel>();

            var tabelaCompleta = ListarHorariosPorCodigo(pCodigo);

            var diasSemana = tabelaCompleta.Select(i => i.Dia).Distinct().ToList();

            foreach (var dia in diasSemana)
            {
                var tabela = new DiaSemanaViewModel();
                tabela.NomeDiaSemana = dia;

                var pontos = tabelaCompleta.Where(i => i.Dia == dia).Select(i => i.Ponto).Distinct().ToList();

                foreach (var nomePonto in pontos)
                {
                    var pontoViewModel = new PontoViewModel();
                    pontoViewModel.Nome = nomePonto;

                    var horarios = tabelaCompleta.Where(i => i.Dia == dia && i.Ponto == nomePonto).Select(i => i.Hora).ToList();
                    pontoViewModel.PreencherHorarios(horarios);

                    tabela.PONTOS.Add(pontoViewModel);
                }
                tabelaHorarios.Add(tabela);
            }

            return tabelaHorarios;
        }

        public void AdicionarLista(List<Horario> horarios)
        {
            buzzOnContext.Horarios.InsertAllOnSubmit(horarios);
        }

        public bool PossuiHorarios(String pCodigo)
        {
            return buzzOnContext.Horarios.Any(i => i.Codigo == pCodigo);
        }

        public void AdicionarListaPrimeiraInicializacao(List<BuzzOn.Model.Horario> horarios)
        {
            List<Horario> listaHorariosEntity = new List<Horario>();

            foreach (var hr in horarios)
            {
                foreach (var pt in hr.Pontos)
                {
                    foreach (var dia in pt.Dias)
                    {
                        foreach (var h in dia.Horarios)
                        {
                            listaHorariosEntity.Add(new Horario
                            {
                                Codigo = hr.Codigo,
                                Ponto = pt.Nome,
                                Dia = dia.DiaSemana,
                                Hora = h
                            });
                        }
                    }
                }
            }

            buzzOnContext.Horarios.InsertAllOnSubmit(listaHorariosEntity);
        }

        public void AtualizarListaDeHorarios(List<BuzzOn.Model.Horario> listaHorarios, List<Versao> versaoHorarios)
        {
            foreach (var item in versaoHorarios)
            {
                var versao = buzzOnContext.Versao.FirstOrDefault(i => i.CodigoLinha == item.CodigoLinha && i.NomeTabela == "Horario");
                if (versao != null && item.ValorVersao != versao.ValorVersao)
                {
                    var horariosDaLinha = buzzOnContext.Horarios.Where(i => i.Codigo == item.CodigoLinha).ToList();

                    var listaHorariosLocal = listaHorarios.Where(i => i.Codigo == item.CodigoLinha).ToList();
                    List<Horario> listaHorariosEntity = new List<Horario>();
                    foreach (var hr in listaHorariosLocal)
                    {
                        foreach (var pt in hr.Pontos)
                        {
                            foreach (var dia in pt.Dias)
                            {
                                foreach (var h in dia.Horarios)
                                {
                                    listaHorariosEntity.Add(new Horario
                                    {
                                        Codigo = hr.Codigo,
                                        Ponto = pt.Nome,
                                        Dia = dia.DiaSemana,
                                        Hora = h
                                    });
                                }
                            }
                        }
                    }

                    buzzOnContext.Horarios.DeleteAllOnSubmit(horariosDaLinha);
                    buzzOnContext.Horarios.InsertAllOnSubmit(listaHorariosEntity);

                    versao.ValorVersao = item.ValorVersao;

                    buzzOnContext.SubmitChanges();
                }
            }
        }
    }

    public class DiaSemanaViewModel
    {
        private String _nomeDiaSemana;
        public String NomeDiaSemana
        {
            get { return _nomeDiaSemana; }
            set
            {
                switch (value)
                {
                    case "U": _nomeDiaSemana = "Dias Úteis"; break;
                    case "S": _nomeDiaSemana = "Sábado"; break;
                    case "D": _nomeDiaSemana = "Domingo"; break;
                    case "F": _nomeDiaSemana = "Feriado"; break;
                    default: _nomeDiaSemana = null; break;
                }
            }
        }
        public List<PontoViewModel> PONTOS { get; set; }

        public DiaSemanaViewModel()
        {
            this.PONTOS = new List<PontoViewModel>();
        }
    }

    public class PontoViewModel
    {
        public String Nome { get; set; }
        public String Horarios { get; set; }

        internal void PreencherHorarios(List<string> horarios)
        {
            if (horarios.Any())
            {
                var sbHorarios = new StringBuilder(horarios.First());
                for (int i = 1; i < horarios.Count; i++)
                {
                    sbHorarios.Append(" - " + horarios[i]);
                }
                this.Horarios = sbHorarios.ToString();
            }
        }
    }
}

using BuzzOn.UI.Data;
using BuzzOn.UI.Model;
using BuzzOn.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Facade
{
    public class HorarioFacade
    {
        public static HorarioFacade Instance { get { return new HorarioFacade(); } }

        public void InserirTodos(List<Model.HorarioModel> listaModel)
        {
            var listaHorarios = new List<Horario>();

            foreach (var hr in listaModel)
            {
                foreach (var pt in hr.Pontos)
                {
                    foreach (var dia in pt.Dias)
                    {
                        listaHorarios.Add(new Horario
                        {
                            Codigo = hr.Codigo,
                            Ponto = pt.Nome,
                            Dia = dia.DiaSemana,
                            Hora = dia.HorarioCompacto
                        });
                    }
                }
            }

            BuzzOnConnection.Instance.InsertAll<Horario>(listaHorarios);
        }

        private List<Horario> ListarHorariosPorCodigo(String CodigoLinha)
        {
            return BuzzOnConnection.Instance.List<Horario>(i => i.Codigo == CodigoLinha).ToList();
        }

        public List<DiaSemanaViewModel> MontarTabelaHorarios(String codigoLinha)
        {
            var tabelaHorarios = new List<DiaSemanaViewModel>();

            try
            {
                var tabelaCompleta = ListarHorariosPorCodigo(codigoLinha);
                var diasSemana = tabelaCompleta.Select(i => i.Dia).Distinct().OrderByDescending(i => i).ToList();
                foreach (var dia in diasSemana)
                {
                    var tabela = new DiaSemanaViewModel();
                    tabela.NomeDiaSemana = dia;

                    var pontos = tabelaCompleta.Where(i => i.Dia == dia).Select(i => i.Ponto).Distinct().ToList();

                    foreach (var nomePonto in pontos)
                    {
                        var pontoViewModel = new PontoViewModel();
                        pontoViewModel.Nome = nomePonto;

                        var pontoDia = tabelaCompleta.FirstOrDefault(i => i.Dia == dia && i.Ponto == nomePonto);
                        if (pontoDia != null)
                        {
                            pontoViewModel.Horarios = pontoDia.Hora.Replace("-", "   ");
                            tabela.PONTOS.Add(pontoViewModel);
                        }
                    }
                    tabelaHorarios.Add(tabela);
                }
            }
            catch (Exception) { }            

            return tabelaHorarios.OrderByDescending(i => i.Prioridade).ToList();
        }

        public bool PossuiHorarios(String pCodigo)
        {
            return BuzzOnConnection.Instance.Any<Horario>(i => i.Codigo == pCodigo);
        }

        public void AtualizarListaDeHorarios(List<HorarioModel> list)
        {
            var conn = BuzzOnConnection.Instance;
            conn.DropAndRecreate<Horario>();

            var horarios = new List<Horario>();
            foreach (var item in list)
            {
                foreach (var ponto in item.Pontos)
                {
                    foreach (var dia in ponto.Dias)
                    {
                        horarios.Add(new Horario
                        {
                            Codigo = item.Codigo,
                            Ponto = ponto.Nome,
                            Dia = dia.DiaSemana,
                            Hora = dia.HorarioCompacto
                        });
                    }
                }
            }
            conn.InsertAll<Horario>(horarios);
        }
    }
}

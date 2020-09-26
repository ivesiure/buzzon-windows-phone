using BuzzOn.UI.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Model
{
    public class PerfilSaldoModel : INotifyPropertyChanged
    {
        #region Private properties
        private string _nome;
        private string _CPF;
        private string _nrCartao;
        private string _ultimoSaldo;
        private string _dtUltimaAtualizacao;
        #endregion

        public int Id { get; set; }

        public string Nome
        {
            get { return _nome; }
            set
            {
                _nome = value;
                NotifyPropertyChanged("Nome");
            }
        }

        public string CPF
        {
            get { return _CPF; }
            set
            {
                _CPF = value;
                NotifyPropertyChanged("CPF");
            }
        }

        public string NrCartao
        {
            get { return _nrCartao; }
            set
            {
                _nrCartao = value;
                NotifyPropertyChanged("NrCartao");
            }
        }

        public string UltimoSaldo
        {
            get { return _ultimoSaldo; }
            set
            {
                _ultimoSaldo = value;
                NotifyPropertyChanged("UltimoSaldo");
            }
        }

        public string DtUltimaAtualizacao
        {
            get
            {
                if (String.IsNullOrEmpty(_dtUltimaAtualizacao))
                {
                    return "";
                }
                return _dtUltimaAtualizacao;
            }
            set
            {
                _dtUltimaAtualizacao = value;
                NotifyPropertyChanged("DtUltimaAtualizacao");
            }
        }

        private ObservableCollection<HistoricoSaldoModel> _historico;
        public ObservableCollection<HistoricoSaldoModel> Historico
        {
            get
            {
                return _historico;
            }
            set
            {
                _historico = value;
            }
        }

        public void AddHistorico(HistoricoSaldo hist)
        {
            _historico.Add(new HistoricoSaldoModel
            {
                DtConsumo = hist.DtConsumo.HasValue ? hist.DtConsumo.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                Local = hist.Local,
                ValorConsumo = hist.ValorConsumo.HasValue ? hist.ValorConsumo.Value.ToString("C", new CultureInfo("pt-BR")) : ""
            });
            NotifyPropertyChanged("Historico");
        }

        public void LimparHistorico()
        {
            _historico.Clear();
            NotifyPropertyChanged("Historico");
        }

        public PerfilSaldoModel()
        {
            this.Historico = new ObservableCollection<HistoricoSaldoModel>();
        }

        public PerfilSaldoModel(PerfilSaldo perfilSaldo)
        {
            this.Id = perfilSaldo.Id;
            this.Nome = perfilSaldo.Nome;
            this.CPF = "CPF: " + perfilSaldo.CPF.Insert(9, "-").Insert(6, ".").Insert(3, ".");
            this.NrCartao = "CT: " + perfilSaldo.NrCartao.ToString();
            this.UltimoSaldo = "Último Saldo: " + (perfilSaldo.UltimoSaldo.HasValue ? perfilSaldo.UltimoSaldo.Value.ToString("C", new CultureInfo("pt-BR")) : "R$ 0,00");
            this.DtUltimaAtualizacao = "Atualização: " + (perfilSaldo.DtUltimaAtualizacao.HasValue ? perfilSaldo.DtUltimaAtualizacao.Value.ToString("dd/MM/yyyy") : "--/--/----");

            var historico = BuzzOnConnection.Instance.List<HistoricoSaldo>(i => i.IdPerfilSaldo == perfilSaldo.Id).Select(hist => new HistoricoSaldoModel
                            {
                                DtConsumo = hist.DtConsumo.HasValue ? hist.DtConsumo.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                                Local = hist.Local,
                                ValorConsumo = hist.ValorConsumo.HasValue ? hist.ValorConsumo.Value.ToString("C", new CultureInfo("pt-BR")) : ""
                            }).ToList();

            this.Historico = new ObservableCollection<HistoricoSaldoModel>(historico);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}

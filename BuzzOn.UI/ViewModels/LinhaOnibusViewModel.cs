using BuzzOn.UI.Data;
using BuzzOn.UI.Facade;
using BuzzOn.UI.Model;
using BuzzOn.UI.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.ViewModels
{
    public class LinhaOnibusViewModel : INotifyPropertyChanged
    {
        private List<LinhaModel> ListaGeral;

        private ObservableCollection<LinhaModel> _linhasOnibus;
        public ObservableCollection<LinhaModel> LinhasOnibus
        {
            get { return _linhasOnibus; }
            set
            {
                _linhasOnibus = value;
                NotifyPropertyChanged("LinhasOnibus");
            }
        }

        private ObservableCollection<LinhaModel> _linhasFavoritas;
        public ObservableCollection<LinhaModel> LinhasFavoritas
        {
            get { return _linhasFavoritas; }
            set
            {
                _linhasFavoritas = value;
                NotifyPropertyChanged("LinhasFavoritas");
            }
        }

        public void CarregarColecaoDeDadosDaBase()
        {
            ListaGeral = LinhaFacade.Instance.ListarTodas();

            LinhasOnibus = new ObservableCollection<LinhaModel>(ListaGeral);
            LinhasFavoritas = new ObservableCollection<LinhaModel>(ListaGeral.Where(i => i.Favorita).OrderBy(i => i.Codigo));
        }

        public bool ColecoesForamCarregadas()
        {
            return LinhasOnibus.Any();
        }

        public void FiltrarLinhas(Func<LinhaModel, bool> filter)
        {
            var linhas = ListaGeral.Where(filter).ToList();
            LinhasOnibus = new ObservableCollection<LinhaModel>(linhas);
        }

        public void RemoverFiltro()
        {
            var todasAsLinhas = LinhaFacade.Instance.ListarTodas();
            LinhasOnibus = new ObservableCollection<LinhaModel>(todasAsLinhas);
        }

        public void FavoritarLinha(Int32 pId)
        {            
            var linhaFacInstance = LinhaFacade.Instance;

            var linha = linhaFacInstance.ObterPorId(pId);
            linha.Favorita = true;
            LinhasFavoritas.Add(linha);

            var linhaViewModel = this.LinhasOnibus.FirstOrDefault(i => i.Id == pId);
            linhaViewModel.Favorita = true;

            linhaFacInstance.AtualizarLinha(linha);
        }

        public void DesfavoritarLinha(Int32 pId)
        {
            var linhaFacInstance = LinhaFacade.Instance;

            var linha = linhaFacInstance.ObterPorId(pId);
            linha.Favorita = false;

            var linhaLocal = LinhasFavoritas.FirstOrDefault(i => i.Id == pId);
            LinhasFavoritas.Remove(linhaLocal);

            var linhaViewModel = this.LinhasOnibus.FirstOrDefault(i => i.Id == pId);
            linhaViewModel.Favorita = false;

            linhaFacInstance.AtualizarLinha(linha);
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

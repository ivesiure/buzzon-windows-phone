using BuzzOn.UI.Data;
using BuzzOn.UI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.ViewModels
{
    public class PerfilSaldoViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<PerfilSaldoModel> _listaPerfisSaldo;
        public ObservableCollection<PerfilSaldoModel> ListaPerfisSaldo
        {
            get { return _listaPerfisSaldo; }
            set
            {
                _listaPerfisSaldo = value;
                NotifyPropertyChanged("ListaPerfisSaldo");
            }
        }

        public void CarregarColecaoDeDadosDaBase()
        {
            var perfis = BuzzOnConnection.Instance.List<PerfilSaldo>().Select(i => new PerfilSaldoModel(i)).ToList();
            ListaPerfisSaldo = new ObservableCollection<PerfilSaldoModel>(perfis);
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
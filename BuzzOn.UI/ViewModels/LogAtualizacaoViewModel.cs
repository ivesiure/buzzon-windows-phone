using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BuzzOn.UI.ViewModels
{
    public class LogAtualizacaoViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private String _Titulo;
        private String _SubTitulo;
        private Visibility _Visibilidade;

        public String Titulo
        {
            get { return _Titulo; }
            set
            {
                if (_Titulo != value)
                {
                    NotifyPropertyChanging("Titulo");
                    _Titulo = value;
                    NotifyPropertyChanged("Titulo");
                }
            }
        }

        public String SubTitulo
        {
            get { return _SubTitulo; }
            set
            {
                if (_SubTitulo != value)
                {
                    NotifyPropertyChanging("SubTitulo");
                    _SubTitulo = value;
                    NotifyPropertyChanged("SubTitulo");
                }
            }
        }

        public Visibility Visibilidade
        {
            get { return _Visibilidade; }
            set
            {
                if (_Visibilidade != value)
                {
                    NotifyPropertyChanging("Visibilidade");
                    _Visibilidade = value;
                    NotifyPropertyChanged("Visibilidade");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}

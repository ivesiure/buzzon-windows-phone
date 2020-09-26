using BuzzOn.UI.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BuzzOn.UI.Model
{
    [Table]
    public class Parametro
    {
        #region Private Objects
        private int _Id;
        private String _Nome;
        private String _Valor;
        #endregion

        #region Properties
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL IDENTITY (1,1)", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get { return _Id; }
            set
            {
                if (_Id != value)
                {
                    NotifyPropertyChanging("Id");
                    _Id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        [Column]
        public String Nome
        {
            get { return _Nome; }
            set
            {
                NotifyPropertyChanging("Nome");
                _Nome = value;
                NotifyPropertyChanged("Nome");
            }
        }

        [Column]
        public String Valor
        {
            get { return _Valor; }
            set
            {
                NotifyPropertyChanging("Valor");
                _Valor = value;
                NotifyPropertyChanged("Valor");
            }
        }
        #endregion

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

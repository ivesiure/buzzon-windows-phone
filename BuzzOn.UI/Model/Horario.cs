using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Model.TableEntities
{
    [Table]
    public class Horario : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Private Objects
        private Int32 _Id;
        private String _Codigo;
        private String _Ponto;
        private String _Dia;
        private String _Hora;
        #endregion

        #region Properties
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get
            {
                return _Id;
            }

            set
            {
                NotifyPropertyChanging("Id");
                _Id = value;
                NotifyPropertyChanged("Id");
            }
        }

        [Column]
        public String Codigo
        {
            get
            {
                return _Codigo;
            }

            set
            {
                NotifyPropertyChanging("Codigo");
                _Codigo = value;
                NotifyPropertyChanged("Codigo");
            }
        }

        [Column]
        public string Ponto
        {
            get
            {
                return _Ponto;
            }

            set
            {
                NotifyPropertyChanging("Ponto");
                _Ponto = value;
                NotifyPropertyChanged("Ponto");
            }
        }

        [Column]
        public string Dia
        {
            get
            {
                return _Dia;
            }

            set
            {
                NotifyPropertyChanging("Dia");
                _Dia = value;
                NotifyPropertyChanged("Dia");
            }
        }

        [Column]
        public string Hora
        {
            get
            {
                return _Hora;
            }

            set
            {
                NotifyPropertyChanging("Hora");
                _Hora = value;
                NotifyPropertyChanged("Hora");
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

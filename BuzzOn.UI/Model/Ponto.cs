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
    public class Ponto : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Private Objects
        private Int32 _Id;
        private Int32 _LinhaId;
        private String _Nome;
        private String _Latitude;
        private String _Longitude;
        private String _Sentido;
        #endregion

        #region Properties
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public Int32 Id
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
        public Int32 LinhaId
        {
            get { return _LinhaId; }
            set
            {
                if (_LinhaId != value)
                {
                    NotifyPropertyChanging("LinhaId");
                    _LinhaId = value;
                    NotifyPropertyChanged("LinhaId");
                }
            }
        }

        [Column]
        public String Nome
        {
            get { return _Nome; }
            set
            {
                if (_Nome != value)
                {
                    NotifyPropertyChanging("Nome");
                    _Nome = value;
                    NotifyPropertyChanged("Nome");
                }
            }
        }

        [Column]
        public String Latitude
        {
            get { return _Latitude; }
            set
            {
                if (_Latitude != value)
                {
                    NotifyPropertyChanging("Latitude");
                    _Latitude = value;
                    NotifyPropertyChanged("Latitude");
                }
            }
        }

        [Column]
        public String Longitude
        {
            get { return _Longitude; }
            set
            {
                if (_Longitude != value)
                {
                    NotifyPropertyChanging("Longitude");
                    _Longitude = value;
                    NotifyPropertyChanged("Longitude");
                }
            }
        }

        [Column]
        public String Sentido
        {
            get { return _Sentido; }
            set
            {
                if (_Sentido != value)
                {
                    NotifyPropertyChanging("Sentido");
                    _Sentido = value;
                    NotifyPropertyChanged("Sentido");
                }
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

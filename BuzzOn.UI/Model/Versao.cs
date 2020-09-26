using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Model.TableEntities
{
    [Table]
    public partial class Versao : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Private Objects
        private int _Id;
        private String _NomeTabela;
        private String _CodigoLinha;
        private String _ValorVersao;
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
        public String NomeTabela
        {
            get { return _NomeTabela; }
            set
            {
                NotifyPropertyChanging("NomeTabela");
                _NomeTabela = value;
                NotifyPropertyChanged("NomeTabela");
            }
        }

        [Column]
        public String CodigoLinha
        {
            get { return _CodigoLinha; }
            set
            {
                NotifyPropertyChanging("CodigoLinha");
                _CodigoLinha = value;
                NotifyPropertyChanged("CodigoLinha");
            }
        }

        [Column]
        public String ValorVersao
        {
            get { return _ValorVersao; }
            set
            {
                NotifyPropertyChanging("ValorVersao");
                _ValorVersao = value;
                NotifyPropertyChanged("ValorVersao");
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

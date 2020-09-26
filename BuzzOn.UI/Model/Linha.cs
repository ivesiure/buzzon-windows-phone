using BuzzOn.UI.Business;
using BuzzOn.UI.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BuzzOn.UI.Model.TableEntities
{
    [Table]
    public partial class Linha : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Private Objects
        private int _Id;
        private String _Codigo;
        private String _Cor;
        private String _Nome;
        private String _Categoria;
        private String _SomenteCartao;
        private Boolean _Favorita;
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
        public String Codigo
        {
            get { return _Codigo; }
            set
            {
                NotifyPropertyChanging("Codigo");
                _Codigo = value;
                NotifyPropertyChanged("Codigo");
            }
        }

        [Column]
        public String Cor
        {
            get { return _Cor; }
            set
            {
                NotifyPropertyChanging("Cor");
                NotifyPropertyChanging("ObjetoCor");
                _Cor = value;
                NotifyPropertyChanged("Cor");
                NotifyPropertyChanged("ObjetoCor");
            }
        }

        [Column]
        public String Nome
        {
            get { return _Nome; }
            set
            {
                NotifyPropertyChanging("Nome");
                NotifyPropertyChanging("NomeCompleto");
                _Nome = value;
                NotifyPropertyChanged("Nome");
                NotifyPropertyChanged("NomeCompleto");
            }
        }

        [Column]
        public String Categoria
        {
            get { return _Categoria; }
            set
            {
                NotifyPropertyChanging("Categoria");
                _Categoria = value;
                NotifyPropertyChanged("Categoria");
            }
        }

        [Column]
        public String SomenteCartao
        {
            get { return _SomenteCartao; }
            set
            {
                NotifyPropertyChanging("SomenteCartao");
                _SomenteCartao = value;
                NotifyPropertyChanged("SomenteCartao");
            }
        }

        [Column]
        public Boolean Favorita
        {
            get { return _Favorita; }
            set
            {
                NotifyPropertyChanging("Favorita");
                NotifyPropertyChanging("ImagemFavorito");
                _Favorita = value;
                NotifyPropertyChanged("Favorita");
                NotifyPropertyChanged("ImagemFavorito");
            }
        }
        #endregion

        #region Complementary Properties

        public String NomeCompleto { get { return Codigo + " " + Nome; } }

        public String ImagemFavorito { get { return this.Favorita ? "/Assets/Buttons/heart_full.png" : "/Assets/Buttons/heart_empty.png"; } }

        public Color ObjetoCor { get { return ColorUtil.GetColorFromHex(this._Cor); } }

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

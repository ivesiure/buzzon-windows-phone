using BuzzOn.UI.Data;
using BuzzOn.UI.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BuzzOn.UI.Model
{
    public class LinhaModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public Int32 Id { get; set; }
        public String Codigo { get; set; }
        public String Nome { get; set; }
        public String Cor { get; set; }
        public String Categoria { get; set; }

        private Boolean _Favorita;
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

        private String _nomeCompleto;
        public String NomeCompleto
        {
            set { _nomeCompleto = value; }
            get { return _nomeCompleto; }
        }

        private String _nomeCompletoFiltro;
        public String NomeCompletoFiltro
        {
            set { _nomeCompletoFiltro = value; }
            get { return _nomeCompletoFiltro; }
        }

        public String ImagemFavorito { get { return this.Favorita ? "/Assets/Buttons/heart_full.png" : "/Assets/Buttons/heart_empty.png"; } }

        public Color ObjetoCor
        {
            get
            {
                try
                {
                    return this.Cor == "#FFFFFF" ? Colors.DarkGray : ColorUtil.GetColorFromHex(this.Cor);
                }
                catch (Exception)
                {
                    return Colors.DarkGray;
                }
            }
        }


        public LinhaModel() { }

        public LinhaModel(JToken firebaseJToken)
        {
            var token = firebaseJToken.ToList()[0];

            this.Codigo = firebaseJToken.Path;
            this.Nome = token["nome"].ToString();
            this.Cor = token["cor"].ToString();
            this.Categoria = token["categoria"].ToString();
        }

        public LinhaModel(Linha linha)
        {
            this.Id = linha.Id;
            this.Codigo = linha.Codigo;
            this.Nome = linha.Nome;
            this.Cor = linha.Cor;
            this.Categoria = linha.Categoria;
            this.Favorita = linha.Favorita;
            this.NomeCompleto = linha.Codigo + " " + linha.Nome;
            this.NomeCompletoFiltro = BuzzOn.UI.Utils.Encoding.RemoveAccents(this.NomeCompleto);
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

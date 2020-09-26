using BuzzOn.UI.Model;
using BuzzOn.UI.Model.TableEntities;
using BuzzOn.UI.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BuzzOn.UI.Business
{
    public class BuzzOnBusiness
    {
        private BuzzOnContext buzzOnContext;

        public static BuzzOnBusiness Instance
        {
            get { return new BuzzOnBusiness(); }
        }

        public BuzzOnBusiness()
        {
            this.buzzOnContext = new BuzzOnContext(AppResources.ConnectionString);
        }

        public void Salvar()
        {
            buzzOnContext.SubmitChanges();
        }

        public void CriarBase()
        {
            if (buzzOnContext.DatabaseExists() == false)
            {
                buzzOnContext.CreateDatabase();
            }
        }

        public bool ExistemDados()
        {
            return this.buzzOnContext.Linhas.Any();
        }

        public LinhaBusiness Linhas
        {
            get { return new LinhaBusiness(this.buzzOnContext); }
        }

        public HorarioBusiness Horarios
        {
            get { return new HorarioBusiness(this.buzzOnContext); }
        }

        public RotaBusiness Rotas
        {
            get { return new RotaBusiness(this.buzzOnContext); }
        }

        public PontoBusiness Pontos
        {
            get { return new PontoBusiness(this.buzzOnContext); }
        }

        public VersaoBusiness Versoes
        {
            get { return new VersaoBusiness(this.buzzOnContext); }
        }

        public ParametroBusiness Parametros
        {
            get { return new ParametroBusiness(this.buzzOnContext); }
        }
    }
}

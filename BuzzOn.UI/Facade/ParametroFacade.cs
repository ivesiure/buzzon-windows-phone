using BuzzOn.UI.Data;
using BuzzOn.UI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzOn.UI.Facade
{
    public class ParametroFacade
    {
        public static ParametroFacade Instance { get { return new ParametroFacade(); } }

        public void SalvarTokenUsuario(String Token)
        {
            var userId = BuzzOnConnection.Instance.Get<Parametro>(i => i.Nome == "UserId");
            if (userId == null)
                BuzzOnConnection.Instance.Insert<Parametro>(new Parametro { Nome = "UserId", Valor = Token });
        }

        public ParametroModel BuscarPorNome(string NomeParametro)
        {
            var param = BuzzOnConnection.Instance.Get<Parametro>(p => p.Nome == NomeParametro);
            if (param != null)
            {
                var paramModel = new ParametroModel(param);
                return paramModel;
            }
            return null;
        }
    }
}

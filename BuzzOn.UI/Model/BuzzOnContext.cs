using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using BuzzOn.UI.Model.TableEntities;
using Microsoft.Phone.Data.Linq;

namespace BuzzOn.UI.Model
{
    public class BuzzOnContext : DataContext
    {
        public BuzzOnContext(string connectionString) : base(connectionString) { }

        public Table<Linha> Linhas;
        public Table<Rota> Rotas;
        public Table<Ponto> Pontos;
        public Table<Horario> Horarios;
        public Table<Versao> Versao;
        public Table<Parametro> Parametro
        {
            get { return VerifyTable<Parametro>(); }
        }

        public Table<TEntity> VerifyTable<TEntity>() where TEntity : class
        {
            var table = GetTable<TEntity>();
            try
            {
                table.Any();
            }
            catch (System.Data.Common.DbException exception)
            {
                if (exception.Message.StartsWith("The specified table does not exist."))
                {
                    var databaseSchemaUpdater = this.CreateDatabaseSchemaUpdater();
                    databaseSchemaUpdater.AddTable<TEntity>();
                    databaseSchemaUpdater.Execute();
                }
                else
                {
                    throw;
                }
            }
            return table;
        }
    }
}

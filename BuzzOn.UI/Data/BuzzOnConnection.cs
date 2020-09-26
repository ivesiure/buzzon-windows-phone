using SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using System;
using System.Linq.Expressions;
using System.IO.IsolatedStorage;

namespace BuzzOn.UI.Data
{
    public class BuzzOnConnection
    {
        private SQLiteConnection _connection;

        public static BuzzOnConnection Instance
        {
            get { return new BuzzOnConnection(); }
        }

        public async Task<bool> OnCreate<T>()
        {
            try
            {
                using (_connection = new SQLiteConnection(App.DB_PATH))
                {
                    _connection.CreateTable<T>();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public T Get<T>(int id) where T : new()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingEntity = dbConn.Query<T>("select * from " + typeof(T).Name + " where Id =" + id).FirstOrDefault();
                return existingEntity;
            }
        }

        public T Get<T>(Func<T, bool> filter) where T : new()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var entity = dbConn.Table<T>().Where(filter).FirstOrDefault();
                return entity;
            }
        }

        public ObservableCollection<T> List<T>() where T : new()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var myCollection = dbConn.Table<T>().ToList<T>();
                var ContactsList = new ObservableCollection<T>(myCollection);
                return ContactsList;
            }
        }

        public ObservableCollection<T> List<T>(Func<T, bool> filter) where T : new()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var entities = dbConn.Table<T>().Where(filter).ToList();
                return new ObservableCollection<T>(entities);
            }
        }

        public void Update<T>(T tEntity) where T : new()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var id = tEntity.GetType().GetProperty("Id").GetValue(tEntity).ToString();
                var existingEntity = dbConn.Query<T>("select * from " + typeof(T).Name + " where Id =" + id).FirstOrDefault();
                if (existingEntity != null)
                {
                    Type type = tEntity.GetType();
                    foreach (PropertyInfo propertyInfo in type.GetProperties())
                    {
                        if (propertyInfo.CanRead)
                        {
                            object firstValue = propertyInfo.GetValue(tEntity, null);
                            object secondValue = propertyInfo.GetValue(existingEntity, null);
                            if (!object.Equals(firstValue, secondValue))
                            {
                                existingEntity.GetType().GetProperty(propertyInfo.Name).SetValue(existingEntity, firstValue);
                            }
                        }
                    }

                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Update(existingEntity);
                    });
                }
            }
        }

        public void Insert<T>(T tEntity)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.RunInTransaction(() =>
                {
                    dbConn.Insert(tEntity);
                });
            }
        }

        public void InsertAll<T>(IEnumerable<T> enumerableObjects)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.RunInTransaction(() =>
                {
                    dbConn.InsertAll(enumerableObjects);
                });
            }
        }

        public void Delete<T>(Func<T, bool> predicate) where T : new ()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingconact = dbConn.Table<T>().FirstOrDefault(predicate);
                if (existingconact != null)
                {
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Delete(existingconact);
                    });
                }
            }
        }

        public int DeleteAll<T>(Func<T, bool> predicate) where T : new()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var idList = string.Join(", ", dbConn.Table<T>().Where(predicate).Select(i => i.GetType().GetProperty("Id").GetValue(i).ToString()).ToList());
                var affectedRows = dbConn.Execute("delete from " + typeof(T).Name + " where Id in (" + idList + ")");
                return affectedRows;
            }
        }

        public void DropAndRecreate<T>()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.DropTable<T>();
                dbConn.CreateTable<T>();
                dbConn.Dispose();
                dbConn.Close();
            }
        }

        public bool Any<T>() where T : new()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var any = dbConn.Query<T>("select * from " + typeof(T).Name).Any();
                return any;
            }
        }

        public bool Any<T>(Func<T, bool> filter) where T : new()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var any = dbConn.Table<T>().Any(filter);
                return any;
            }
        }
    }
}

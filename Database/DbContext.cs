using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using WydatkiAnd.Models;
using System.Linq.Expressions;
using SQLiteNetExtensions.Extensions;

namespace WydatkiAnd.Database
{
    public class DbContext : IDisposable
    {
        static readonly object Locker = new object();

        private SQLiteConnection connection;

        public DbContext(string path)
        {
            connection = new SQLiteConnection(path);

        }

        public IEnumerable<T> GetAllItems<T>() where T : IEntity, new()
            {
            lock (Locker)
            {
                return connection.Table<T>();
            }
                
            }

        public IEnumerable<T> GetAllItemsWithChildren<T>() where T : IEntity, new()
        {
            lock (Locker)
            {
                return connection.GetAllWithChildren<T>();
            }

        }

        public T GetItem<T>(int id) where T : IEntity, new()
        {
            lock (Locker)
            {
                return connection.Get<T>(id);
            }
        }

        public T GetItemWithChildren<T>(int id) where T : IEntity, new()
        {
            lock (Locker)
            {
                return connection.GetWithChildren<T>(id);
            }
        }

        public int SaveItem(IEntity item)
        {
            lock (Locker)
            {
                if (item.Id != 0)
                {
                    connection.Update(item);
                    return item.Id;
                }
                return connection.Insert(item);
            }
        }

        public int DeleteItem(IEntity item)
        {
            lock (Locker)
            {
                return connection.Delete(item);
            }
        }

        
        public IEnumerable<T> GetItemsWhere<T>(Expression<Func<T, bool>> predicate) where T : IEntity, new()
        {
            lock (Locker)
            {
                var query = connection.Table<T>().Where(predicate);
                return query;
            }
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
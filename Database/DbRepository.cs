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
using System.IO;
using System.Linq.Expressions;

namespace WydatkiAnd.Database
{
    public class DbRepository <T> : IDisposable where T : IEntity, new()
    {
        
        protected DbContext _ctx;

        protected string FilePath
        {
            get
            {
                var docFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var dbFile = Path.Combine(docFolder, "Wydatkidb.sqlite");
                return dbFile;
            }
        }

        public DbRepository()
        {
            _ctx = new DbContext(FilePath);
        }

        public IEnumerable<T> GetAllItems()
        {
            return _ctx.GetAllItems<T>();
        }

        public IEnumerable<T> GetAllItemsWithChildren()
        {
            return _ctx.GetAllItemsWithChildren<T>();
        }

        public T GetItem(int id)
        {
            return _ctx.GetItem<T>(id);
        }

        public T GetItemWithChildren(int id)
        {
            return _ctx.GetItemWithChildren<T>(id);
        }
        
        public int SaveItem(T item)
        {
            return _ctx.SaveItem(item);
        }

        public int DeleteItem(T item)
        {
            return _ctx.DeleteItem(item);
        }

        public IEnumerable<T> GetItemsWhere(Expression<Func<T, bool>> predicate)
        {
            return _ctx.GetItemsWhere<T>(predicate);
            
        }

        public void Dispose()
        {
            _ctx.Dispose();
        }
    }
}
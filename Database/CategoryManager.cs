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
using WydatkiAnd.Models;
using System.Linq.Expressions;


namespace WydatkiAnd.Database
{
    public class CategoryManager : IDisposable
    {
        protected DbRepository<Category> db;

        public CategoryManager()
        {
            db = new DbRepository<Category>();
        }

        public Category GetItemWithChildren(int id)
        {
            return db.GetItemWithChildren(id);
        }

        public List<Category> GetAllItems()
        {
            return new List<Category>(db.GetAllItems());
        }

        public List<Category> GetEditableItems()
        {
            Expression<Func<Category, bool>> pre = (a => (a.Id !=1 && a.Id!=2));
            return new List<Category>(db.GetItemsWhere(pre));
        }

        public Category GetItemByName (string name)
        {
            var item = from a in db.GetAllItems()
                       where a.Name == name
                       select a;

            return item.FirstOrDefault();
        }

        public int SaveItem(Category cat)
        {
            return db.SaveItem(cat);
        }

        public int DeleteItem(Category cat)
        {
            return db.DeleteItem(cat);
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
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
using System.Collections;
using System.Linq.Expressions;

namespace WydatkiAnd.Database
{
    public class ExpenseManager : IDisposable
    {
        protected DbRepository<Expense> db;

        public ExpenseManager()
        {
            db = new DbRepository<Expense>();
        }



        public List<Expense> GetAllItems()
        {
            return new List<Expense>(db.GetAllItems()); 
        }

        public List<Expense> GetSomeItems(DateTime startDate, DateTime endDate, string name)
        {
            return db.GetAllItemsWithChildren().Where(a => a.Category.Name == name)
                .Where(a => (a.Date >= startDate && a.Date <= endDate)).ToList();
        }

        public List<Expense> GetItemsByDates(DateTime startDate, DateTime endDate)
        {
            Expression<Func<Expense, bool>> pre = (a => (a.Date >= startDate && a.Date <= endDate));

            return new List<Expense>(db.GetItemsWhere(pre));
        }

        public List<Expense> GetItemsByCategory (Category cat)
        {
            Expression<Func<Expense, bool>> pre = (a => a.CategoryId == cat.Id);
            return new List<Expense>(db.GetItemsWhere(pre));
        }

        public List<Expense> GetItemsByCategoryName (string name)
        {
            Expression<Func<Expense, bool>> pre = (a => a.Category.Name == name);
            return new List<Expense>(db.GetItemsWhere(pre));
        }

        public Expense GetItem(int id)
        {
            return db.GetItem(id);
        }

        public Expense GetItemWithChildren(int id)
        {
            return db.GetItemWithChildren(id);
        }

        public List<Expense> GetItemsByLinq()
        {
            var item = from a in db.GetAllItems()
                       select a;

            return item.ToList();
        }

        

        public int SaveItem(Expense item)
        {
            return db.SaveItem(item);
        }

        public int DeleteItem(Expense item)
        {
            return db.DeleteItem(item);
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
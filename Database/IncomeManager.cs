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
    public class IncomeManager : IDisposable
    {

        protected DbRepository<Income> db;

        public IncomeManager()
        {
            db = new DbRepository<Income>();
        }

        

        public List<Income> GetAllItems()
        {
            return new List<Income>(db.GetAllItems());
        }

       

        public List<Income> GetItemsByDates(DateTime startDate, DateTime endDate)
        {
            Expression<Func<Income, bool>> pre = (a => (a.Date >= startDate && a.Date <= endDate));
                                                 
            return new List<Income>(db.GetItemsWhere(pre));
        }

        public Income GetItem(int id)
        {
            return db.GetItem(id);
        }
        
        public List<Income> GetItemsByLinq()
        {
            var item = from a in db.GetAllItems()
                       select a;
            
            return item.ToList();
        }

        public int SaveItem (Income item)
        {
            return db.SaveItem(item);
        }

        public int DeleteItem (Income item)
        {
            return db.DeleteItem(item);
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
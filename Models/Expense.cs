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
using SQLiteNetExtensions.Attributes;


namespace WydatkiAnd.Models
{ 
    public class Expense : IEntity
    {

        [PrimaryKey, AutoIncrement, NotNull]
        public int Id { get; set; }
        [NotNull]
        public double Amount { get; set; }
        [NotNull]
        public DateTime Date { get; set; }

        public string Details { get; set; }
        [ForeignKey(typeof(Category))]
        public int CategoryId { get; set; }

        [ManyToOne]
        public Category Category { get; set; }
        
        public Expense(double amount, DateTime date, string details, int categoryId )
        {
            Amount = amount;
            Date = date;
            Details = details;
            CategoryId = categoryId;
        }

        public Expense()
        {

        }

    }
}
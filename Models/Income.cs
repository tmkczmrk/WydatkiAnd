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


namespace WydatkiAnd.Models
{
    public class Income : IEntity
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int Id { get; set; }
        [NotNull]
        public double Amount { get; set; }
        [NotNull]
        public DateTime Date { get; set; }

        public string Details { get; set; }

        public Income(double amount, DateTime date, string details)
        {
            Amount = amount;
            Date = date;
            Details = details;
        }

        public Income()
        {

        }

        public override string ToString()
        {
            string text = Amount.ToString() + "zł.  " + Details +"  "+Date;
            return text;
        }
    }

    
}
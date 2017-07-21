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

namespace WydatkiAnd
{
    public class ExpensesListAdapter : BaseAdapter<Expense>
    {
        List<Expense> expenses;
        Activity context;

        public ExpensesListAdapter(Activity context, List<Expense> expenses) : base()
        {
            this.expenses = expenses;
            this.context = context;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Expense this[int position]
        {
            get
            {
                return expenses[position];
            }
        }


        public override int Count
        {
            get
            {
                return expenses.Count;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.ExpenseListRow, null);

            }
            view.FindViewById<TextView>(Resource.Id.expenseRowText1).Text = expenses[position].Amount.ToString();
            view.FindViewById<TextView>(Resource.Id.expenseRowText2).Text = expenses[position].Date.ToShortDateString();
            view.FindViewById<TextView>(Resource.Id.expenseRowText3).Text = expenses[position].Category.Name;

            return view;
        }
    }
}
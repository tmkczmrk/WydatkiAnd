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
    class IncomesListAdapter : BaseAdapter<Income>
    {
        List<Income> incomes;
        Activity context;

        public IncomesListAdapter (Activity context, List<Income> incomes) : base()
        {
            this.incomes = incomes;
            this.context = context;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Income this[int position]
        {
            get
            {
                return incomes[position];
            }
        }


        public override int Count
        {
            get
            {
                return incomes.Count;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if(view == null)
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.IncomeListRow, null);
                
            }
            view.FindViewById<TextView>(Resource.Id.incomeRowText1).Text = incomes[position].Amount.ToString();
            view.FindViewById<TextView>(Resource.Id.incomeRowText2).Text = incomes[position].Date.ToShortDateString();
            view.FindViewById<TextView>(Resource.Id.incomeRowText3).Text = incomes[position].Details;

            return view;
        }
    }
}
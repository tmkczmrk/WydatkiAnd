using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using WydatkiAnd.Models;
using WydatkiAnd.Database;


namespace WydatkiAnd.Fragments
{
    public class ShowIncomesFragment : Fragment
    {
        private ListView incomeLV;
        private EditText editStart;
        private EditText editEnd;
        private List<Income> incomes = new List<Income>();
        private DateTime startDate;
        private DateTime endDate;
        

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(
            Resource.Layout.ShowIncomesView, container, false);

            var context = this.Activity;

            incomeLV = view.FindViewById<ListView>(Resource.Id.incomesList);
            editStart = view.FindViewById<EditText>(Resource.Id.editStartDate);
            editEnd = view.FindViewById<EditText>(Resource.Id.editEndDate);

            DateTime today = DateTime.Today;
            startDate = new DateTime(today.Year, today.Month, 1);
            endDate = startDate.AddMonths(1).AddDays(-1);

            editStart.Text = startDate.ToShortDateString();
            editEnd.Text = endDate.ToShortDateString();
            
            editStart.Click += (sender, e) =>
            {
                DateTime now = DateTime.Today;
                
                DatePickerDialog dialog = new DatePickerDialog(context, OnStartDateSet, now.Year, now.Month - 1, now.Day);
                dialog.DatePicker.MinDate = now.Millisecond;
                dialog.Show();
            };

            editEnd.Click += (sender, e) =>
            {
                DateTime now = DateTime.Today;
                DatePickerDialog dialog = new DatePickerDialog(context, OnEndDateSet, now.Year, now.Month - 1, now.Day);
                dialog.DatePicker.MinDate = now.Millisecond;
                dialog.Show();
            };


            using (var db = new IncomeManager())
            {
                incomes = db.GetItemsByDates(startDate, endDate);
            }
           

            incomeLV.Adapter = new IncomesListAdapter(context, incomes);
            incomeLV.ItemClick += IncomeLV_ItemClick;

            return view;
        }
        void OnStartDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            editStart.Text = e.Date.ToShortDateString();

            startDate = e.Date;
            incomes.Clear();
            using (var db = new IncomeManager())
            {
                incomes = db.GetItemsByDates(startDate, endDate);
            }
            incomeLV.Adapter = new IncomesListAdapter(this.Activity, incomes);

        }

        void OnEndDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            editEnd.Text = e.Date.ToShortDateString();

            endDate = e.Date;
            incomes.Clear();
            using (var db = new IncomeManager())
            {
                incomes = db.GetItemsByDates(startDate, endDate);
            }
            incomeLV.Adapter = new IncomesListAdapter(this.Activity, incomes);

        }

        private void IncomeLV_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int i = incomes[e.Position].Id;
            var dialog = IncomeDetailsDialog.NewInstance(i);
            dialog.DialogClosed += OnDialogClosed;
            dialog.Show(FragmentManager, "dialog");
        }

        private void OnDialogClosed(object sender, EventArgs e)
        {
            incomes.Clear();
            using (var db = new IncomeManager())
            {
                incomes = db.GetItemsByDates(startDate, endDate);
            }
            incomeLV.Adapter = new IncomesListAdapter(this.Activity, incomes);
        }
    }
}
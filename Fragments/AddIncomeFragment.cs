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
using Android.Text.Method;
using Java.Text;
using Java.Util;
using Android.Text;
using WydatkiAnd.Models;
using WydatkiAnd.Database;
using Android.Views.InputMethods;

namespace WydatkiAnd.Fragments
{
    public class AddIncomeFragment : Fragment
    {

        private static EditText dateEdit;
        private static EditText amountEdit;
        private static EditText detailsEdit;
        private static Button addBtn;
        
        double incomeAmount;
        DateTime incomeDate;
        string incomeDetails;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(
            Resource.Layout.AddIncomeView, container, false);

            var context = this.Activity;

            amountEdit = view.FindViewById<EditText>(Resource.Id.editIncomeAmount);

            dateEdit = view.FindViewById<EditText>(Resource.Id.editIncomeDate);

            detailsEdit = view.FindViewById<EditText>(Resource.Id.editIncomeDetails);

            addBtn = view.FindViewById<Button>(Resource.Id.addIncomeBtn);

            amountEdit.KeyPress += (object sender, View.KeyEventArgs e) =>
            {
                e.Handled = false;
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    double doubleValue = 0;

                    doubleValue = Java.Lang.Double.ParseDouble(amountEdit.Text.ToString().Replace(',', '.'));

                    incomeAmount = doubleValue;

                    InputMethodManager inputManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
                    inputManager.HideSoftInputFromWindow(amountEdit.WindowToken, HideSoftInputFlags.None);
                    e.Handled = true;
                }
            };

            incomeDate = DateTime.Now;
            dateEdit.Text = incomeDate.ToShortDateString();


            dateEdit.Click += (sender, e) => {
                DateTime today = DateTime.Today;
                DatePickerDialog dialog = new DatePickerDialog(context, OnDateSet, today.Year, today.Month - 1, today.Day);
                dialog.DatePicker.MinDate = today.Millisecond;
                dialog.Show();
            };

            detailsEdit.KeyPress += (object sender, View.KeyEventArgs e) =>
            {
                e.Handled = false;
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    incomeDetails = detailsEdit.Text;
                    InputMethodManager inputManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
                    inputManager.HideSoftInputFromWindow(detailsEdit.WindowToken, HideSoftInputFlags.None);
                    e.Handled = true;
                }
            };

            addBtn.Click += AddBtn_Click;

            return view;
        }


        void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            dateEdit.Text = e.Date.ToShortDateString();

            incomeDate = e.Date;

        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            if (incomeAmount != 0)
            {
                Income income = new Income(incomeAmount, incomeDate, incomeDetails);
                using (var db = new IncomeManager())
                {

                    db.SaveItem(income);
                }

                Toast.MakeText(this.Activity, string.Format
                    ("Dodałeś przychód: {0}, {1}, {2}", income.Amount, income.Date.ToShortDateString(), income.Details)
                    , ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this.Activity, string.Format("Podaj kwotę przychodu"), ToastLength.Short).Show();
            }
        }
    }

}
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;

using SQLite;
using WydatkiAnd.Models;
using System.Threading.Tasks;
using WydatkiAnd.Services;
using System.IO;
using WydatkiAnd.Database;
using SQLiteNetExtensions.Extensions;
using Android.Views;
using Android.Views.InputMethods;
using System.Linq;

namespace WydatkiAnd
{
    [Activity(Label = "WydatkiAnd", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private float monthLimit;
        private float estBills;
        

        private Button AddBtn;
        private Button ReportsBtn;
        
        private EditText editMonthLimit;
        private EditText editEstBills;
        private TextView textBalance;
        private TextView textPlusMinus;
        

        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Intent loadDbIntent = new Intent(this, typeof(LoadDbService));
            StartService(loadDbIntent);
            
            SetContentView (Resource.Layout.Main);
            
            FindId();

            AddBtn.Click += AddBtn_Click;
            ReportsBtn.Click += ReportsBtn_Click;
            
            editMonthLimit.KeyPress += EditMonthLimit_KeyPress;
            editEstBills.KeyPress += EditEstBills_KeyPress;

        }

        protected override void OnResume()
        {
            base.OnResume();
            
            monthLimit = Application.Context.GetSharedPreferences("MyNumbers", FileCreationMode.Private)
                .GetFloat("Limit", 0);

            editMonthLimit.Text = monthLimit.ToString();
           
            estBills = Application.Context.GetSharedPreferences("MyNumbers", FileCreationMode.Private)
                .GetFloat("EstBills", 0);

            editEstBills.Text = estBills.ToString();

            CountBalance();
            
        }

        private void EditEstBills_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                float floatValue;

                bool isFloat = float.TryParse
                    (editEstBills.Text.ToString().Replace('.', ','), out floatValue);

                if (isFloat)
                {
                    estBills = floatValue;

                    Application.Context.GetSharedPreferences("MyNumbers", FileCreationMode.Private).
                        Edit().PutFloat("EstBills", estBills).Commit();

                    InputMethodManager inputManager = (InputMethodManager)Application.GetSystemService(Context.InputMethodService);
                    inputManager.HideSoftInputFromWindow(editEstBills.WindowToken, HideSoftInputFlags.None);
                    e.Handled = true;
                    CountBalance();
                }
                else
                {
                    Toast.MakeText(this, string.Format("Nieprawidłowa kwota"), ToastLength.Short).Show();
                }
            }
            
        }

        private void EditMonthLimit_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                float floatValue;

                bool isFloat = float.TryParse
                    (editMonthLimit.Text.ToString().Replace('.', ','), out floatValue);

                if (isFloat)
                {

                    monthLimit = floatValue;

                    Application.Context.GetSharedPreferences("MyNumbers", FileCreationMode.Private).
                        Edit().PutFloat("Limit", monthLimit).Commit();

                    InputMethodManager inputManager = (InputMethodManager)Application.GetSystemService(Context.InputMethodService);
                    inputManager.HideSoftInputFromWindow(editMonthLimit.WindowToken, HideSoftInputFlags.None);
                    e.Handled = true;
                    CountBalance();
                }
                else
                {
                    Toast.MakeText(this, string.Format("Nieprawidłowa kwota"), ToastLength.Short).Show();
                }
            }
            
        }

        void FindId()
        {
            AddBtn = FindViewById<Button>(Resource.Id.AddBtn);
            ReportsBtn = FindViewById<Button>(Resource.Id.ReportsBtn);
            editMonthLimit = FindViewById<EditText>(Resource.Id.editMonthLimit);
            editEstBills = FindViewById<EditText>(Resource.Id.editEstBills);
            textBalance = FindViewById<TextView>(Resource.Id.textBalance);
            textPlusMinus = FindViewById<TextView>(Resource.Id.textPlusMinus);
        }
        private void ReportsBtn_Click(object sender, System.EventArgs e)
        {
            var intent = new Intent(this, typeof(ReportsActivity));
            StartActivity(intent);
        }

        private void AddBtn_Click(object sender, System.EventArgs e)
        {
            var intent = new Intent(this, typeof(AddActivity));
            StartActivity(intent);
        }

        private void CountBalance()
        {
            double bills;
            double expenses;
            float balance;

            DateTime today = DateTime.Today;
            DateTime startDate = new DateTime(today.Year, today.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            int daysOfMonth = endDate.Day;
            int dayOfMonth = today.Day;

            using (var db = new ExpenseManager())
            {
                bills = db.GetSomeItems(startDate, endDate, "Rachunki").Sum(a => a.Amount);
                expenses = db.GetItemsByDates(startDate, endDate)
                                        .Where(a => a.CategoryId != 1)
                                        .Sum(a => a.Amount);
            }



            balance = (((monthLimit - (float)bills - estBills) / daysOfMonth) * dayOfMonth) - (float)expenses;

            if (balance > 0)
            {
                textBalance.Text = balance.ToString();
                textPlusMinus.Text = " zł do przodu.";
            }
            else if (balance == 0)
            {
                textBalance.Text = "";
                textPlusMinus.Text = "na zero";
            }
            else
            {
                balance = Math.Abs(balance);
                textBalance.Text = balance.ToString();
                textPlusMinus.Text = " zł do tyłu.";
            }
        }
       
        
    }
}


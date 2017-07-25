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

namespace WydatkiAnd
{
    [Activity(Label = "WydatkiAnd", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        float monthLimit;
        float estBills;
        float balance;
        

        private Button AddBtn;
        private Button ReportsBtn;
        
        private EditText editMonthLimit;
        private EditText editEstBills;
        private TextView balanceTV;

        

        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Intent loadDbIntent = new Intent(this, typeof(LoadDbService));
            StartService(loadDbIntent);
           
            balance = 10;
            
            SetContentView (Resource.Layout.Main);
            
            FindId();

            AddBtn.Click += AddBtn_Click;
            ReportsBtn.Click += ReportsBtn_Click;

            var limit = Application.Context.GetSharedPreferences("MyNumbers", FileCreationMode.Private);
            monthLimit = limit.GetFloat("Limit", 0);

            editMonthLimit.Text = monthLimit.ToString();

            var bills = Application.Context.GetSharedPreferences("MyNumbers", FileCreationMode.Private);
            estBills = bills.GetFloat("EstBills", 0);

            editEstBills.Text = estBills.ToString();
            balanceTV.Text = balance.ToString();

            editMonthLimit.KeyPress += EditMonthLimit_KeyPress;
            editEstBills.KeyPress += EditEstBills_KeyPress;
            
        }

        private void EditEstBills_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                estBills = Java.Lang.Float.ParseFloat(editEstBills.Text.ToString().Replace(',', '.'));

                InputMethodManager inputManager = (InputMethodManager)Application.GetSystemService(Context.InputMethodService);
                inputManager.HideSoftInputFromWindow(editEstBills.WindowToken, HideSoftInputFlags.None);
                e.Handled = true;
            }
        }

        private void EditMonthLimit_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                monthLimit = Java.Lang.Float.ParseFloat(editMonthLimit.Text.ToString().Replace(',', '.'));

                InputMethodManager inputManager = (InputMethodManager)Application.GetSystemService(Context.InputMethodService);
                inputManager.HideSoftInputFromWindow(editMonthLimit.WindowToken, HideSoftInputFlags.None);
                e.Handled = true;
            }
        }

        void FindId()
        {
            AddBtn = FindViewById<Button>(Resource.Id.AddBtn);
            ReportsBtn = FindViewById<Button>(Resource.Id.ReportsBtn);
            editMonthLimit = FindViewById<EditText>(Resource.Id.editMonthLimit);
            editEstBills = FindViewById<EditText>(Resource.Id.editEstBills);
            balanceTV = FindViewById<TextView>(Resource.Id.textBalance);
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

       
       
        
    }
}


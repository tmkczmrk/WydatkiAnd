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

namespace WydatkiAnd
{
    [Activity(Label = "WydatkiAnd", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        string monthLimit;
        string estBills;
        decimal balance;
        string catname;
        

        private Button AddBtn;
        private Button ReportsBtn;
        private Button ChangeMonthLimitBtn;
        private Button ChangeEstBillsBtn;
        private TextView monthLimitTV;
        private TextView estBillsTV;
        private TextView balanceTV;

        

        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Intent loadDbIntent = new Intent(this, typeof(LoadDbService));
            StartService(loadDbIntent);
           
            balance = 10;
            
            SetContentView (Resource.Layout.Main);
            
            FindId();

            StartActions();

            var limit = Application.Context.GetSharedPreferences("MyValues", FileCreationMode.Private);
            monthLimit = limit.GetString("Limit", null);

            monthLimitTV.Text = monthLimit;

            var bills = Application.Context.GetSharedPreferences("MyValues", FileCreationMode.Private);
            estBills = bills.GetString("EstBills", null);

            estBillsTV.Text = estBills;
            balanceTV.Text = balance.ToString();


            var docFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            var dbFile = Path.Combine(docFolder, "Wydatkidb.sqlite");

           
        }

        


        void FindId()
        {
            AddBtn = FindViewById<Button>(Resource.Id.AddBtn);
            ReportsBtn = FindViewById<Button>(Resource.Id.ReportsBtn);
            ChangeMonthLimitBtn = FindViewById<Button>(Resource.Id.btnChangeMonthLimit);
            ChangeEstBillsBtn = FindViewById<Button>(Resource.Id.btnChangeEstBills);
            monthLimitTV = FindViewById<TextView>(Resource.Id.textMonthLimitShow);
            estBillsTV = FindViewById<TextView>(Resource.Id.textEstBillsShow);
            balanceTV = FindViewById<TextView>(Resource.Id.textBalance);
        }

        void StartActions()
        {

            AddBtn.Click += AddBtn_Click;

            ReportsBtn.Click += ReportsBtn_Click;

            ChangeMonthLimitBtn.Click += ChangeMonthLimitBtn_Click;

            ChangeEstBillsBtn.Click += ChangeEstBillsBtn_Click;
        }

        private void ChangeEstBillsBtn_Click(object sender, EventArgs e)
        {
            var dialog = EstBillsDialog.NewInstance();
            dialog.DialogClosed += OnEstBillsDialogClosed;
            dialog.Show(FragmentManager, "dialog");
        }

        private void ChangeMonthLimitBtn_Click(object sender, System.EventArgs e)
        {
            var dialog = LimitDialog.NewInstance();
            dialog.DialogClosed += OnLimitDialogClosed;
            dialog.Show(FragmentManager, "dialog");
            
        }

        private void OnEstBillsDialogClosed(object sender, EventArgs e)
        {
            var bills = Application.Context.GetSharedPreferences("MyValues", FileCreationMode.Private);
            estBills = bills.GetString("EstBills", null);

            estBillsTV.Text = estBills;

        }

        private void OnLimitDialogClosed(object sender, EventArgs e)
        {
            var limit = Application.Context.GetSharedPreferences("MyValues", FileCreationMode.Private);
            monthLimit = limit.GetString("Limit", null);

            monthLimitTV.Text = monthLimit;
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


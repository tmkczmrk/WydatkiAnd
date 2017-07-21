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
    public class ShowExpensesFragment : Fragment
    {
        private ListView expenseLV;
        private EditText editStart;
        private EditText editEnd;
        private Spinner spinner;

        private List<Expense> expenses = new List<Expense>();
        private DateTime startDate;
        private DateTime endDate;
        string selectedCatName;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(
            Resource.Layout.ShowExpensesView, container, false);

            expenseLV = view.FindViewById<ListView>(Resource.Id.expensesList);
            editStart = view.FindViewById<EditText>(Resource.Id.editExpStartDate);
            editEnd = view.FindViewById<EditText>(Resource.Id.editExpEndDate);
            spinner = view.FindViewById<Spinner>(Resource.Id.spinnerShowExp);

            DateTime today = DateTime.Today;
            startDate = new DateTime(today.Year, today.Month, 1);
            endDate = startDate.AddMonths(1).AddDays(-1);

            editStart.Text = startDate.ToShortDateString();
            editEnd.Text = endDate.ToShortDateString();

            editStart.Click += EditStart_Click;
            editEnd.Click += EditEnd_Click;
            
            LoadSpinnerData();
            spinner.ItemSelected += Spinner_ItemSelected;
            
            //expenseLV.Adapter = new ExpensesListAdapter(this.Activity, expenses);
            expenseLV.ItemClick += ExpenseLV_ItemClick;

            return view;
        }

        
        private void EditStart_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Today;

            DatePickerDialog dialog = new DatePickerDialog(this.Activity, OnStartDateSet, now.Year, now.Month - 1, now.Day);
            dialog.DatePicker.MinDate = now.Millisecond;
            dialog.Show();
        }

        private void EditEnd_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Today;
            DatePickerDialog dialog = new DatePickerDialog(this.Activity, OnEndDateSet, now.Year, now.Month - 1, now.Day);
            dialog.DatePicker.MinDate = now.Millisecond;
            dialog.Show();
        }

        void OnStartDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            editStart.Text = e.Date.ToShortDateString();

            startDate = e.Date;
            
            LoadData();
            expenseLV.Adapter = new ExpensesListAdapter(this.Activity, expenses);

        }

        void OnEndDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            editEnd.Text = e.Date.ToShortDateString();

            endDate = e.Date;
            
            LoadData();
            expenseLV.Adapter = new ExpensesListAdapter(this.Activity, expenses);

        }

        private void ExpenseLV_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int exp = expenses[e.Position].Id;
            var dialog = ExpenseDetailsDialog.NewInstance(exp);
            dialog.DialogClosed += OnDialogClosed;
            dialog.Show(FragmentManager, "dialog");
        }

        private void OnDialogClosed(object sender, EventArgs e)
        {
            LoadData();
            expenseLV.Adapter = new ExpensesListAdapter(this.Activity, expenses);
        }

        private void LoadSpinnerData()
        {
            List<Category> categoriesList;
            
            using (var db = new CategoryManager())
            {
                categoriesList = db.GetAllItems();
            }

            var categories = categoriesList.Select(category => category.Name).ToList();

            var categoryAdapter = new ArrayAdapter<string>(
                Activity, Android.Resource.Layout.SimpleSpinnerItem, categories);

            categoryAdapter.SetDropDownViewResource
                (Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = categoryAdapter;
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            
            var mySpinner = (Spinner)sender;
            selectedCatName = string.Format("{0}", mySpinner.GetItemAtPosition(e.Position));
            
            LoadData();
            expenseLV.Adapter = new ExpensesListAdapter(this.Activity, expenses);
            
        }

        private void LoadData()
        {
            using (var db = new ExpenseManager())
            {
                expenses = db.GetSomeItems(startDate, endDate, selectedCatName);
            }
        }
    }
}
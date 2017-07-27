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
using WydatkiAnd.Dialogs;

namespace WydatkiAnd.Fragments
{
    public class AddExpenseFragment : Fragment
    {
        private static EditText dateEdit;
        private static EditText amountEdit;
        private static EditText detailsEdit;
        private static Spinner catSpinner;
        private static Button catBtn;
        private static Button addBtn;

        double expenseAmount;
        DateTime expenseDate;
        string expenseDetails;
        int expenseCat;
        string selectedCatName;
        

        Category selectedCat;

        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(
            Resource.Layout.AddExpenseView, container, false);

            var context = this.Activity;

            amountEdit = view.FindViewById<EditText>(Resource.Id.editExpenseAmount);

            dateEdit = view.FindViewById<EditText>(Resource.Id.editExpenseDate);

            detailsEdit = view.FindViewById<EditText>(Resource.Id.editExpenseDetails);

            catSpinner = view.FindViewById<Spinner>(Resource.Id.spinnerCategory);

            catBtn = view.FindViewById<Button>(Resource.Id.btnCategories);

            addBtn = view.FindViewById<Button>(Resource.Id.addExpenseBtn);

            amountEdit.KeyPress += (object sender, View.KeyEventArgs e) =>
            {
                e.Handled = false;
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    double doubleValue;

                    bool isDouble = Double.TryParse
                    (amountEdit.Text.ToString().Replace('.', ','), out doubleValue);

                    if (isDouble)
                    {
                        expenseAmount = doubleValue;
                        InputMethodManager inputManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
                        inputManager.HideSoftInputFromWindow(amountEdit.WindowToken, HideSoftInputFlags.None);
                        e.Handled = true;
                    }
                    else
                    {
                        Toast.MakeText(this.Activity, string.Format("Nieprawidłowa kwota"), ToastLength.Short).Show();
                    }
                }
            };

            expenseDate = DateTime.Today;
            dateEdit.Text = expenseDate.ToShortDateString();


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
                    expenseDetails = detailsEdit.Text;
                    InputMethodManager inputManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
                    inputManager.HideSoftInputFromWindow(detailsEdit.WindowToken, HideSoftInputFlags.None);
                    e.Handled = true;
                }
            };

            addBtn.Click += AddBtn_Click;

            catBtn.Click += CatBtn_Click;

            LoadSpinnerData();

            catSpinner.SetSelection(1);
            selectedCatName = catSpinner.SelectedItem.ToString();
            using (var db = new CategoryManager())
            {
                selectedCat = db.GetItemByName(selectedCatName);
            }
            expenseCat = selectedCat.Id;
            catSpinner.ItemSelected += Spinner_ItemSelected;

            return view;
        }

        private void CatBtn_Click(object sender, EventArgs e)
        {
            var dialog = EditCategoriesDialog.NewInstance();
            dialog.DialogClosed += OnDialogClosed;
            dialog.Show(FragmentManager, "dialog");
        }

        private void OnDialogClosed(object sender, EventArgs e)
        {
            LoadSpinnerData();
        }

        void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            dateEdit.Text = e.Date.ToShortDateString();

            expenseDate = e.Date;

        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            if (expenseAmount!=0) { 
            Expense expense = new Expense(expenseAmount, expenseDate, expenseDetails, expenseCat);
            using (var db = new ExpenseManager())
            {

                db.SaveItem(expense);
            }

            Toast.MakeText(this.Activity, string.Format
                ("Dodałeś wydatek: {0}, {1}", expense.Amount, expense.Date.ToShortDateString()),
                ToastLength.Short).Show();

                if (expenseCat == 1 && expenseDate.Month == DateTime.Today.Month)
                {
                    float bills = Application.Context.GetSharedPreferences
                        ("MyNumbers", FileCreationMode.Private).GetFloat("EstBills", 0);

                    float estBills = bills - (float)expense.Amount;

                    if (estBills < 0)
                    {
                        estBills = 0;
                    }

                    Application.Context.GetSharedPreferences("MyNumbers", FileCreationMode.Private).
                        Edit().PutFloat("EstBills", estBills).Commit();
                }
                
            }
            else
            {
                Toast.MakeText(this.Activity, string.Format("Kwota musi być inna niż 0")
                    , ToastLength.Short).Show();
            }
        }

        private void LoadSpinnerData()
        {
            List<string> categories;
            using (var db = new CategoryManager())
            {
                categories = db.GetAllItems().Select(category => category.Name).ToList();
            }
            
            var categoryAdapter = new ArrayAdapter<string>(
                Activity, Android.Resource.Layout.SimpleSpinnerItem, categories);

            categoryAdapter.SetDropDownViewResource
                (Android.Resource.Layout.SimpleSpinnerDropDownItem);
            catSpinner.Adapter = categoryAdapter;
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = (Spinner)sender;
            selectedCatName = string.Format("{0}", spinner.GetItemAtPosition(e.Position));
            
            using (var db = new CategoryManager())
            {
                selectedCat = db.GetItemByName(selectedCatName);
            }
            expenseCat = selectedCat.Id;
        }
    }
}
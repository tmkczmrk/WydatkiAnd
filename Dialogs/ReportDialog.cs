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
using WydatkiAnd.Database;
using Android.Views.InputMethods;
using System.Text.RegularExpressions;

namespace WydatkiAnd.Dialogs
{
    public class ReportDialog : DialogFragment
    {
        private TextView monthTV;
        private TextView yearTV;
        private TextView catTV;
        private TextView expensesTV;
        private TextView realExpensesTV;
        private Spinner spinner;

        private static int selectedMonth { get; set; }
        private static int selectedYear { get; set; }

        private DateTime startDate;
        private DateTime endDate;
        private double catSum;
        private double expensesSum;
        private double incomesSum;
        private float realSum;
        private float startAmount;
        private float endAmount;
        

        public event EventHandler DialogClosed;

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            if (DialogClosed != null)
            {
                DialogClosed(this, null);
            }
        }

        public static ReportDialog NewInstance(int _selectedMonth, int _selectedYear)
        {
            selectedMonth = _selectedMonth;
            selectedYear = _selectedYear;

            var dialogFragment = new ReportDialog();
            return dialogFragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var builder = new AlertDialog.Builder(Activity);

            var inflater = Activity.LayoutInflater;

            var dialogView = inflater.Inflate(Resource.Layout.ReportDialogView, null);

            if (dialogView != null)
            {
                monthTV = dialogView.FindViewById<TextView>(Resource.Id.textMonth);
                yearTV = dialogView.FindViewById<TextView>(Resource.Id.textYear);
                catTV = dialogView.FindViewById<TextView>(Resource.Id.textRepCategory);
                expensesTV = dialogView.FindViewById<TextView>(Resource.Id.textRepExpenses);
                realExpensesTV = dialogView.FindViewById<TextView>(Resource.Id.textRealExpenses);
                spinner = dialogView.FindViewById<Spinner>(Resource.Id.spinnerRep);

                GetTVText();

                startDate = new DateTime(selectedYear, selectedMonth, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);

                LoadSpinnerData();
                spinner.ItemSelected += Spinner_ItemSelected;
                spinner.SetSelection(1);

                
                using (var db = new ExpenseManager())
                {
                    expensesSum = db.GetItemsByDates(startDate, endDate).Sum(a => a.Amount);
                }

                expensesTV.Text = expensesSum.ToString();


                SetAmounts();
                if (startAmount == -1 || endAmount == -1)
                {
                    realExpensesTV.Text = "Brak wystarczających danych";
                } else
                {
                    using (var db = new IncomeManager())
                    {
                        incomesSum = db.GetItemsByDates(startDate, endDate).Sum(a => a.Amount);
                    }

                    realSum = startAmount + (float)incomesSum - endAmount;
                    realExpensesTV.Text = realSum.ToString();
                }

                

                

                builder.SetView(dialogView);
                builder.SetNeutralButton("Wróć", HandleNeutralButtonClick);
            }

            var dialog = builder.Create();

            return dialog;
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var mySpinner = (Spinner)sender;
            string selectedCatName = string.Format("{0}", mySpinner.GetItemAtPosition(e.Position));

            using (var db = new ExpenseManager())
            {
                catSum = db.GetSomeItems(startDate, endDate, selectedCatName)
                    .Sum(a => a.Amount);
            }

            catTV.Text = catSum.ToString();

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

        private void HandleNeutralButtonClick(object sender, DialogClickEventArgs e)
        {
            var dialog = (AlertDialog)sender;
            dialog.Dismiss();
        }

        private void GetTVText()
        {
            switch (selectedMonth)
            {
                case 1:
                    monthTV.Text = "ze stycznia ";
                    break;
                case 2:
                    monthTV.Text = "z lutego ";
                    break;
                case 3:
                    monthTV.Text = "z marca ";
                    break;
                case 4:
                    monthTV.Text = "z kwietnia ";
                    break;
                case 5:
                    monthTV.Text = "z maja ";
                    break;
                case 6:
                    monthTV.Text = "z czerwca ";
                    break;
                case 7:
                    monthTV.Text = "z lipca ";
                    break;
                case 8:
                    monthTV.Text = "z sierpnia ";
                    break;
                case 9:
                    monthTV.Text = "z września ";
                    break;
                case 10:
                    monthTV.Text = "z października ";
                    break;
                case 11:
                    monthTV.Text = "z listopada ";
                    break;
                case 12:
                    monthTV.Text = "z grudnia ";
                    break;
            }

            yearTV.Text = selectedYear.ToString();
        }

        private void SetAmounts()
        {
            string amountKeyEnd;
            string amountKeyStart = selectedMonth.ToString() + selectedYear.ToString();

            if (selectedMonth == 12)
            {
                amountKeyEnd = "1" + (selectedYear + 1).ToString();
            }
            else
            {
                amountKeyEnd = (selectedMonth + 1).ToString() + selectedYear.ToString();
            }

            startAmount = Application.Context.GetSharedPreferences
                ("MyNumbers", FileCreationMode.Private).GetFloat(amountKeyStart, -1);
            endAmount = Application.Context.GetSharedPreferences
                ("MyNumbers", FileCreationMode.Private).GetFloat(amountKeyEnd, -1);
        }
    }
}
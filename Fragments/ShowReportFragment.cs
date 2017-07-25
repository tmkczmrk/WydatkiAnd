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
    public class ShowReportFragment : Fragment
    {
        private Spinner monthsSpinner;
        private Spinner yearsSpinner;
        private EditText editStartAmount;
        private EditText editEndAmount;
        private Button btnShowReport;
        
        private float startAmount;
        private float endAmount;
        int selectedMonth;
        int selectedYear;
        private string amountKeyStart;
        private string amountKeyEnd;
       
        private static DateTime today = DateTime.Today;
        
        
        List<int> years = new List<int>();


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(
            Resource.Layout.ShowReportView, container, false);

            var context = this.Activity;

            monthsSpinner = view.FindViewById<Spinner>(Resource.Id.spinnerRep1);
            yearsSpinner = view.FindViewById<Spinner>(Resource.Id.spinnerRep2);
            editStartAmount = view.FindViewById<EditText>(Resource.Id.editRepStart);
            editEndAmount = view.FindViewById<EditText>(Resource.Id.editRepEnd);
            btnShowReport = view.FindViewById<Button>(Resource.Id.btnRepShow);
            
            editStartAmount.KeyPress += EditStartAmount_KeyPress;
            editEndAmount.KeyPress += EditEndAmount_KeyPress;
            btnShowReport.Click += BtnShowReport_Click;

            
            LoadMonthsSpinnerData();
            LoadYearsSpinnerData();

            int monthNow = today.Month - 1;
            int yearNow = today.Year;
            monthsSpinner.SetSelection(monthNow);
            yearsSpinner.SetSelection(years.FindIndex(y => y == yearNow));

            monthsSpinner.ItemSelected += MonthsSpinner_ItemSelected;
            yearsSpinner.ItemSelected += YearsSpinner_ItemSelected;
            selectedMonth = today.Month;
            selectedYear = today.Year;

            SetAmounts();

            
            //startDate = new DateTime(selectedYear, selectedMonth, 1);

            return view;
        }

        private void BtnShowReport_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EditEndAmount_KeyPress(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {

                endAmount = Java.Lang.Float.ParseFloat(editEndAmount.Text.ToString().Replace(',', '.'));

                Application.Context.GetSharedPreferences("MyNumbers", FileCreationMode.Private).
                    Edit().PutFloat(amountKeyEnd, endAmount).Commit();

                InputMethodManager inputManager = (InputMethodManager)this.Activity.GetSystemService(Context.InputMethodService);
                inputManager.HideSoftInputFromWindow(editEndAmount.WindowToken, HideSoftInputFlags.None);
                e.Handled = true;
            }
        }

        private void EditStartAmount_KeyPress(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                startAmount = Java.Lang.Float.ParseFloat(editStartAmount.Text.ToString().Replace(',', '.'));

                Application.Context.GetSharedPreferences("MyNumbers", FileCreationMode.Private).
                    Edit().PutFloat(amountKeyStart, startAmount).Commit();
                
                InputMethodManager inputManager = (InputMethodManager)this.Activity.GetSystemService(Context.InputMethodService);
                inputManager.HideSoftInputFromWindow(editStartAmount.WindowToken, HideSoftInputFlags.None);
                e.Handled = true;
            }

        }

        private void LoadMonthsSpinnerData()
        {
            List<string> months = new List<string>{ "Styczeń", "Luty", "Marzec", "Kwiecień",
                                    "Maj", "Czerwiec", "Lipiec", "Sierpień", "Wrzesień",
                                    "Październik", "Listopad", "Grudzień"};

            var categoryAdapter = new ArrayAdapter<string>(
                Activity, Android.Resource.Layout.SimpleSpinnerItem, months);

            categoryAdapter.SetDropDownViewResource
                (Android.Resource.Layout.SimpleSpinnerDropDownItem);
            monthsSpinner.Adapter = categoryAdapter;
        }

        private void LoadYearsSpinnerData()
        {

            int dateMax;
            int dateMin;

            using (var db = new ExpenseManager())
            {
                dateMax = (from a in db.GetAllItems()
                            select a.Date).Max().Year;

                dateMin = (from a in db.GetAllItems()
                               select a.Date).Min().Year;
                
            }
            while (dateMax >= dateMin)
            {
                years.Add(dateMin);
                dateMin++;
            }

            var categoryAdapter = new ArrayAdapter<int>(
                Activity, Android.Resource.Layout.SimpleSpinnerItem, years);

            categoryAdapter.SetDropDownViewResource
                (Android.Resource.Layout.SimpleSpinnerDropDownItem);
            yearsSpinner.Adapter = categoryAdapter;
        }

        private void YearsSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = (Spinner)sender;
            selectedYear = (int)spinner.GetItemAtPosition(e.Position);
            SetAmounts();
        }

        private void MonthsSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = (Spinner)sender;
            selectedMonth = e.Position + 1;
            SetAmounts();
        }

        private void SetAmounts()
        {
            amountKeyStart = selectedMonth.ToString() + selectedYear.ToString();

            if (selectedMonth == 12)
            {
                amountKeyEnd = "1" + (selectedYear+1).ToString();
            } else
            {
                amountKeyEnd = (selectedMonth + 1).ToString() + selectedYear.ToString();
            }

            startAmount = Application.Context.GetSharedPreferences
                ("MyNumbers", FileCreationMode.Private).GetFloat(amountKeyStart, -1);

            if (startAmount == -1)
            {
                editStartAmount.Text = "Brak danych";
            }
            else
            {
                editStartAmount.Text = startAmount.ToString();
            }

            endAmount = Application.Context.GetSharedPreferences
                ("MyNumbers", FileCreationMode.Private).GetFloat(amountKeyEnd, -1);

            if (endAmount == -1)
            {
                editEndAmount.Text = "Brak danych";
            }
            else
            {
                editEndAmount.Text = endAmount.ToString();
            }
        }

    }
}
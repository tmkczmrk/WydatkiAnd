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
                





                builder.SetView(dialogView);
                builder.SetNeutralButton("Wróć", HandleNeutralButtonClick);
            }

            var dialog = builder.Create();

            return dialog;
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
    }
}
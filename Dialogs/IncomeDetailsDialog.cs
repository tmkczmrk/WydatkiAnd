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

namespace WydatkiAnd
{
    public class IncomeDetailsDialog : DialogFragment
    {
        private EditText editDate;
        private EditText editAmount;
        private EditText editDetails;

        private static Income income;

        private static int id { get; set; }

       

        public event EventHandler DialogClosed;

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            if (DialogClosed != null)
            {
                DialogClosed(this, null);
            }
        }

        public static IncomeDetailsDialog NewInstance( int i)
        {
            id = i;
            var dialogFragment = new IncomeDetailsDialog();
            return dialogFragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

           

            var builder = new AlertDialog.Builder(Activity);
            
            var inflater = Activity.LayoutInflater;

            var dialogView = inflater.Inflate(Resource.Layout.IncomeDetailsDialogView, null);

            if (dialogView != null)
            {
                editDate = dialogView.FindViewById<EditText>(Resource.Id.editIncomeDateDialog);
                editAmount = dialogView.FindViewById<EditText>(Resource.Id.editIncomeAmountDialog);
                editDetails = dialogView.FindViewById<EditText>(Resource.Id.editIncomeDetailsDialog);

                using (var db = new IncomeManager())
                {
                    income = db.GetItem(id);
                }

                

                
                editDate.Text = income.Date.ToShortDateString();
                editAmount.Text = income.Amount.ToString();
                editDetails.Text = income.Details;
                
                editAmount.KeyPress += (object sender, View.KeyEventArgs e) =>
                {
                    e.Handled = false;
                    if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                    {
                        double doubleValue;

                        bool isDouble = Double.TryParse
                        (editAmount.Text.ToString().Replace('.', ','), out doubleValue);

                        if (isDouble)
                        {
                            income.Amount = doubleValue;

                            InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                            inputManager.HideSoftInputFromWindow(editAmount.WindowToken, HideSoftInputFlags.None);
                            e.Handled = true;
                        }
                        else
                        {
                            Toast.MakeText(this.Activity, string.Format("Nieprawidłowa kwota"), ToastLength.Short).Show();
                        }
                    }
                };

                editDate.KeyPress += (object sender, View.KeyEventArgs e) => {
                    e.Handled = false;
                    if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                    {
                       
                        string str = editDate.Text;
                        if (DateTime.TryParse(str, out DateTime dt))
                        {
                           
                            income.Date = dt;
                            
                            InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                            inputManager.HideSoftInputFromWindow(editDate.WindowToken, HideSoftInputFlags.None);
                            e.Handled = true;
                        }
                        else
                        {
                            Toast.MakeText(this.Activity, string.Format("Nieprawidłowa data"), ToastLength.Short).Show();
                        }
                    }
                };

                editDetails.KeyPress += (object sender, View.KeyEventArgs e) =>
                {
                    e.Handled = false;
                    if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                    {
                        income.Details = editDetails.Text;
                        InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                        inputManager.HideSoftInputFromWindow(editDetails.WindowToken, HideSoftInputFlags.None);
                        e.Handled = true;
                    }

                };
                builder.SetView(dialogView);
                builder.SetPositiveButton("Zapisz zmiany", HandlePositiveButtonClick);
                builder.SetNeutralButton("Wróć", HandleNeutralButtonClick);
                builder.SetNegativeButton("Usuń", HandleNegativeButtonClick);
            }


            var dialog = builder.Create();

            return dialog;

        }

        private void HandleNegativeButtonClick(object sender, DialogClickEventArgs e)
        {
            var dialog = (AlertDialog)sender;

            using (var db = new IncomeManager())
            {
                db.DeleteItem(income);
            }

            dialog.Dismiss();
        }

        private void HandleNeutralButtonClick(object sender, DialogClickEventArgs e)
        {
            var dialog = (AlertDialog)sender;
            dialog.Dismiss();
        }

        private void HandlePositiveButtonClick(object sender, DialogClickEventArgs e)
        {
            var dialog = (AlertDialog)sender;

            using (var db = new IncomeManager())
            {

                db.SaveItem(income);
            }

            dialog.Dismiss();
        }
    }
}
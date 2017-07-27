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
    public class ExpenseDetailsDialog : DialogFragment
    {
        private EditText editDate;
        private EditText editAmount;
        private EditText editDetails;
        private TextView catText;
        private Spinner spinner;
        private Button changeCatBtn;

        private static Expense expense;
        private static int id { get; set; }
        private string selectedCatName;
        private bool billUp = false;
        private bool billDown = false;
        private double diff;

        public event EventHandler DialogClosed;

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            if (DialogClosed != null)
            {
                DialogClosed(this, null);
            }
        }

        public static ExpenseDetailsDialog NewInstance(int i)
        {
            id = i;
            var dialogFragment = new ExpenseDetailsDialog();
            return dialogFragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            var builder = new AlertDialog.Builder(Activity);

            var inflater = Activity.LayoutInflater;

            var dialogView = inflater.Inflate(Resource.Layout.ExpenseDetailsView, null);

            if (dialogView != null)
            {
                editDate = dialogView.FindViewById<EditText>(Resource.Id.editExpenseDateDialog);
                editAmount = dialogView.FindViewById<EditText>(Resource.Id.editExpenseAmountDialog);
                editDetails = dialogView.FindViewById<EditText>(Resource.Id.editExpenseDetailsDialog);
                catText = dialogView.FindViewById<TextView>(Resource.Id.textView145);
                spinner = dialogView.FindViewById<Spinner>(Resource.Id.spinnerExpenseEdit);
                changeCatBtn = dialogView.FindViewById<Button>(Resource.Id.btnExpenseEdit);

                using (var db = new ExpenseManager())
                {
                    expense = db.GetItemWithChildren(id);
                }

                editDate.Text = expense.Date.ToShortDateString();
                editAmount.Text = expense.Amount.ToString();
                editDetails.Text = expense.Details;
                catText.Text = expense.Category.Name;

                editDate.KeyPress += EditDate_KeyPress;
                editAmount.KeyPress += EditAmount_KeyPress;
                editDetails.KeyPress += EditDetails_KeyPress;
                changeCatBtn.Click += ChangeCatBtn_Click;

                LoadSpinnerData();
                spinner.ItemSelected += Spinner_ItemSelected;

                builder.SetView(dialogView);
                builder.SetPositiveButton("Zapisz zmiany", HandlePositiveButtonClick);
                builder.SetNeutralButton("Wróć", HandleNeutralButtonClick);
                builder.SetNegativeButton("Usuń", HandleNegativeButtonClick);
            }


            var dialog = builder.Create();

            return dialog;
        }




        private void ChangeCatBtn_Click(object sender, EventArgs e)
        {
            catText.Text = selectedCatName;

            using (var db = new CategoryManager())
            {
                expense.CategoryId = db.GetItemByName(selectedCatName).Id;
            }
        }

        private void EditDetails_KeyPress(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                expense.Details = editDetails.Text;
                InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                inputManager.HideSoftInputFromWindow(editDetails.WindowToken, HideSoftInputFlags.None);
                e.Handled = true;
            }
        }

        private void EditAmount_KeyPress(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                double doubleValue;

                bool isDouble = Double.TryParse
                    (editAmount.Text.ToString().Replace('.', ','), out doubleValue);

                if (isDouble)
                {
                    if (doubleValue > expense.Amount)
                    {
                        diff = doubleValue - expense.Amount;
                        billUp = true;
                        billDown = false;
                    }

                    if (doubleValue < expense.Amount)
                    {
                        diff = expense.Amount - doubleValue;
                        billDown = true;
                        billUp = false;
                    }

                    expense.Amount = doubleValue;

                    InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                    inputManager.HideSoftInputFromWindow(editAmount.WindowToken, HideSoftInputFlags.None);
                    e.Handled = true;
                }
                else
                {
                    Toast.MakeText(this.Activity, string.Format("Nieprawidłowa kwota"), ToastLength.Short).Show();
                }
            }
        }

        private void EditDate_KeyPress(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {

                string str = editDate.Text;
                if (DateTime.TryParse(str, out DateTime dt))
                {

                    expense.Date = dt;

                    InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                    inputManager.HideSoftInputFromWindow(editDate.WindowToken, HideSoftInputFlags.None);
                    e.Handled = true;
                }
                else
                {
                    Toast.MakeText(this.Activity, string.Format("Nieprawidłowa data"), ToastLength.Short).Show();
                }
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
            spinner.Adapter = categoryAdapter;
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinn = (Spinner)sender;
            selectedCatName = string.Format("{0}", spinn.GetItemAtPosition(e.Position));
            
        }

        private void HandleNegativeButtonClick(object sender, DialogClickEventArgs e)
        {
            var dialog = (AlertDialog)sender;

            using (var db = new ExpenseManager())
            {
                db.DeleteItem(expense);
            }

            if (expense.CategoryId == 1 && expense.Date.Month == DateTime.Today.Month)
            {
                float bills = Application.Context.GetSharedPreferences
                    ("MyNumbers", FileCreationMode.Private).GetFloat("EstBills", 0);

                float estBills = bills + (float)expense.Amount;

                Application.Context.GetSharedPreferences("MyNumbers", FileCreationMode.Private).
                    Edit().PutFloat("EstBills", estBills).Commit();
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

            using (var db = new ExpenseManager())
            {

                db.SaveItem(expense);
            }

            if (expense.CategoryId == 1 && expense.Date.Month == DateTime.Today.Month && billUp)
            {
                float bills = Application.Context.GetSharedPreferences
                    ("MyNumbers", FileCreationMode.Private).GetFloat("EstBills", 0);

                float estBills = bills - (float)diff;

                if (estBills < 0)
                {
                    estBills = 0;
                }

                Application.Context.GetSharedPreferences("MyNumbers", FileCreationMode.Private).
                    Edit().PutFloat("EstBills", estBills).Commit();
            }

            if (expense.CategoryId == 1 && expense.Date.Month == DateTime.Today.Month && billDown)
            {
                float bills = Application.Context.GetSharedPreferences
                    ("MyNumbers", FileCreationMode.Private).GetFloat("EstBills", 0);

                float estBills = bills + (float)diff;

                Application.Context.GetSharedPreferences("MyNumbers", FileCreationMode.Private).
                    Edit().PutFloat("EstBills", estBills).Commit();
            }

                dialog.Dismiss();
        }
    }
}
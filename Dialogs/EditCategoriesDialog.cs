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

namespace WydatkiAnd.Dialogs
{
    public class EditCategoriesDialog : DialogFragment
    {
        private EditText editAddCat;
        private EditText editEditCat;
        private Button btnAddCat;
        private Button btnDelCat;
        private Button btnEditCat;
        private Spinner spinner;
        
        private Category selectedCat;
        private string selectedCatName;

        public event EventHandler DialogClosed;

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            if (DialogClosed != null)
            {
                DialogClosed(this, null);
            }
        }

        public static EditCategoriesDialog NewInstance()
        {
            var dialogFragment = new EditCategoriesDialog();
            return dialogFragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var builder = new AlertDialog.Builder(Activity);

            var inflater = Activity.LayoutInflater;

            var dialogView = inflater.Inflate(Resource.Layout.EditCategoriesView, null);

            if (dialogView != null)
            {
                editAddCat = dialogView.FindViewById<EditText>(Resource.Id.editAddCat);
                editEditCat = dialogView.FindViewById<EditText>(Resource.Id.editEditCat);
                btnAddCat = dialogView.FindViewById<Button>(Resource.Id.btnAddCat);
                btnAddCat = dialogView.FindViewById<Button>(Resource.Id.btnAddCat);
                btnDelCat = dialogView.FindViewById<Button>(Resource.Id.btnDelCat);
                btnEditCat = dialogView.FindViewById<Button>(Resource.Id.btnEditCat);
                spinner = dialogView.FindViewById<Spinner>(Resource.Id.spinnerEditCat);

                

                LoadSpinnerData();

                //spinner.SetSelection(1);
                //selectedCatName = spinner.SelectedItem.ToString();
                //using (var db = new CategoryManager())
                //{
                //    selectedCat = db.GetItemByName(selectedCatName);
                //}
                
                spinner.ItemSelected += Spinner_ItemSelected;

                btnAddCat.Click += BtnAddCat_Click;
                btnDelCat.Click += BtnDelCat_Click;
                btnEditCat.Click += BtnEditCat_Click;

                builder.SetView(dialogView);
                
                builder.SetNegativeButton("Wróć", HandleNegativeButtonClick);
            }

            var dialog = builder.Create();

            return dialog;
        }

        private void BtnEditCat_Click(object sender, EventArgs e)
        {
            if (editEditCat.Text != "")
            {
                selectedCat.Name = editEditCat.Text;
                using (var db = new CategoryManager())
                {
                    db.SaveItem(selectedCat);
                }

                Dialog.Dismiss();
            }
            else
            {
                Toast.MakeText(this.Activity, string.Format("Nazwa nie może być pusta"), ToastLength.Short).Show();
            }
        }

        private void BtnDelCat_Click(object sender, EventArgs e)
        {

            if (selectedCat != null)
            {
                using (var db = new CategoryManager())
                {
                    db.DeleteItem(selectedCat);
                }

                using (var db = new ExpenseManager())
                {
                    var expenses = db.GetItemsByCategory(selectedCat);
                    foreach (var exp in expenses)
                    {
                        exp.CategoryId = 2;
                        db.SaveItem(exp);
                    }
                }

                Dialog.Dismiss();
            }
        }

        private void BtnAddCat_Click(object sender, EventArgs e)
        {
            if (editAddCat.Text != "")
            {
                Category category = new Category(editAddCat.Text);
                using (var db = new CategoryManager())
                {
                    db.SaveItem(category);
                }

                Dialog.Dismiss();
            }
            else
            {
                Toast.MakeText(this.Activity, string.Format("Nazwa nie może być pusta"), ToastLength.Short).Show();
            }
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinn = (Spinner)sender;
            selectedCatName = string.Format("{0}", spinn.GetItemAtPosition(e.Position));

            using (var db = new CategoryManager())
            {
                selectedCat = db.GetItemByName(selectedCatName);
            }
            
        }

        private void LoadSpinnerData()
        {
            List<string> categories;
            using (var db = new CategoryManager())
            {
                categories = db.GetEditableItems().Select(category => category.Name).ToList();
            }
            
            var categoryAdapter = new ArrayAdapter<string>(
                this.Activity, Android.Resource.Layout.SimpleSpinnerItem, categories);

            categoryAdapter.SetDropDownViewResource
                (Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = categoryAdapter;
        }

        private void HandleNegativeButtonClick(object sender, DialogClickEventArgs e)
        {
            var dialog = (AlertDialog)sender;
            dialog.Dismiss();
        }
    }
}
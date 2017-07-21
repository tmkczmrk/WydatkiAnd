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

namespace WydatkiAnd
{
    public class EstBillsDialog : DialogFragment
    {
        private EditText edit;

        public event EventHandler DialogClosed;

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            if (DialogClosed != null)
            {
                DialogClosed(this, null);
            }
        }

        public static EstBillsDialog NewInstance()
        {
            var dialogFragment = new EstBillsDialog();
            return dialogFragment;
        }



        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Begin building a new dialog.
            var builder = new AlertDialog.Builder(Activity);

            //Get the layout inflater
            var inflater = Activity.LayoutInflater;

            //Inflate the layout for this dialog
            var dialogView = inflater.Inflate(Resource.Layout.ChangeEstBillsView, null);

            if (dialogView != null)
            {
                edit = dialogView.FindViewById<EditText>(Resource.Id.editEstBills);

                builder.SetView(dialogView);
                builder.SetPositiveButton("Tak jest!", HandlePositiveButtonClick);
                builder.SetNegativeButton("Nie, dzięki", HandleNegativeButtonClick);
            }

            //Create the builder 
            var dialog = builder.Create();

            //Now return the constructed dialog to the calling activity
            return dialog;

        }

        private void HandlePositiveButtonClick(object sender, DialogClickEventArgs e)
        {
            var dialog = (AlertDialog)sender;

            string newBills = edit.Text;

            var estBills = Application.Context.GetSharedPreferences("MyValues", FileCreationMode.Private);

            var estBillsEdit = estBills.Edit();
            estBillsEdit.PutString("EstBills", newBills);
            estBillsEdit.Commit();

            dialog.Dismiss();
        }

        private void HandleNegativeButtonClick(object sender, DialogClickEventArgs e)
        {
            var dialog = (AlertDialog)sender;
            dialog.Dismiss();
        }

    }
}
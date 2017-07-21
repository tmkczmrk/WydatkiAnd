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
    public class LimitDialog : DialogFragment
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

        public static LimitDialog NewInstance()
        {
            var dialogFragment = new LimitDialog();
            return dialogFragment;
        }



        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            var builder = new AlertDialog.Builder(Activity);
            
            var inflater = Activity.LayoutInflater;

            var dialogView = inflater.Inflate(Resource.Layout.ChangeLimitView, null);

            if (dialogView != null)
            {
                edit = dialogView.FindViewById<EditText>(Resource.Id.editLimit);
                
                builder.SetView(dialogView);
                builder.SetPositiveButton("Tak jest!", HandlePositiveButtonClick);
                builder.SetNegativeButton("Nie, dzięki", HandleNegativeButtonClick);
            }

            
            var dialog = builder.Create();
            
            return dialog;

        }

        private void HandlePositiveButtonClick(object sender, DialogClickEventArgs e)
        {
            var dialog = (AlertDialog)sender;

            string newLimit = edit.Text;

            var limit = Application.Context.GetSharedPreferences("MyValues", FileCreationMode.Private);

            var limitEdit = limit.Edit();
            limitEdit.PutString("Limit", newLimit);
            limitEdit.Commit();

            dialog.Dismiss();
        }

        private void HandleNegativeButtonClick(object sender, DialogClickEventArgs e)
        {
            var dialog = (AlertDialog)sender;
            dialog.Dismiss();
        }

    }
}
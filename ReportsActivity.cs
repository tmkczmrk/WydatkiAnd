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
using WydatkiAnd.Database;
using WydatkiAnd.Models;
using SQLite;
using System.Linq.Expressions;
using WydatkiAnd.Fragments;

namespace WydatkiAnd
{
    [Activity(Label = "Raporty")]
    public class ReportsActivity : Activity
    {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            SetContentView(Resource.Layout.ReportsView);

            ActionBar.Tab tab = ActionBar.NewTab();
            tab.SetText("Wydatki");

            tab.TabSelected += delegate (object sender, ActionBar.TabEventArgs e)
            {
                e.FragmentTransaction.Replace(Resource.Id.showHost,
                    new ShowExpensesFragment());
            };

            ActionBar.AddTab(tab);

            tab = ActionBar.NewTab();
            tab.SetText("Przychody");

            tab.TabSelected += delegate (object sender, ActionBar.TabEventArgs e)
            {
                e.FragmentTransaction.Replace(Resource.Id.showHost,
                    new ShowIncomesFragment());
            };

            ActionBar.AddTab(tab);

            tab = ActionBar.NewTab();
            tab.SetText("Raport");

            tab.TabSelected += (sender, args) =>
            {
                // Do something when tab is selected
            };
            ActionBar.AddTab(tab);

            
        }

        
    }
}
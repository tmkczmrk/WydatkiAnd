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
    [Activity(Label = "Dodaj wpis")]
    public class AddActivity : Activity//, ActionBar.ITabListener
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            SetContentView(Resource.Layout.AddView);

            ActionBar.Tab tab = ActionBar.NewTab();
            tab.SetText("Wydatek");
            
            tab.TabSelected += (object sender, ActionBar.TabEventArgs e) =>
            {
                e.FragmentTransaction.Replace(Resource.Id.addHost,
                    new AddExpenseFragment());


            };
            ActionBar.AddTab(tab);

            tab = ActionBar.NewTab();
            tab.SetText("Przychód");
            
            tab.TabSelected += (object sender, ActionBar.TabEventArgs e) =>
            {
                e.FragmentTransaction.Replace(Resource.Id.addHost,
                    new AddIncomeFragment());

                
            };
            ActionBar.AddTab(tab);

        }
        
    }
}
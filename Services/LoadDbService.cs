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
using System.IO;

using WydatkiAnd.Models;
using SQLiteNetExtensions;
using SQLite;

namespace WydatkiAnd.Services
{
    [Service]
    class LoadDbService : IntentService
    {
        public LoadDbService() : base("LoadDbService") { }

        protected override void OnHandleIntent(Intent intent)
        {
           
            var docFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            var dbFile = Path.Combine(docFolder, "Wydatkidb.sqlite");

            //File.Delete(dbFile);

            if (!System.IO.File.Exists(dbFile))
            {

                using (var conn = new SQLiteConnection(dbFile))
                {
                    conn.CreateTable<Category>();
                    conn.CreateTable<Expense>();
                    conn.CreateTable<Income>();

                    var rachunki = new Category { Name = "Rachunki" };
                    var inne = new Category { Name = "Inne" };

                    conn.Insert(rachunki);
                    conn.Insert(inne);
                }

                //var s = Resources.OpenRawResource(Resource.Raw.Wydatkidb);  
                //FileStream writeStream = new FileStream(dbFile, FileMode.OpenOrCreate, FileAccess.Write);
                //ReadWriteStream(s, writeStream);
            }

            
            //void ReadWriteStream(Stream readStream, Stream writeStream)
            //{
            //    int Length = 256;
            //    Byte[] buffer = new Byte[Length];
            //    int bytesRead = readStream.Read(buffer, 0, Length);
               
            //    while (bytesRead > 0)
            //    {
            //        writeStream.Write(buffer, 0, bytesRead);
            //        bytesRead = readStream.Read(buffer, 0, Length);
            //    }
            //    readStream.Close();
            //    writeStream.Close();
            //}

        }

        
    }
}
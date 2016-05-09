using System;
using Android.Database.Sqlite;
using Android.Content;
using System.Collections.Generic;
using Android.Graphics;
using Android.Database;

namespace Euro2016Wallpapers.Services
{

	public class DataBaseServices: SQLiteOpenHelper, IDatabaseServices
	{
		private const int DatabaseVersion = 5;
		private const string DatabaseName = "WOHdb";
		private static Context _currentContext;
		private SQLiteDatabase _db;

		public DataBaseServices (Context context)
			: base (context, DatabaseName, null, DatabaseVersion)
		{
			_currentContext = _currentContext ?? context;
			_db = WritableDatabase;
		}

		public override void OnCreate (SQLiteDatabase db)
		{			
			string databasecreate = "CREATE TABLE IF NOT EXISTS " + "picture" + "(" + "url " + "text, " + "image " + "blob);";   
			db.ExecSQL (databasecreate);
		}

		public override void OnUpgrade (SQLiteDatabase db, int oldVersion, int newVersion)
		{
		}

		private SQLiteDatabase GetDatabase ()
		{
			if (_db != null && _db.IsOpen)
				return _db;
			_db = WritableDatabase;
			return _db;
		}

		public void CloseDatabase ()
		{
			if (_db == null)
				return;
			_db.Close ();
		}

		public void InsertImage (string url, byte[] img)
		{	
			var db = GetDatabase ();
			var values = new ContentValues ();
			values.Put ("url", url);
			values.Put ("image", img);
			try {
				db.Insert ("picture", null, values);	
			} catch (SQLException s) {
				var a = 0;
			}
		}

		public bool CheckExist (string url)
		{
			var db = GetDatabase ();
			string query = string.Format ("Select * from picture where url='{0}'", url);

			var cursor = db.RawQuery (query, null);
			if (cursor.Count <= 0) {					
				cursor.Close ();
				return false;
			}
			cursor.Close ();
			return true;
		}



		public Bitmap GetImage (string url)
		{
			var db = GetDatabase ();
			Bitmap imageBitmap = null;

			string query = string.Format ("Select * from picture where url='{0}'", url);
			//const string query = "Select * from picture";
			try {
				var cursor = db.RawQuery (query, null);
				if (cursor.MoveToFirst ()) {
					do {
						var img = cursor.GetBlob (1);
						imageBitmap = BitmapFactory.DecodeByteArray (img, 0, img.Length);
					} while (cursor.MoveToNext ());
				}

				cursor.Close ();
			} catch (SQLException s) {
				var abc = 0;
			}
			////db.Close();
			 
			return imageBitmap;
		}
		
	}
}


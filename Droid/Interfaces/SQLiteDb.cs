using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using Facile.Interfaces;
using Facile.Interfaces.Droid;

[assembly: Dependency(typeof(SQLiteDb))]

namespace Facile.Interfaces.Droid
{
	public class SQLiteDb : ISQLiteDb
	{
        protected readonly string _dbname;
        protected readonly string _path;

        public SQLiteDb()
        {
            _dbname = "Facile.db3";
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), _dbname);
        }

		public SQLiteAsyncConnection GetConnection()
		{
            return new SQLiteAsyncConnection(_path);
		}

        public void RemoveDB()
        {
            if (string.IsNullOrWhiteSpace(_path) || string.IsNullOrEmpty(_path)) return;
            try
            {
                File.Delete(_path);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("File Not Found : {0}", e);
            }
        }
	}
}


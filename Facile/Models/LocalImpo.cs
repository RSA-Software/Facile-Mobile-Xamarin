using System;
using SQLite;

namespace Facile.Models
{
	[Table("localimpo")]
	public class LocalImpo
	{
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }
	
		public string ftpServer { get; set; }
		public string user { get; set; }
		public string registro { get; set; }
		public int age { get; set; }
		public bool data_download { get; set; }

		public LocalImpo()
		{
			Initalize();
		}

		public void Initalize()
		{
			ftpServer = "";
			user = "";
			registro = "";
			age = 0;
			data_download = false;
		}
	}
}

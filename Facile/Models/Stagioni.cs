using System;
using SQLite;

namespace Facile.Models
{
	public class Stagioni
	{
		[PrimaryKey]
		public int sta_codice { get; set; }

		[Indexed]
		public string sta_desc { get; set; }

		public DateTime sta_last_update { get; set; }
		public string sta_user { get; set; }
		public int sta_web_id { get; set; }
	}
}

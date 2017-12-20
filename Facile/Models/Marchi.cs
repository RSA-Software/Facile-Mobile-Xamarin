using System;
using SQLite;

namespace Facile.Models
{
	public class Marchi
	{
		[PrimaryKey]
		public int mar_codice { get; set; }

		[Indexed]
		public string mar_desc { get; set; }

		public string mar_last_update { get; set; }
		public string mar_user { get; set; }
		public int mar_web_id { get; set; }
	}
}

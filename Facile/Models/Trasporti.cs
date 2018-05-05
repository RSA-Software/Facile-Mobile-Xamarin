using System;
using SQLite;

namespace Facile.Models
{
	[Table("trasport")]
	public class Trasporti
	{
		[PrimaryKey]
		public int tra_codice { get; set; }

		[Indexed]
		public string tra_desc { get; set; }
		public int tra_nofat { get; set; }
		public string tra_user { get; set; }
		public DateTime? tra_last_update { get; set; }
	}
}

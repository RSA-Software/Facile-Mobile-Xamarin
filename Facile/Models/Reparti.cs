using System;
using SQLite;

namespace Facile.Models
{
	public class Reparti
	{
		[PrimaryKey]
		public int rep_codice { get; set; }

		[Indexed]
		public string rep_desc { get; set; }

		public double rep_sconto { get; set; }
		public int rep_cassa { get; set; }
		public int rep_non_fiscale { get; set; }
		public string rep_user { get; set; }
		public DateTime rep_last_update { get; set; }
		public int rep_cassa2 { get; set; }
	}
}

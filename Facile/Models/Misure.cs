using System;
using SQLite;

namespace Facile.Models
{
	public class Misure
	{
		[PrimaryKey]
		public int mis_codice { get; set; }

		[Indexed]
		public string mis_desc { get; set; }

		public string mis_abbr { get; set; }
		public double mis_coef_mol { get; set; }
		public int mis_ripcolli { get; set; }
		public string mis_user { get; set; }
		public DateTime mis_last_update { get; set; }
	}
}

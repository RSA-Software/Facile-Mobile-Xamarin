using System;
using SQLite;

namespace Facile.Models
{
	[Table("catmerc1")]
	public class Catmerc
	{
		[PrimaryKey]
		public int mer_codice { get; set; }

		[Indexed]
		public string mer_desc { get; set; }

		public double mer_sconto { get; set; }
		public string mer_abbr { get; set; }
		public double mer_ric1 { get; set; }
		public string mer_codart { get; set; }
		public int mer_noweb { get; set; }
		public string mer_suffix { get; set; }
		public double mer_ric2 { get; set; }
		public double mer_ric3 { get; set; }
		public int mer_pos { get; set; }
		public int mer_back_from { get; set; }
		public int mer_back_to { get; set; }
		public int mer_fore { get; set; }
		public int mer_printer { get; set; }
		public int mer_turno { get; set; }
		public int mer_monopolio { get; set; }
		public DateTime mer_last_update { get; set; }
		public int mer_hide { get; set; }
		public string mer_codgds { get; set; }
		public bool mer_consumabile { get; set; }
		public string mer_user { get; set; }
		public bool mer_coperti { get; set; }
	}
}

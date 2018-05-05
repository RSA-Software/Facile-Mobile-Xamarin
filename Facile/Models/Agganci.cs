using System;
using SQLite;

namespace Facile.Models
{
	[Table("agganci1")]
	public class Agganci
	{
		[PrimaryKey, AutoIncrement]
		public int agg_id { get; set; }

		[Indexed (Name = "FornCliDst", Order = 1, Unique = true)]
		public int agg_forn { get; set; }
		[Indexed(Name = "FornCliDst", Order = 2, Unique = true)]
		public int agg_cli { get; set; }
		[Indexed(Name = "FornCliDst", Order = 3, Unique = true)]
		public int agg_dst { get; set; }

		public string agg_codice { get; set; }
		public float agg_comp_nos { get; set; }
		public float agg_comp_age { get; set; }
		public string agg_codfor { get; set; }
		public string agg_codsoc { get; set; }
		public string agg_user { get; set; }
		public DateTime? agg_last_update { get; set; }
	}
}

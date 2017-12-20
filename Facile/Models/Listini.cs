using System;
using SQLite;

namespace Facile.Models
{
	[Table("listini1")]
	public class Listini
	{
		[PrimaryKey, AutoIncrement]
		public int lis_id { get; set; }

		[Indexed(Name = "LisCodAna", Order = 2, Unique = true)]
		public int lis_codice { get; set; }

		[Indexed(Name = "LisCodAna", Order = 1, Unique = true)]
		public string lis_art { get; set; }

		public double lis_prezzo { get; set; }
		public double lis_sco1 { get; set; }
		public double lis_sco2 { get; set; }
		public double lis_sco3 { get; set; }
		public double lis_sco4 { get; set; }
		public double lis_sco5 { get; set; }
		public double lis_sco6 { get; set; }
		public double lis_sco7 { get; set; }
		public double lis_pr1 { get; set; }
		public double lis_pr2 { get; set; }
		public double lis_pr3 { get; set; }
		public double lis_pr4 { get; set; }
		public string lis_data { get; set; }
		public DateTime lis_data_var { get; set; }
		public DateTime lis_last_update { get; set; }
		public string lis_user { get; set; }
		public int lis_web_id { get; set; }
		public double lis_netto { get; set; }
		public double lis_old_prezzo { get; set; }
		public double lis_old_netto { get; set; }
		public bool lis_stbat { get; set; }
	}
}

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
		public float lis_sco1 { get; set; }
		public float lis_sco2 { get; set; }
		public float lis_sco3 { get; set; }
		public float lis_sco4 { get; set; }
		public float lis_sco5 { get; set; }
		public float lis_sco6 { get; set; }
		public float lis_sco7 { get; set; }
		public float lis_pr1 { get; set; }
		public float lis_pr2 { get; set; }
		public float lis_pr3 { get; set; }
		public float lis_pr4 { get; set; }
		public DateTime? lis_data { get; set; }
		public DateTime? lis_data_var { get; set; }
		public DateTime? lis_last_update { get; set; }
		public string lis_user { get; set; }
		public int lis_web_id { get; set; }
		public double lis_netto { get; set; }
		public double lis_old_prezzo { get; set; }
		public double lis_old_netto { get; set; }
		public bool lis_stbat { get; set; }
		public float lis_qta_from_0 { get; set; }
		public float lis_qta_from_1 { get; set; }
		public float lis_qta_from_2 { get; set; }
		public float lis_qta_from_3 { get; set; }
		public float lis_qta_from_4 { get; set; }
		public float lis_qta_from_5 { get; set; }
		public float lis_qta_to_0 { get; set; }
		public float lis_qta_to_1 { get; set; }
		public float lis_qta_to_2 { get; set; }
		public float lis_qta_to_3 { get; set; }
		public float lis_qta_to_4 { get; set; }
		public float lis_qta_to_5 { get; set; }
		public float lis_qta_sco_0 { get; set; }
		public float lis_qta_sco_1 { get; set; }
		public float lis_qta_sco_2 { get; set; }
		public float lis_qta_sco_3 { get; set; }
		public float lis_qta_sco_4 { get; set; }
		public float lis_qta_sco_5 { get; set; }
		public float lis_cpr_pr1 { get; set; }
		public float lis_cpr_pr2 { get; set; }
		public float lis_cpr_pr3 { get; set; }
		public float lis_cpr_pr4 { get; set; }
	}
}

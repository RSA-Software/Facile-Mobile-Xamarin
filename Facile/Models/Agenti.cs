using System;
using SQLite;

namespace Facile.Models
{
	[Table("agenti1")]
	public class Agenti
	{
		[PrimaryKey]
		public int age_codice { get; set; }

		public int age_zon { get; set; }
		public string age_free_old_desc { get; set; }
		public string age_free_old_via { get; set; }
		public string age_citta { get; set; }
		public string age_cap { get; set; }
		public string age_prov { get; set; }
		public string age_tel { get; set; }
		public string age_da { get; set; }
		public string age_user { get; set; }
		public DateTime? age_last_update { get; set; }
		public string age_free_space { get; set; }
		public double age_pro_0 { get; set; }
		public double age_pro_1 { get; set; }
		public double age_pro_2 { get; set; }
		public double age_pro_3 { get; set; }
		public double age_pro_4 { get; set; }
		public double age_pro_5 { get; set; }
		public double age_pro_6 { get; set; }
		public double age_pro_7 { get; set; }
		public double age_pro_8 { get; set; }
		public double age_pro_9 { get; set; }
		public double age_pro_10 { get; set; }
		public double age_pro_11 { get; set; }
		public double age_pro_12 { get; set; }
		public double age_pro_13 { get; set; }
		public double age_pro_14 { get; set; }
		public string age_piva { get; set; }
		public string age_codfis { get; set; }
		public string age_cod_gesa { get; set; }
		public string age_password { get; set; }
		public string age_free_space2 { get; set; }
		public int age_no_ftp_check { get; set; }
		public string age_targa { get; set; }

		[Indexed]
		public string age_desc { get; set; }

		public string age_indirizzo { get; set; }
		public string age_email { get; set; }
		public DateTime? age_last_exp_tablet { get; set; }
		public int age_cpr { get; set; }
	}
}

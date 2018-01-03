using System;
using SQLite;

namespace Facile.Models
{
	[Table("impostazioni")]
	public class Ditte
	{
		[PrimaryKey , AutoIncrement]
		public int impo_id { get; set; }

		public int impo_anno { get; set; }
		public int impo_iva_inc { get; set; }
		public int impo_sco_iva_esc { get; set; }
		public int impo_nocalc_sostituzioni { get; set; }
		public int impo_cod_iva_ese { get; set; }
		public double impo_spetra_imp_0 { get; set; }
		public double impo_spetra_imp_1 { get; set; }
		public double impo_spetra_imp_2 { get; set; }
		public double impo_spetra_imp_3 { get; set; }
		public double impo_spetra_imp_4 { get; set; }
		public double impo_spetra_per_0 { get; set; }
		public double impo_spetra_per_1 { get; set; }
		public double impo_spetra_per_2 { get; set; }
		public double impo_spetra_per_3 { get; set; }
		public double impo_spetra_per_4 { get; set; }
		public double impo_spetra { get; set; }
		public double impo_fat_spese { get; set; }
		public double impo_iva_spese { get; set; }
		public double impo_bolli { get; set; }
		public double impo_trat_perc_bolli { get; set; }
		public int impo_iva_age { get; set; }
		public int impo_codice { get; set; }
		public string impo_desc { get; set; }
		public bool impo_blocca_prezzi { get; set; }
		public int impo_listino { get; set; }
		public bool impo_no_scorporo { get; set; }
	}
}

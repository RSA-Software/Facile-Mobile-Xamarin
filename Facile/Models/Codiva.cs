using System;
using SQLite;

namespace Facile.Models
{
	public class Codiva
	{
		[PrimaryKey]
		public int iva_codice { get; set; }

		[Indexed]
		public string iva_desc { get; set; }

		public int iva_tip { get; set; }
		public double iva_aliq { get; set; }
		public double iva_inded { get; set; }
		public string iva_rif_norma { get; set; }
		public string iva_freespace { get; set; }
		public string iva_cod_gesa { get; set; }
		public int iva_rep_cassa { get; set; }
		public int iva_noriv { get; set; }
		public string iva_aggancio { get; set; }
		public int iva_omaggi { get; set; }
		public double iva_ritenuta { get; set; }
		public double iva_cassa { get; set; }
		public DateTime? iva_last_update { get; set; }
		public float iva_ritenuta_perc { get; set; }
		public string iva_user { get; set; }
		public int iva_escludi_iva { get; set; }
		public bool iva_enasarco { get; set; }
	}
}

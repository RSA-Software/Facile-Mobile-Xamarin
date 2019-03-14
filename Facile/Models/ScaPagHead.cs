using System;
using SQLite;

namespace Facile.Models
{
	[Table("scapaghe")]
	public class ScaPagHead
	{
		public short dsp_relaz { get; set; }

		[PrimaryKey, AutoIncrement]
		public int dsp_codice { get; set; }

		public int dsp_clifor { get; set; }

		[Indexed]
		public DateTime? dsp_data { get; set; }

		public DateTime? dsp_data_sel { get; set; }
		public int dsp_mas { get; set; }
		public int dsp_con { get; set; }
		public int dsp_sot { get; set; }
		public double dsp_totale { get; set; }
		public string dsp_des_con { get; set; }
		public int dsp_mezzo { get; set; }
		public int dsp_codass { get; set; }
		public int dsp_oldass { get; set; }
		public int dsp_pnota { get; set; }
		public int dsp_dst { get; set; }
		public double dsp_abbuoni { get; set; }
		public int dsp_age { get; set; }
		public short dsp_sez { get; set; }
		public double dsp_ass { get; set; }
		public int dsp_sez_sca { get; set; }
		public int dsp_nonconf { get; set; }
		public DateTime? dsp_timeid { get; set; }
		public int dsp_codppc { get; set; }
		public short dsp_parked { get; set; }
		public DateTime? dsp_data_ass { get; set; }
		public string dsp_user { get; set; }
		public DateTime? dsp_last_update { get; set; }
		public bool dsp_immediato { get; set; }
		public bool dsp_no_contabilizza { get; set; }

		// Viene impostata a true per i documenti inviati alla sede
		public bool dsp_inviato { get; set; }
	}
}

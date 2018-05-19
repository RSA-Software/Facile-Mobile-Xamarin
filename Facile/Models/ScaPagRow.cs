using System;
using SQLite;

namespace Facile.Models
{
	[Table("scapagro")]
	public class ScaPagRow
	{
		[PrimaryKey, AutoIncrement]
		public int dsr_id { get; set; }

		public short dsr_relaz { get; set; }

		[Indexed]
		public int dsr_codice { get; set; }

		public short dsr_rel_sca { get; set; }
		public int dsr_num_sca { get; set; }
		public DateTime? dsr_data { get; set; }
		public string dsr_old_num_doc { get; set; }
		public DateTime? dsr_d_doc { get; set; }
		public double dsr_tot_doc { get; set; }
		public double dsr_importo { get; set; }
		public double dsr_paginc { get; set; }
		public int dsr_idx { get; set; }
		public int dsr_sez { get; set; }
		public string dsr_user { get; set; }
		public DateTime? dsr_last_update { get; set; }
		public string dsr_num_doc { get; set; }
	}
}

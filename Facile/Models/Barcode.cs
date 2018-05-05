using System;
using SQLite;

namespace Facile.Models
{
	public class Barcode
	{
		[PrimaryKey]
		public string bar_barcode { get; set; }

		[Indexed]
		public string bar_codart { get; set; }

		public double bar_qta { get; set; }
		public int bar_idxtag { get; set; }
		public short bar_tab_col { get; set; }
		public int bar_cod_col { get; set; }
		public string bar_user { get; set; }
		public DateTime? bar_last_update { get; set; }
		public int bar_tipo { get; set; }
	}
}

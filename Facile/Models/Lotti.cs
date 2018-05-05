using System;
using SQLite;

namespace Facile.Models
{
	[Table("lotti1")]
	public class Lotti
	{
		[PrimaryKey, AutoIncrement]
		public int lot_id { get; set; }

		[Indexed(Name = "LotCodiceLotto", Order = 1, Unique = true)] 
		public string lot_codice { get; set; }

		[Indexed(Name = "LotCodiceLotto", Order = 2, Unique = true)] 
		public string lot_lotto { get; set; }

		public string lot_sscc_unused { get; set; }

		[Indexed]
		public DateTime lot_scadenza { get; set; }
		public DateTime lot_start { get; set; }
		public DateTime lot_stop { get; set; }
		public int lot_numero { get; set; }
		public string lot_user { get; set; }
		public DateTime lot_last_update { get; set; }
		public DateTime lot_produzione { get; set; }
	}
}

using System;
using SQLite;

namespace Facile.Models
{
	[Table("banche1")]
	public class Banche
	{
		[PrimaryKey]
		public int ban_codice { get; set; }

		public string ban_indirizzo { get; set; }
		public string ban_citta { get; set; }
		public string ban_cap { get; set; }
		public string ban_pro { get; set; }
		public string ban_tel { get; set; }
		public string ban_azi { get; set; }
		public string ban_dip { get; set; }
		public string ban_cab { get; set; }
		public string ban_abi { get; set; }
		public string ban_conto { get; set; }
		public string ban_spor { get; set; }

		[Indexed]
		public string ban_desc { get; set; }
		public string ban_cin { get; set; }
		public string ban_iban { get; set; }
		public string ban_swift { get; set; }
		public string ban_user { get; set; }
		public DateTime ban_last_update { get; set; }

	}
}

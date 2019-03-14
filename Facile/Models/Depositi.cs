using System;
using SQLite;

namespace Facile.Models
{
	public class Depositi
	{
		[PrimaryKey]
		public int dep_codice { get; set; }

		[Indexed]
		public string dep_desc { get; set; }

		public string dep_tel1 { get; set; }
		public string dep_tel2 { get; set; }
		public int dep_dst { get; set; }
		public string dep_cod_ente { get; set; }
		public bool dep_solosma { get; set; }
		public bool dep_noweb { get; set; }
		public string dep_user { get; set; }
		public DateTime dep_last_update { get; set; }
		public DateTime dep_last_send_sez1 { get; set; }
		public bool dep_noexport { get; set; }
		public DateTime dep_last_send_sez2 { get; set; }
		public DateTime dep_last_send_sez3 { get; set; }
		public DateTime dep_last_send_sez4 { get; set; }
		public string dep_registro_free { get; set; }
		public bool dep_noweb_query { get; set; }
		public int dep_forn { get; set; }
		public string dep_registro { get; set; }
	}
}

using System;
using SQLite;

namespace Facile.Models
{
	public class Scadenze
	{
		[PrimaryKey, AutoIncrement]
		public int sca_id { get; set; }

		[Indexed(Name = "ScaRelNum", Order = 1, Unique = true)] 
		public int sca_relaz { get; set; }

		[Indexed(Name = "ScaRelNum", Order = 2, Unique = true)] 
		public int sca_num { get; set; }

		public int sca_cod_mov { get; set; }

		[Indexed]
		public int sca_cli_for { get; set; }

		public string sca_old_fattura { get; set; }
		public DateTime sca_data_fattura { get; set; }
		public int sca_nota { get; set; }
		public string sca_protocollo { get; set; }
		public int sca_pag { get; set; }
		public int sca_tipo_pag { get; set; }
		public int sca_ban { get; set; }

		[Indexed] 
		public DateTime sca_data { get; set; }

		public double sca_importo { get; set; }
		public int sca_tipo_0 { get; set; }
		public int sca_tipo_1 { get; set; }
		public int sca_pagato { get; set; }
		public DateTime sca_data_giro { get; set; }
		public int sca_forn { get; set; }
		public string sca_desc { get; set; }
		public double sca_tot_fat { get; set; }
		public DateTime sca_emessa { get; set; }
		public int sca_cod_fat { get; set; }
		public int sca_cont { get; set; }
		public int sca_anno { get; set; }
		public int sca_printed { get; set; }

		[Indexed] 
		public int sca_age { get; set; }

		[Indexed] 
		public int sca_dst { get; set; }

		public double sca_interessi { get; set; }
		public int sca_sez { get; set; }
		public int sca_tipo_fat { get; set; }
		public int sca_locked { get; set; }
		public DateTime sca_data_pag { get; set; }
		public bool sca_parked { get; set; }
		public string sca_user { get; set; }
		public DateTime sca_last_update { get; set; }
		public int sca_mezzo { get; set; }
		public double sca_ass { get; set; }
		public string sca_fattura { get; set; }
		public bool sca_insoluto { get; set; }
		public DateTime sca_data_orig { get; set; }
	}
}

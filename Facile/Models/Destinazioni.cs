using System;
using SQLite;

namespace Facile.Models
{
	[Table("destina1")]
	public class Destinazioni
	{
		[PrimaryKey]
		public int dst_codice { get; set; }

		[Indexed(Name = "DstRelCliFor", Order = 1, Unique = false)] 
		public int dst_rel { get; set; }

		[Indexed(Name = "DstRelCliFor", Order = 2, Unique = false)] 
		public int dst_cli_for { get; set; }

		public string dst_rag_soc1 { get; set; }
		public string dst_rag_soc2 { get; set; }

		[Indexed]
		public string dst_desc { get; set; }

		public string dst_tel { get; set; }
		public string dst_fax { get; set; }
		public string dst_citta { get; set; }
		public string dst_cap { get; set; }
		public string dst_prov { get; set; }
		public string dst_naz { get; set; }
		public string dst_luogo_nas { get; set; }
		public string dst_prov_nas { get; set; }
		public int dst_esc_fat { get; set; }
		public string dst_cap_nas { get; set; }
		public int dst_sesso { get; set; }
		public string dst_cfpiva { get; set; }
		public int dst_ass_age { get; set; }
		public int dst_ass_pag { get; set; }
		public DateTime dstt_data_nas { get; set; }
		public int dst_listra { get; set; }
		public DateTime dst_last_update { get; set; }
		public string dst_user { get; set; }
		public string dst_indirizzo { get; set; }
		public int dst_cod_age_prov { get; set; }
		public int dst_cod_dst_prov { get; set; }
		public string dst_email_com { get; set; }
		public string dst_email_log { get; set; }
		public double dst_latitudine { get; set; }
		public double dst_longitudine { get; set; }
		public string dst_cod_ipa { get; set; }
		public int dst_web_id { get; set; }
	}
}

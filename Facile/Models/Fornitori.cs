using System;
using SQLite;

namespace Facile.Models
{
	[Table("fornito1")]
	public class Fornitori
	{
		[PrimaryKey]
		public int for_codice { get; set; }

		public string for_rag_soc1 { get; set; }
		public string for_rag_soc2 { get; set; }

		[Indexed]
		public string for_desc { get; set; }

		public string for_pers { get; set; }
		public string for_sesso { get; set; }
		public string for_citta { get; set; }
		public string for_cap { get; set; }
		public string for_prov { get; set; }
		public string for_codfis { get; set; }
		public string for_tel { get; set; }
		public string for_cel { get; set; }
		public string for_fax { get; set; }

		[Indexed]
		public int for_pag { get; set; }

		[Indexed]
		public int for_ban { get; set; }

		[Indexed]
		public int for_eco { get; set; }

		[Indexed]
		public int for_vet { get; set; }

		public double for_sco { get; set; }
		public int for_listino { get; set; }
		public string for_naz { get; set; }
		public string for_luogo_nas { get; set; }
		public string for_prov_nas { get; set; }
		public DateTime for_data_nas { get; set; }
		public string for_tipo_doc { get; set; }
		public string for_num_doc { get; set; }
		public DateTime for_data_doc { get; set; }
		public string for_user { get; set; }
		public DateTime for_last_update { get; set; }
		public int for_txt { get; set; }
		public int for_iva_inc { get; set; }
		public string for_tel2 { get; set; }
		public string for_tel3 { get; set; }
		public string for_tel4 { get; set; }
		public double for_saldo_piu { get; set; }
		public double for_saldo_men { get; set; }
		public double for_commis { get; set; }
		public string for_aggancio { get; set; }
		public int for_dest { get; set; }
		public int for_codcli { get; set; }
		public int for_nota { get; set; }
		public double for_commis2 { get; set; }
		public string for_note { get; set; }
		public string for_rifto { get; set; }
		public string for_email { get; set; }
		public int for_zon { get; set; }
		public string for_registro { get; set; }
		public int for_esporta { get; set; }
		public int for_escludi_iva { get; set; }
		public int for_agz { get; set; }
		public string for_indirizzo { get; set; }
		public string for_web { get; set; }
		public double for_latitudine { get; set; }
		public double for_longitudine { get; set; }
		public string for_piva { get; set; }
		public int for_web_id { get; set; }
		public int for_ord_tpl { get; set; }
		public int for_ord_tipo { get; set; }
		public int for_ord_fraz { get; set; }
		public string for_pec { get; set; }
		public string for_naz_doc { get; set; }
	}
}

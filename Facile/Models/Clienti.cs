using System;
using SQLite;

namespace Facile.Models
{
	[Table("clienti1")]
	public class Clienti
	{
		[PrimaryKey]
		public int cli_codice { get; set; }

		public string cli_rag_soc1 { get; set; }
		public string cli_rag_soc2 { get; set; }

		[Indexed]
		public string cli_desc { get; set; }
		public string cli_pers { get; set; }
		public string cli_sesso { get; set; }
		public string cli_bytes_free { get; set; }
		public string cli_citta { get; set; }
		public string cli_cap { get; set; }
		public string cli_prov { get; set; }
		public string cli_codfis { get; set; }
		public string cli_old_piva { get; set; }
		public string cli_tel { get; set; }
		public string cli_cel { get; set; }
		public string cli_fax { get; set; }
		public int cli_pag { get; set; }
		public int cli_ban { get; set; }
		public int cli_eco { get; set; }
		public int cli_vet { get; set; }
		public int cli_age { get; set; }
		public int cli_zon { get; set; }
		public double cli_sco { get; set; }
		public int cli_scaglione { get; set; }
		public int cli_listino { get; set; }
		public int cli_ragg { get; set; }
		public int cli_spese { get; set; }
		public int cli_bolli { get; set; }
		public int cli_dest { get; set; }
		public int cli_coef_mol { get; set; }
		public string cli_naz { get; set; }
		public double cli_lavorazione { get; set; }
		public int cli_txt { get; set; }
		public string cli_user { get; set; }
		public string cli_mmdd { get; set; }
		public int cli_agg_mantua { get; set; }
		public float cli_fat_previsto { get; set; }
		public int cli_cau { get; set; }
		public string cli_tessera { get; set; }
		public int cli_bollini_att { get; set; }
		public int cli_bollini_pre { get; set; }
		public DateTime? cli_bollini_data { get; set; }
		public DateTime? cli_data_nas { get; set; }
		public int cli_cauzioni { get; set; }
		public int cli_trasporto { get; set; }
		public int cli_escludi_iva { get; set; }
		public int cli_cod_gru { get; set; }
		public int cli_cod_att { get; set; }
		public short cli_tab_ces { get; set; }
		public int cli_cod_ces { get; set; }
		public DateTime? cli_data_acquisiz { get; set; }
		public int cli_can { get; set; }
		public DateTime? cli_last_update { get; set; }
		public int cli_agz { get; set; }
		public int cli_cod_nat { get; set; }
		public string cli_non_usato { get; set; }
		public double cli_scoperto { get; set; }
		public string cli_aggancio { get; set; }
		public int cli_inutilizzato { get; set; }
		public int cli_iva { get; set; }
		public string cli_tel2 { get; set; }
		public string cli_tel3 { get; set; }
		public string cli_tel4 { get; set; }
		public string cli_doc { get; set; }
		public double cli_saldo_piu { get; set; }
		public double cli_saldo_men { get; set; }
		public string cli_password { get; set; }
		public string cli_email { get; set; }
		public string cli_cod_gesa { get; set; }
		public string cli_note { get; set; }
		public DateTime? cli_last { get; set; }
		public short cli_listino_tra { get; set; }
		public int cli_codfor { get; set; }
		public short cli_ass_age { get; set; }
		public short cli_ass_pag { get; set; }
		public int cli_esc_fat { get; set; }
		public short cli_ditta_ind { get; set; }
		public double cli_anticipi { get; set; }
		public DateTime? cli_data_cessazione { get; set; }
		public int cli_blocco_bollini { get; set; }
		public short cli_tab_gru { get; set; }
		public short cli_tab_att { get; set; }
		public short cli_livello { get; set; }
		public short cli_tab_nat { get; set; }
		public string cli_unused_pace { get; set; }
		public string cli_indirizzo { get; set; }
		public int cli_dep { get; set; }
		public int cli_cod_age_prov { get; set; }
		public int cli_cod_cli_prov { get; set; }
		public string cli_intra { get; set; }
		public string cli_web { get; set; }
		public double cli_latitudine { get; set; }
		public double cli_longitudine { get; set; }
		public int cli_visultima { get; set; }
		public int cli_visprossima { get; set; }
		public short cli_ricevgg_0 { get; set; }
		public short cli_ricevgg_1 { get; set; }
		public short cli_ricevgg_2 { get; set; }
		public int cli_ricevorain_0 { get; set; }
		public int cli_ricevorain_1 { get; set; }
		public int cli_ricevorain_2 { get; set; }
		public int cli_ricevorafi_0 { get; set; }
		public int cli_ricevorafi_1 { get; set; }
		public int cli_ricevorafi_2 { get; set; }
		public short cli_chiusura { get; set; }
		public bool cli_bloccato { get; set; }
		public short cli_status { get; set; }
		public string cli_piva { get; set; }
		public string cli_cod_ipa { get; set; }
		public int cli_web_id { get; set; }
		public int cli_web_address_id { get; set; }
		public int cli_crediti_pa_acq { get; set; }
		public int cli_crediti_pa_uti { get; set; }
		public int cli_dep_acquisiz { get; set; }
		public string cli_email_certificata { get; set; }
		public string cli_naz_doc { get; set; }
		public bool cli_note_cred_pa_neg { get; set; }
		public float cli_pr1 { get; set; }
		public float cli_pr2 { get; set; }
		public float cli_pr3 { get; set; }
		public float cli_pr4 { get; set; }
		public bool cli_esc_cerved_payline { get; set; }
	}
}

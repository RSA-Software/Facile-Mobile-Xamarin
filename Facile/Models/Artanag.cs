using System;
using SQLite;

namespace Facile.Models
{
	public enum DatiEtichetta : int
	{
		ANA_VEN_CODICE        = 0,
		ANA_VEN_COD_PESO      = 1,
		ANA_VEN_COD_PREZZO    = 2,
		ANA_VEN_COD_PREZZO_Q1 = 3,
	}



	public class Artanag
	{
		[PrimaryKey]
		public string ana_codice { get; set; }

		[Indexed]
		public string ana_desc1 { get; set; }

		public string ana_desc2 { get; set; }

		[Indexed]
		public int ana_iva { get; set; }

		[Indexed]
		public int ana_mis { get; set; }

		[Indexed]
		public int ana_mer { get; set; }

		[Indexed]
		public int ana_for_abituale { get; set; }

		[Indexed]
		public int ana_for_alternativo { get; set; }

		[Indexed]
		public int ana_rep { get; set; }

		[Indexed]
		public int ana_sta { get; set; }

		[Indexed]
		public int ana_mar { get; set; }

		public int ana_mis_mol { get; set; }
		public string ana_cod_fornitore { get; set; }

		[Indexed]
		public string ana_gruppo { get; set; }

		[Indexed]
		public string ana_sottog { get; set; }

		public double ana_coef_mol { get; set; }
		public float ana_pezzi_conf { get; set; }
		public int ana_scaglione_agente { get; set; }
		public double ana_sconto_agente { get; set; }
		public int ana_test { get; set; }
		public int ana_giorni_invenduto { get; set; }
		public int ana_giorni_riordino { get; set; }
		public double ana_minimo_riordino { get; set; }
		public DateTime? ana_ult_acquisto { get; set; }
		public double ana_ult_pr_acquisto { get; set; }
		public DateTime? ana_pen_acquisto { get; set; }
		public double ana_pen_pr_acquisto { get; set; }
		public DateTime? ana_ult_vendita { get; set; }
		public int ana_iva_inclusa { get; set; }
		public double ana_peso { get; set; }
		public string ana_peso_mis { get; set; }
		public int ana_pos { get; set; }
		public bool ana_trasf1 { get; set; }
		public bool ana_trasf2 { get; set; }
		public bool ana_trasf3 { get; set; }
		public bool ana_trasf4 { get; set; }
		public int ana_vend_commis { get; set; }
		public int ana_web { get; set; }
		public int ana_sot { get; set; }
		public int ana_mas { get; set; }
		public int ana_con { get; set; }
		public int ana_cod_ta1 { get; set; }
		public int ana_cod_ta2 { get; set; }
		public int ana_cod_ta3 { get; set; }
		public int ana_cod_col { get; set; }
		public short ana_tab_ta1 { get; set; }
		public short ana_tab_ta2 { get; set; }
		public short ana_tab_ta3 { get; set; }
		public short ana_tab_col { get; set; }
		public double ana_pr_med_kil { get; set; }
		public double ana_commis { get; set; }
		public DateTime? ana_d_creazione { get; set; }
		public string ana_des_mag { get; set; }
		public string ana_cod_ean_ass { get; set; }
		public int ana_zuccheri { get; set; }
		public int ana_tag { get; set; }
		public short ana_tab_set { get; set; }
		public int ana_cod_set { get; set; }
		public DateTime? ana_datainv { get; set; }
		public bool ana_notrasf { get; set; }
		public bool ana_esclis { get; set; }
		public bool ana_quotazione { get; set; }
		public int ana_venapeso { get; set; }
		public int ana_complistino { get; set; }
		public short ana_plu { get; set; }
		public short ana_bancone { get; set; }
		public int ana_listino_comp { get; set; }
		public float ana_comp_pezzo { get; set; }
		public bool ana_noagglis { get; set; }
		public bool ana_fiscale { get; set; }
		public bool ana_snregfispro { get; set; }
		public bool ana_snallfispro { get; set; }
		public int ana_fktipfispro { get; set; }
		public string ana_serfispro { get; set; }
		public short ana_numfogpro { get; set; }
		public short ana_pti_erogati { get; set; }
		public short ana_pti_detratti { get; set; }
		public bool ana_stbat { get; set; }
		public float ana_comp_nos { get; set; }
		public float ana_comp_age { get; set; }
		public int ana_sma_are { get; set; }
		public int ana_sma_set { get; set; }
		public int ana_sma_rep { get; set; }
		public int ana_sma_mod { get; set; }
		public int ana_sma_sot { get; set; }
		public int ana_sma_fam { get; set; }
		public DateTime? ana_data_npo { get; set; }
		public DateTime? ana_data_in_assortfatto { get; set; }
		public int ana_pallet_strati { get; set; }
		public int ana_pallet_colli_strato { get; set; }
		public double ana_qta_mis { get; set; }
		public float ana_peso_sgo { get; set; }
		public float ana_perc_calo_peso { get; set; }
		public bool ana_alcool { get; set; }
		public bool ana_marca_privata { get; set; }
		public bool ana_bilancia { get; set; }
		public bool ana_in_assortimento { get; set; }
		public bool ana_primo_prezzo { get; set; }
		public bool ana_in_assortfatto { get; set; }
		public bool ana_primo_assortimento { get; set; }
		public string ana_des_sco { get; set; }
		public string ana_tipo_mis { get; set; }
		public string ana_stagionalita { get; set; }
		public string ana_tipo_cons { get; set; }
		public DateTime? ana_data_in_assortimento { get; set; }
		public string ana_sma_dep { get; set; }
		public string ana_ibm_check { get; set; }
		public double ana_ult_pr_acq_sma { get; set; }
		public bool ana_premio { get; set; }
		public double ana_contributo { get; set; }
		public int ana_bollini { get; set; }
		public bool ana_campagna_prec { get; set; }
		public bool ana_noaggfor { get; set; }
		public int ana_ggscad { get; set; }
		public bool ana_gest_lotti { get; set; }
		public bool ana_moved { get; set; }
		public bool ana_esc_statistiche { get; set; }
		public bool ana_hide_pos_text { get; set; }
		public int ana_colli_sca { get; set; }
		public double ana_prezzokg { get; set; }
		public bool ana_oro_usato { get; set; }
		public double ana_ult_pr_acq_non_sco { get; set; }
		public double ana_sco_acq1 { get; set; }
		public double ana_sco_acq2 { get; set; }
		public double ana_sco_acq3 { get; set; }
		public double ana_sco_acq4 { get; set; }
		public double ana_sco_acq5 { get; set; }
		public double ana_sco_acq6 { get; set; }
		public double ana_sco_acq7 { get; set; }
		public double ana_sco_acq8 { get; set; }
		public bool ana_das { get; set; }
		public string ana_backup_code { get; set; }
		public DateTime? ana_last_update { get; set; }
		public short ana_tipo_gruppo { get; set; }
		public bool ana_gest_seriali { get; set; }
		public string ana_pen { get; set; }
		public bool ana_pen_qta { get; set; }
		public string ana_user { get; set; }
		public int ana_back_from { get; set; }
		public int ana_back_to { get; set; }
		public int ana_fore { get; set; }
		public int ana_mask { get; set; }
		public bool ana_mask_bitmap { get; set; }
		public bool ana_npo { get; set; }
		public bool ana_check_out_bilancia { get; set; }
		public int ana_iva_ult_pr_acquisto { get; set; }
		public int ana_iva_pen_pr_acquisto { get; set; }
		public short ana_tipo_web { get; set; }
		public float ana_altezza { get; set; }
		public float ana_larghezza { get; set; }
		public float ana_profondita { get; set; }
		public bool ana_imported { get; set; }
		public DateTime? ana_last_import { get; set; }
		public short ana_eko_disponibilita { get; set; }
		public DateTime? ana_eko_data_arrivo { get; set; }
		public short ana_tensione { get; set; }
		public short ana_classe_energetica { get; set; }
		public short ana_consumo { get; set; }
		public string ana_grado_protezione { get; set; }
		public double ana_capacita { get; set; }
		public string ana_capacita_mis { get; set; }
		public int ana_web_id { get; set; }
		public string ana_cod_mag { get; set; }
		public float ana_accise { get; set; }
		public float ana_contrassegni { get; set; }
		public float ana_raee { get; set; }
		public float ana_conai { get; set; }
		public short ana_deleted { get; set; }
		public short ana_tab_mix { get; set; }
		public int ana_cod_mix { get; set; }
		public short ana_pti_max { get; set; }
		public bool ana_blocca_trasf_casse { get; set; }
		public double ana_vendita_cedi { get; set; }
		public bool ana_radio { get; set; }
		public bool ana_conto { get; set; }
		public bool ana_stampa_qta { get; set; }
		public bool ana_modifica_prezzo { get; set; }
		public bool ana_uscita_cassa { get; set; }
		public short ana_formato_barcode_bil { get; set; }
		public bool ana_menu { get; set; }
		public bool ana_web_prezzo_disable { get; set; }
		public bool ana_monopolio { get; set; }
		public short ana_tipo_spesa_730 { get; set; }
		public bool ana_preferiti { get; set; }
		public bool ana_in_evidenza { get; set; }
		public int ana_pos_preferiti { get; set; }
		public int ana_pos_in_evidenza { get; set; }
		public DateTime? ana_d_primo_acq { get; set; }
		public DateTime? ana_d_prima_ven { get; set; }
		public DateTime? ana_ult_ord_cli { get; set; }
		public DateTime? ana_ult_ord_for { get; set; }

		[Ignore]
		public string ana_img_path { get; set;}

		[Ignore]
		public string ana_desc { get; set; }
	}
}

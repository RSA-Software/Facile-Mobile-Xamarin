﻿using System;
using SQLite;

namespace Facile.Models
{
	public enum MovRel
	{
		REL_CLI = 0,
		REL_FOR = 1,
		REL_NIE = 2,
	}

	public enum DocTipo : short
	{
		TIPO_FAT = 0,
		TIPO_BOL = 1,
		TIPO_DDT = 2,
		TIPO_BUO = 3,
		TIPO_ACC = 4,
		TIPO_RIC = 5,
		TIPO_PRE = 6,
		TIPO_ORD = 7,
		TIPO_FAR = 8,
		TIPO_OFO = 9,
		TIPO_AUF = 10,
		TIPO_RIO = 11,
		TIPO_DRI = 12,
		TIPO_FPF = 13,
		TIPO_ANO = 14,
	}

	public enum DocTipoVen
	{
		VEN_NORMALE = 0,
		VEN_TRASFERT = 1,
		VEN_CSERVIZI = 2,
		VEN_DELIVERY = 3,
	}


	[Table("fatture2")]
	public class Fatture
	{
		[PrimaryKey, AutoIncrement]
		public int fat_id { get; set; }

		public int fat_status { get; set; }

		[Indexed(Name = "FatTipoNum", Order = 1, Unique = true)]
		public short fat_tipo { get; set; }

		[Indexed(Name = "FatTipoNum", Order = 2, Unique = true)]
		public int fat_n_doc { get; set; }

		public int fat_n_from { get; set; }
		public int fat_n_ord { get; set; }

		[Indexed]
		public DateTime fat_d_doc { get; set; }

		public DateTime? fat_d_from { get; set; }
		public DateTime? fat_d_ord { get; set; }
		public DateTime? fat_d_diversa { get; set; }

		[Indexed]
		public int fat_inte { get; set; }

		[Indexed]
		public int fat_dest { get; set; }

  		public int fat_pag { get; set; }
  		public int fat_ban { get; set; }
  		public int fat_age { get; set; }
  		public int fat_mag { get; set; }
  		public int fat_con { get; set; }
  		public int fat_credito { get; set; }
  		public int fat_bolli { get; set; }
  		public int fat_spese { get; set; }
  		public int fat_rag { get; set; }
  		public double fat_sconto { get; set; }
  		public int fat_sez { get; set; }
  		public string fat_registro_free { get; set; }
  		public int fat_listino { get; set; }
  		public int fat_coef_mol { get; set; }
  		public int fat_colli { get; set; }
  		public double fat_peso { get; set; }
  		public int fat_recalc_colli { get; set; }
  		public int fat_recalc_peso { get; set; }
  		public int fat_cura_0 { get; set; }
  		public int fat_cura_1 { get; set; }
  		public int fat_cura_2 { get; set; }
  		public int fat_imb { get; set; }
  		public int fat_tra { get; set; }
  		public int fat_vet_0 { get; set; }
  		public int fat_vet_1 { get; set; }
  		public DateTime? fat_data_vet_0 { get; set; }
  		public DateTime? fat_data_vet_1 { get; set; }
		public int fat_ora_vet_0 { get; set; }
		public int fat_ora_vet_1 { get; set; }
		public int fat_ora_tras { get; set; }
  		public DateTime? fat_data_tras { get; set; }
  		public int fat_porto { get; set; }
  		public string fat_porto_desc { get; set; }
  		public string fat_desc_varie_0 { get; set; }
  		public string fat_desc_varie_1 { get; set; }
  		public string fat_desc_varie_2 { get; set; }
  		public string fat_desc_varie_3 { get; set; }
  		public string fat_scad_eff { get; set; }
  		public string fat_annotaz { get; set; }
  		public string fat_peso_mis { get; set; }
  		public double fat_tot_merce { get; set; }
  		public double fat_tot_netto { get; set; }
  		public double fat_imballo { get; set; }
  		public double fat_varie { get; set; }
  		public double fat_inc_eff { get; set; }
  		public double fat_tot_non_docum { get; set; }
  		public int fat_cod_iva_0 { get; set; }
  		public int fat_cod_iva_1 { get; set; }
  		public int fat_cod_iva_2 { get; set; }
  		public int fat_cod_iva_3 { get; set; }
  		public double fat_ripartizione_0 { get; set; }
  		public double fat_ripartizione_1 { get; set; }
  		public double fat_ripartizione_2 { get; set; }
  		public double fat_ripartizione_3 { get; set; }
  		public double fat_imponibile_0 { get; set; }
  		public double fat_imponibile_1 { get; set; }
  		public double fat_imponibile_2 { get; set; }
  		public double fat_imponibile_3 { get; set; }
  		public double fat_importo_0 { get; set; }
  		public double fat_importo_1 { get; set; }
  		public double fat_importo_2 { get; set; }
  		public double fat_importo_3 { get; set; }
  		public double fat_aliquota_0 { get; set; }
  		public double fat_aliquota_1 { get; set; }
  		public double fat_aliquota_2 { get; set; }
  		public double fat_aliquota_3 { get; set; }
  		public double fat_importo_iva_0 { get; set; }
  		public double fat_importo_iva_1 { get; set; }
  		public double fat_importo_iva_2 { get; set; }
  		public double fat_importo_iva_3 { get; set; }
  		public string fat_desc_iva_0 { get; set; }
  		public string fat_desc_iva_1 { get; set; }
  		public string fat_desc_iva_2 { get; set; }
  		public string fat_desc_iva_3 { get; set; }
  		public int fat_tipo_iva_0 { get; set; }
  		public int fat_tipo_iva_1 { get; set; }
  		public int fat_tipo_iva_2 { get; set; }
  		public int fat_tipo_iva_3 { get; set; }
  		public double fat_imponibile { get; set; }
  		public double fat_tot_esente { get; set; }
  		public double fat_tot_esclusa { get; set; }
  		public double fat_tot_non_imp { get; set; }
  		public double fat_totale_imponibile { get; set; }
  		public double fat_tot_iva { get; set; }
  		public double fat_art15 { get; set; }
  		public double fat_bolli_eff { get; set; }
  		public double fat_tot_fattura { get; set; }
  		public double fat_abbuoni { get; set; }
  		public double fat_anticipo { get; set; }
  		public double fat_tot_pagare { get; set; }
  		public double fat_provvigione { get; set; }
  		public short fat_tabacchi { get; set; }
  		public short fat_tentata_vendita { get; set; }
  		public double fat_ritenuta_acconto { get; set; }
  		public double fat_cassa { get; set; }
		public int fat_time { get; set; }
  		public int fat_cam { get; set; }
  		public bool fat_art74 { get; set; }
  		public int fat_num_pnota { get; set; }
  		public double fat_tot_anticipi { get; set; }
  		public double fat_tot_qta { get; set; }
  		public string fat_gruppo { get; set; }
  		public short fat_posid { get; set; }
  		public short fat_tab_ope { get; set; }
  		public int fat_cod_ope { get; set; }
  		public DateTime? fat_d_consegna { get; set; }
  		public int fat_printed { get; set; }
  		public float fat_dec_iva_0 { get; set; }
  		public float fat_dec_iva_1 { get; set; }
  		public float fat_dec_iva_2 { get; set; }
  		public float fat_dec_iva_3 { get; set; }
  		public int fat_tit { get; set; }
  		public int fat_del { get; set; }
  		public double fat_omaggi { get; set; }
  		public int fat_cnc { get; set; }
  		public double fat_tot_commiss { get; set; }
  		public int fat_calc_abbuoni { get; set; }
  		public double fat_tot_costo { get; set; }
  		public int fat_righe { get; set; }
  		public DateTime? fat_sca_0 { get; set; }
  		public DateTime? fat_sca_1 { get; set; }
  		public DateTime? fat_sca_2 { get; set; }
  		public DateTime? fat_sca_3 { get; set; }
  		public DateTime? fat_sca_4 { get; set; }
  		public DateTime? fat_sca_5 { get; set; }
  		public DateTime? fat_sca_6 { get; set; }
  		public DateTime? fat_sca_7 { get; set; }
  		public DateTime? fat_sca_8 { get; set; }
  		public DateTime? fat_sca_9 { get; set; }
  		public DateTime? fat_sca_10 { get; set; }
  		public DateTime? fat_sca_11 { get; set; }
  		public int fat_iva_cli { get; set; }
  		public double fat_sca_imp_0 { get; set; }
  		public double fat_sca_imp_1 { get; set; }
  		public double fat_sca_imp_2 { get; set; }
  		public double fat_sca_imp_3 { get; set; }
  		public double fat_sca_imp_4 { get; set; }
  		public double fat_sca_imp_5 { get; set; }
  		public double fat_sca_imp_6 { get; set; }
  		public double fat_sca_imp_7 { get; set; }
  		public double fat_sca_imp_8 { get; set; }
  		public double fat_sca_imp_9 { get; set; }
  		public double fat_sca_imp_10 { get; set; }
  		public double fat_sca_imp_11 { get; set; }
  		public string fat_destinazione { get; set; }
  		public int fat_rel { get; set; }
  		public int fat_scorporo { get; set; }
  		public int fat_scontrino { get; set; }
  		public int fat_recalc_trasp { get; set; }
  		public int fat_recalc_incef { get; set; }
  		public int fat_recalc_varie { get; set; }
  		public int fat_recalc_spese { get; set; }
  		public int fat_recalc_bolli { get; set; }
  		public int fat_trasf { get; set; }
  		public string fat_prot_ord_free { get; set; }
  		public int fat_rel_des { get; set; }
  		public short fat_tipo_ven { get; set; }
  		public string fat_prot_from_free { get; set; }
  		public int fat_cns { get; set; }
  		public int fat_vuo_cod_0 { get; set; }
  		public int fat_vuo_cod_1 { get; set; }
  		public int fat_vuo_cod_2 { get; set; }
  		public int fat_vuo_cod_3 { get; set; }
  		public int fat_vuo_cod_4 { get; set; }
  		public int fat_vuo_cod_5 { get; set; }
  		public int fat_vuo_cod_6 { get; set; }
  		public int fat_vuo_cod_7 { get; set; }
  		public int fat_vuo_cod_8 { get; set; }
  		public int fat_vuo_cod_9 { get; set; }
  		public string fat_vuo_des_0 { get; set; }
  		public string fat_vuo_des_1 { get; set; }
  		public string fat_vuo_des_2 { get; set; }
  		public string fat_vuo_des_3 { get; set; }
  		public string fat_vuo_des_4 { get; set; }
  		public string fat_vuo_des_5 { get; set; }
  		public string fat_vuo_des_6 { get; set; }
  		public string fat_vuo_des_7 { get; set; }
  		public string fat_vuo_des_8 { get; set; }
  		public string fat_vuo_des_9 { get; set; }
  		public double fat_vuo_prec_0 { get; set; }
  		public double fat_vuo_prec_1 { get; set; }
  		public double fat_vuo_prec_2 { get; set; }
  		public double fat_vuo_prec_3 { get; set; }
  		public double fat_vuo_prec_4 { get; set; }
  		public double fat_vuo_prec_5 { get; set; }
  		public double fat_vuo_prec_6 { get; set; }
  		public double fat_vuo_prec_7 { get; set; }
  		public double fat_vuo_prec_8 { get; set; }
  		public double fat_vuo_prec_9 { get; set; }
  		public double fat_vuo_cons_0 { get; set; }
  		public double fat_vuo_cons_1 { get; set; }
  		public double fat_vuo_cons_2 { get; set; }
  		public double fat_vuo_cons_3 { get; set; }
  		public double fat_vuo_cons_4 { get; set; }
  		public double fat_vuo_cons_5 { get; set; }
  		public double fat_vuo_cons_6 { get; set; }
  		public double fat_vuo_cons_7 { get; set; }
  		public double fat_vuo_cons_8 { get; set; }
  		public double fat_vuo_cons_9 { get; set; }
  		public double fat_vuo_resi_0 { get; set; }
  		public double fat_vuo_resi_1 { get; set; }
  		public double fat_vuo_resi_2 { get; set; }
  		public double fat_vuo_resi_3 { get; set; }
  		public double fat_vuo_resi_4 { get; set; }
  		public double fat_vuo_resi_5 { get; set; }
  		public double fat_vuo_resi_6 { get; set; }
  		public double fat_vuo_resi_7 { get; set; }
  		public double fat_vuo_resi_8 { get; set; }
  		public double fat_vuo_resi_9 { get; set; }
  		public double fat_vuo_diff_0 { get; set; }
  		public double fat_vuo_diff_1 { get; set; }
  		public double fat_vuo_diff_2 { get; set; }
  		public double fat_vuo_diff_3 { get; set; }
  		public double fat_vuo_diff_4 { get; set; }
  		public double fat_vuo_diff_5 { get; set; }
  		public double fat_vuo_diff_6 { get; set; }
  		public double fat_vuo_diff_7 { get; set; }
  		public double fat_vuo_diff_8 { get; set; }
  		public double fat_vuo_diff_9 { get; set; }
  		public double fat_vuo_pre_cau_0 { get; set; }
  		public double fat_vuo_pre_cau_1 { get; set; }
  		public double fat_vuo_pre_cau_2 { get; set; }
  		public double fat_vuo_pre_cau_3 { get; set; }
  		public double fat_vuo_pre_cau_4 { get; set; }
  		public double fat_vuo_pre_cau_5 { get; set; }
  		public double fat_vuo_per_cau_6 { get; set; }
  		public double fat_vuo_pre_cau_7 { get; set; }
  		public double fat_vuo_pre_cau_8 { get; set; }
  		public double fat_vuo_pre_cau_9 { get; set; }
  		public double fat_vuo_tot_cau_0 { get; set; }
  		public double fat_vuo_tot_cau_1 { get; set; }
  		public double fat_vuo_tot_cau_2 { get; set; }
  		public double fat_vuo_tot_cau_3 { get; set; }
  		public double fat_vuo_tot_cau_4 { get; set; }
  		public double fat_vuo_tot_cau_5 { get; set; }
  		public double fat_vuo_tot_cau_6 { get; set; }
  		public double fat_vuo_tot_cau_7 { get; set; }
  		public double fat_vuo_tot_cau_8 { get; set; }
  		public double fat_vuo_tot_cau_9 { get; set; }
  		public double fat_vuo_cauzione { get; set; }
  		public int fat_cauzioni { get; set; }
  		public double fat_vuo_cauzione_resa { get; set; }
  		public int fat_sta { get; set; }
  		public short fat_tab_set { get; set; }
  		public int fat_cod_set { get; set; }
  		public short fat_tab_prd { get; set; }
  		public int fat_cod_prd { get; set; }
  		public int fat_cod_com { get; set; }
  		public int fat_cod_car { get; set; }
  		public int fat_cod_pro { get; set; }
  		public string fat_space_unused { get; set; }
  		public double fat_aliquota_cas_0 { get; set; }
  		public double fat_aliquota_cas_1 { get; set; }
  		public double fat_aliquota_cas_2 { get; set; }
  		public double fat_aliquota_cas_3 { get; set; }
  		public double fat_importo_cas_0 { get; set; }
  		public double fat_importo_cas_1 { get; set; }
  		public double fat_importo_cas_2 { get; set; }
  		public double fat_importo_cas_3 { get; set; }
  		public double fat_aliquota_rit_0 { get; set; }
  		public double fat_aliquota_rit_1 { get; set; }
  		public double fat_aliquota_rit_2 { get; set; }
  		public double fat_aliquota_rit_3 { get; set; }
  		public double fat_importo_rit_0 { get; set; }
  		public double fat_importo_rit_1 { get; set; }
  		public double fat_importo_rit_2 { get; set; }
  		public double fat_importo_rit_3 { get; set; }
  		public DateTime? fat_last_update { get; set; }
  		public int fat_pax { get; set; }
  		public int fat_gru { get; set; }
  		public int fat_arr { get; set; }
  		public int fat_agz { get; set; }
  		public double fat_agz_perc { get; set; }
  		public int fat_imp { get; set; }
  		public string fat_convenz { get; set; }
  		public string fat_pratica { get; set; }
  		public bool fat_com { get; set; }
  		public bool fat_com_ricevuta { get; set; }
  		public double fat_com_imponibile { get; set; }
  		public double fat_com_iva { get; set; }
  		public double fat_com_importo { get; set; }
  		public bool fat_rie { get; set; }
  		public int fat_prn { get; set; }
  		public int fat_osp { get; set; }
  		public bool fat_sco_iva_esc { get; set; }
  		public bool fat_ric_fat { get; set; }
  		public int fat_cod_lca { get; set; }
  		public float fat_perc_rit_0 { get; set; }
  		public float fat_perc_rit_1 { get; set; }
  		public float fat_perc_rit_2 { get; set; }
  		public float fat_perc_rit_3 { get; set; }
  		public string fat_user { get; set; }
  		public float fat_tot_sconto { get; set; }
  		public short fat_luogo_compilaz { get; set; }
  		public string fat_doc_rif { get; set; }
  		public string fat_new_desc_varie { get; set; }
  		public int fat_num_doc_prov { get; set; }
  		public string fat_gruppo_prov { get; set; }
  		public DateTime? fat_data_doc_prov { get; set; }
  		public bool fat_doc_mese_prec { get; set; }
  		public bool fat_ord_for_gen { get; set; }
  		public DateTime? fat_invio_hub_pa { get; set; }
  		public bool fat_split_payment { get; set; }
  		public short fat_status_pa { get; set; }
  		public DateTime? fat_status_dt { get; set; }
  		public short fat_tipo_ordine_tabacchi { get; set; }
  		public double fat_tot_accise { get; set; }
  		public double fat_tot_contrassegni { get; set; }
  		public double fat_tot_raee { get; set; }
  		public double fat_tot_conai { get; set; }
  		public bool fat_in_transito { get; set; }
  		public bool fat_ord_in_arrivo { get; set; }
  		public bool fat_ungen_scad { get; set; }
  		public bool fat_blocca_ricezione { get; set; }
  		public int fat_dep_origine { get; set; }
  		public bool fat_check_listino { get; set; }
  		public int fat_cod_tes { get; set; }
  		public bool fat_escludi_730 { get; set; }
  		public short fat_invio_730 { get; set; }
  		public double fat_tot_tara { get; set; }
  		public int fat_cms { get; set; }
  		public int fat_cco { get; set; }
  		public double fat_tot_prov_age { get; set; }
  		public double fat_tot_prov_cpr { get; set; }
  		public double fat_tot_qevasa { get; set; }
		public short fat_stato_anomalia { get; set; }

		public string fat_prot_ord { get; set; }
		public string fat_prot_from { get; set; }
		public int fat_num_fe { get; set; }
		public double fat_aliquota_enasarco_0 { get; set; }
		public double fat_aliquota_enasarco_1 { get; set; }
		public double fat_aliquota_enasarco_2 { get; set; }
		public double fat_aliquota_enasarco_3 { get; set; }
		public double fat_importo_enasarco_0 { get; set; }
		public double fat_importo_enasarco_1 { get; set; }
		public double fat_importo_enasarco_2 { get; set; }
		public double fat_importo_enasarco_3 { get; set; }
		public double fat_totale_enasarco { get; set; }
		public bool fat_anno_enasarco_pre { get; set; }
		public string fat_registro { get; set; }
		public double fat_bolli_corpo { get; set; }
		public string fat_unused { get; set; }


		// Viene impostata a true per i nuovi documenti che sono editabili fino all'invio
		public bool fat_editable { get; set; }

		// Viene impostata a true per i documenti inseriti sul dispositivo
		public bool fat_local_doc { get; set; }

		[Ignore]
		public double des_imponibile_ivato_0 { get; set; }
		[Ignore]
		public double des_imponibile_ivato_1 { get; set; }
		[Ignore]
		public double des_imponibile_ivato_2 { get; set; }
		[Ignore]
		public double des_imponibile_ivato_3 { get; set; }
	}
}

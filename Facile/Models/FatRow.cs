using System;
using SQLite;

namespace Facile.Models
{
	[Table("fatrow2")]
	public class FatRow
	{
		[PrimaryKey, AutoIncrement]
		public int rig_id { get; set; }

		[Indexed(Name = "RigTipoNum", Order = 1, Unique = true)]
		public int rig_tipo { get; set; }

		[Indexed(Name = "RigTipoNum", Order = 2, Unique = true)]
		public int rig_n_doc { get; set; }

		public int rig_dep { get; set; }
		public int rig_iva { get; set; }
		public int rig_mis { get; set; }
		public int rig_colli { get; set; }
		public int rig_iva_inclusa { get; set; }
		public double rig_coef_mol { get; set; }
		public double rig_qta { get; set; }
		public double rig_prezzo { get; set; }
		public double rig_sconto1 { get; set; }
		public double rig_sconto2 { get; set; }
		public double rig_sconto3 { get; set; }
		public double rig_peso { get; set; }
		public double rig_tot_sconto { get; set; }
		public double rig_importo { get; set; }
		public double rig_tot_peso { get; set; }
		public double rig_ultpracq { get; set; }
		public int rig_listino { get; set; }
		public int rig_righe { get; set; }
		public int rig_usecoef { get; set; }
		public int rig_unmove { get; set; }
		public string rig_peso_mis { get; set; }

		[Indexed]
		public string rig_art { get; set; }

		public int rig_iva_cli { get; set; }

		[Indexed(Name = "RigTipoNum", Order = 3, Unique = true)]
		public DateTime rig_d_ins { get; set; }

		[Indexed(Name = "RigTipoNum", Order = 4, Unique = true)]
		public int rig_t_ins { get; set; }

		public double rig_qevasa { get; set; }
		public double rig_prezfor { get; set; }
		public string rig_artfor { get; set; }
		public int rig_num_car { get; set; }
		public int rig_sost { get; set; }
		public int rig_idxtag { get; set; }
		public int rig_cod_col { get; set; }
		public int rig_tab_col { get; set; }
		public double rig_provvig { get; set; }
		public double rig_commiss { get; set; }
		public double rig_sconto4 { get; set; }
		public double rig_scomerce { get; set; }
		public double rig_scovalore { get; set; }
		public double rig_spese { get; set; }
		public int rig_tab_ope { get; set; }
		public int rig_cod_ope { get; set; }
		public int rig_gest_lotto { get; set; }
		public string rig_lotto { get; set; }
		public string rig_sscc { get; set; }
		public string rig_gtin { get; set; }
		public DateTime rig_scadenza { get; set; }
		public double rig_coef_mol2 { get; set; }
		public double rig_qta_conf { get; set; }
		public double rig_num_conf { get; set; }
		public double rig_tara { get; set; }
		public double rig_tot_prov { get; set; }
		public int rig_inte { get; set; }
		public DateTime rig_last_update { get; set; }
		public double rig_sco_iva_esc { get; set; }
		public double rig_importo_impo { get; set; }
		public string rig_newdes { get; set; }
		public string rig_user { get; set; }
		public bool rig_check_prezzo { get; set; }
		public double rig_accise { get; set; }
		public double rig_contrassegni { get; set; }
		public double rig_raee { get; set; }
		public double rig_conai { get; set; }
		public double rig_sconto5 { get; set; }
		public double rig_sconto6 { get; set; }
		public double rig_sconto7 { get; set; }
		public bool rig_tara_recalc { get; set; }
		public double rig_tara_altre { get; set; }
		public double rig_tara_imballo { get; set; }
	}
}

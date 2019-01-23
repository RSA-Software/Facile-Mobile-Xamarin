using System;
using SQLite;

namespace Facile.Models
{
	[Table("artcount")]
	public class ArtCounter
	{
		[PrimaryKey, AutoIncrement]
		public int aco_id {get; set;}

		[Indexed(Name = "CodDepAnnoSezTagCol", Order = 1, Unique = true)]
	  	public string aco_codice { get; set; }

		[Indexed(Name = "CodDepAnnoSezTagCol", Order = 2, Unique = true)]
	  	public int aco_dep { get; set; }

	  	[Indexed(Name = "CodDepAnnoSezTagCol", Order = 5, Unique = true)]

	  	public int aco_idxtag { get; set; }
	  	[Indexed(Name = "CodDepAnnoSezTagCol", Order = 6, Unique = true)]

	  	public int aco_col { get; set; }

	  	[Indexed(Name = "CodDepAnnoSezTagCol", Order = 4, Unique = true)]
		public int aco_sez { get; set; }

	  	[Indexed(Name = "CodDepAnnoSezTagCol", Order = 3, Unique = true)]
	  	public int aco_anno { get; set; }

		public double aco_esist_attuale { get; set; }
	  	public double aco_esist_prec { get; set; }
	  	public double aco_qta_rim_iniziale { get; set; }
	  	public double aco_val_rim_iniziale { get; set; }
	  	public double aco_qta_caricata { get; set; }
	  	public double aco_val_caricato { get; set; }
	  	public double aco_qta_resa_for { get; set; }
	  	public double aco_val_reso_for { get; set; }
	  	public double aco_car_nv_cli { get; set; }
	  	public double aco_car_nv_for { get; set; }
	  	public DateTime? aco_ult_acquisto { get; set; }
	  	public double aco_ult_pr_acquisto { get; set; }
	  	public DateTime? aco_pen_acquisto { get; set; }
	  	public double aco_pen_pr_acquisto { get; set; }
	  	public double aco_qta_ord_for { get; set; }
	  	public double aco_qta_lavorazione { get; set; }
	  	public double aco_qta_impegnata { get; set; }
	  	public double aco_val_impegnato { get; set; }
	  	public double aco_qta_venduta { get; set; }
	  	public double aco_val_venduto { get; set; }
	  	public double aco_qta_resa_cli { get; set; }
	  	public double aco_val_reso_cli { get; set; }
	  	public double aco_scar_nv_cli { get; set; }
	  	public double aco_scar_nv_for { get; set; }
	  	public DateTime? aco_ult_vendita { get; set; }
	  	public double aco_venduto_periodo { get; set; }
	  	public double aco_cali_scarti { get; set; }
	  	public double aco_qta_ord_cli { get; set; }
	  	public double aco_quantita { get; set; }
	  	public double aco_valore { get; set; }
	  	public double aco_giacenza { get; set; }
	  	public double aco_spese { get; set; }
	  	public double aco_ult_pr_acq_non_sco { get; set; }
	  	public double aco_sco_acq1 { get; set; }
	  	public double aco_sco_acq2 { get; set; }
	  	public double aco_sco_acq3 { get; set; }
	  	public double aco_sco_acq4 { get; set; }
	  	public double aco_sco_acq5 { get; set; }
	  	public double aco_sco_acq6 { get; set; }
	  	public double aco_sco_acq7 { get; set; }
	  	public double aco_sco_acq8 { get; set; }
	  	public int aco_colli { get; set; }
	  	public int aco_colli_sca { get; set; }
	  	public int aco_iva_ult_pr_acquisto { get; set; }
	  	public int aco_iva_pen_pr_acquisto { get; set; }
	  	public DateTime? aco_last_update { get; set; }
	  	public string aco_user { get; set; }
	  	public int aco_web_id { get; set; }
	  	public DateTime? aco_ult_inventario { get; set; }
	  	public DateTime? aco_ult_ord_cli { get; set; }
	  	public DateTime? aco_ult_ord_for { get; set; }

	}
}

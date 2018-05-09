using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Facile.Interfaces;
using Facile.Models;
using Facile.Utils;
using SQLite;
using Xamarin.Forms;

namespace Facile.Extension
{
	public static class FatRowExtensions 
	{
		public async static Task<double> RecalcAsync(this FatRow rig)
		{
			var dec = 2;

			if (rig.rig_tara_recalc == true || !rig.rig_tara_altre.TestIfZero(3) || !rig.rig_tara_imballo.TestIfZero(3))
			{
				rig.rig_tara = rig.rig_tara_altre + rig.rig_colli * rig.rig_tara_imballo;
			}

			double sco1 = Math.Round((rig.rig_prezzo) * (rig.rig_sconto1 / 100.0), dec + 2, MidpointRounding.AwayFromZero);
			double sco2 = Math.Round(((rig.rig_prezzo - sco1) * (rig.rig_sconto2 / 100.0)), dec + 2, MidpointRounding.AwayFromZero);
			double sco3 = Math.Round(((rig.rig_prezzo - sco1 - sco2) * (rig.rig_sconto3 / 100.0)), dec + 2, MidpointRounding.AwayFromZero);
			double sco4 = Math.Round(((rig.rig_prezzo - sco1 - sco2 - sco3) * (rig.rig_sconto4 / 100.0)), dec + 2, MidpointRounding.AwayFromZero);
			double sco5 = Math.Round(((rig.rig_prezzo - sco1 - sco2 - sco3 - sco4) * (rig.rig_sconto5 / 100.0)), dec + 2, MidpointRounding.AwayFromZero);
			double sco6 = Math.Round(((rig.rig_prezzo - sco1 - sco2 - sco3 - sco4 - sco5) * (rig.rig_sconto6 / 100.0)), dec + 2, MidpointRounding.AwayFromZero);
			double sco7 = Math.Round(((rig.rig_prezzo - sco1 - sco2 - sco3 - sco4 - sco5 - sco6) * (rig.rig_sconto7 / 100.0)), dec + 2, MidpointRounding.AwayFromZero);

			rig.rig_tot_peso = rig.rig_qta * rig.rig_coef_mol * rig.rig_coef_mol2 * rig.rig_peso;
			rig.rig_tot_sconto = (rig.rig_qta - rig.rig_tara) * rig.rig_coef_mol * rig.rig_coef_mol2 * (sco1 + sco2 + sco3 + sco4 + sco5 + sco6 + sco7);
			rig.rig_tot_sconto += rig.rig_scovalore;
			rig.rig_tot_sconto += rig.rig_scomerce * rig.rig_coef_mol * rig.rig_coef_mol2 * (sco1 + sco2 + sco3 + sco4 + sco5 + sco6 + sco7);
			rig.rig_tot_sconto = Math.Round(rig.rig_tot_sconto, dec, MidpointRounding.AwayFromZero);
			double totale = Math.Round(rig.rig_prezzo - sco1 - sco2 - sco3 - sco4 - sco5 - sco6 - sco7, dec + 2, MidpointRounding.AwayFromZero);
			rig.rig_importo = Math.Round((rig.rig_qta - rig.rig_tara - rig.rig_scomerce) * rig.rig_coef_mol * rig.rig_coef_mol2 * (totale + rig.rig_spese) - rig.rig_scovalore, dec, MidpointRounding.AwayFromZero);

			//
			// Codice Aggiunto per il calcolo dello sconto iva esclusa 24/03/2009
			//
			rig.rig_sco_iva_esc = 0;
			rig.rig_importo_impo = rig.rig_importo;
			if (rig.rig_iva_inclusa != 0 && rig.rig_iva != 0)
			{
				try
				{
					SQLiteAsyncConnection dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();
					var iva = await dbcon.GetAsync<Codiva>(rig.rig_iva);
					rig.rig_importo_impo = Math.Round(rig.rig_importo_impo / (1 + (iva.iva_aliq / 100.0)), dec, MidpointRounding.AwayFromZero);
				}
				catch
				{
					throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
				}
			}
			totale = Math.Round((rig.rig_qta - rig.rig_tara - rig.rig_scomerce) * rig.rig_coef_mol * rig.rig_coef_mol2 * (rig.rig_prezzo + rig.rig_spese) - rig.rig_scovalore, dec, MidpointRounding.AwayFromZero);

			if (totale.TestIfZero(4) != true)
			{
				rig.rig_sco_iva_esc = (float)(100 - (rig.rig_importo_impo / totale) * 100);
			}
			// Fine Codice Aggiunto	

			// Torniamo il prezzo unitario
			if (rig.rig_qta.TestIfZero(3) != true)
				return (Math.Round(rig.rig_importo / rig.rig_qta, dec, MidpointRounding.AwayFromZero));
			else
				return (totale);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Facile.Interfaces;
using Facile.Models;
using Facile.Utils;
using SQLite;
using Xamarin.Forms;
using static Facile.Extension.NumericExtensions;

namespace Facile.Extension
{
	public static class FattureExtensions
	{
		private static readonly double LIMITE_BOLLO = 77.47;

		public async static Task GetTotaliAsync(this Fatture fat)
		{
			int i;
			double [] rip_cassa = new double[4];
			var app = (App)Application.Current;
			SQLiteAsyncConnection dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();
			if (app.facile_db_impo == null) throw new ArgumentNullException();

			fat.fat_tot_merce = 0.0;
			fat.fat_tot_netto = 0.0;
			fat.fat_imponibile = 0.0;
			fat.fat_tot_esente = 0.0;
			fat.fat_tot_esclusa = 0.0;
			fat.fat_tot_non_imp = 0.0;
			fat.fat_totale_imponibile = 0.0;
			fat.fat_tot_iva = 0.0;
			fat.fat_tot_fattura = 0.0;
			fat.fat_tot_pagare = 0.0;
			fat.fat_com_importo = 0.0;

			for (i = 0; i < 4; i++)
			{
				rip_cassa[i] = 0.0;
				switch(i)
				{
					case 0 :
						fat.fat_ripartizione_0 = 0.0;
						if (fat.fat_importo_cas_0.TestIfZero((int)DecPlaces.TOT_DECIMAL_PLACES) != true)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_0);
								rip_cassa[i] = Math.Round(fat.fat_imponibile_0 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
							}
							catch 
							{
								throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
							}
						}
						break;

					case 1:
						fat.fat_ripartizione_1 = 0.0;
						if (fat.fat_importo_cas_1.TestIfZero((int)DecPlaces.TOT_DECIMAL_PLACES) != true)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_1);
								rip_cassa[i] = Math.Round(fat.fat_imponibile_1 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
							}
							catch (System.Exception)
							{
								throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
							}
						}
						break;

					case 2:
						fat.fat_ripartizione_2 = 0.0;
						if (fat.fat_importo_cas_2.TestIfZero((int)DecPlaces.TOT_DECIMAL_PLACES) != true)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_2);
								rip_cassa[i] = Math.Round(fat.fat_imponibile_2 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
							}
							catch (System.Exception)
							{
								throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
							}
						}
						break;

					case 3:
						fat.fat_ripartizione_3 = 0.0;
						if (fat.fat_importo_cas_3.TestIfZero((int)DecPlaces.TOT_DECIMAL_PLACES) != true)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_3);
								rip_cassa[i] = Math.Round(fat.fat_imponibile_3 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
							}
							catch (System.Exception)
							{
								throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
							}
						}
						break;
				}
			}

			fat.fat_tot_conai = Math.Round(fat.fat_tot_conai, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
			fat.fat_tot_raee = Math.Round(fat.fat_tot_raee, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
			fat.fat_tot_accise = Math.Round(fat.fat_tot_accise, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
			fat.fat_tot_contrassegni = Math.Round(fat.fat_tot_contrassegni, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);


			// Eseguiamo la ripartizione delle spese 
			double  tot = fat.fat_imponibile_0 + fat.fat_imponibile_1 + fat.fat_imponibile_2 + fat.fat_imponibile_3;

			//-----------------24/03/02 15.05-------------------
			// Codice Aggiunto per il calcolo delle spese di tra-
			// sporto e accessorie.
			//--------------------------------------------------
			if (fat.fat_recalc_trasp != 0) fat.fat_imballo = 0.0;
			if (fat.fat_recalc_incef != 0) fat.fat_inc_eff = 0.0;
			if (fat.fat_recalc_varie != 0) fat.fat_varie = 0.0;
			if (fat.fat_recalc_spese != 0) fat.fat_art15 = 0.0;
			if (fat.fat_recalc_bolli != 0) fat.fat_bolli_eff = 0.0;

			if (((fat.fat_tipo == (int)DocTipo.TIPO_FAT) || (fat.fat_tipo == (int)DocTipo.TIPO_ACC)) || (fat.fat_tipo == (int)DocTipo.TIPO_RIC))
			{
				if (fat.fat_credito == 0)
				{
					if ((fat.fat_spese != 0) && (fat.fat_recalc_trasp != 0))
					{
						double perc = 0.0;
						double importo_merci = tot - tot * (fat.fat_sconto / 100.0);
						for (int xxx = 0; xxx < 5; xxx++)
						{
							double spetra_imp;
							double spetra_per;
							switch(xxx)
							{
								case  0: 
									spetra_imp = app.facile_db_impo.dit_spetra_imp_0;
									spetra_per = app.facile_db_impo.dit_spetra_per_0;
									break;

								case  1: 
									spetra_imp = app.facile_db_impo.dit_spetra_imp_1;
									spetra_per = app.facile_db_impo.dit_spetra_per_1;
									break;

								case  2: 
									spetra_imp = app.facile_db_impo.dit_spetra_imp_2;
									spetra_per = app.facile_db_impo.dit_spetra_per_2;
									break;

								case  3: 
									spetra_imp = app.facile_db_impo.dit_spetra_imp_3;
									spetra_per = app.facile_db_impo.dit_spetra_per_3;
									break;

								default: 
									spetra_imp = app.facile_db_impo.dit_spetra_imp_4;
									spetra_per = app.facile_db_impo.dit_spetra_per_4;
									break;
							}
							if (importo_merci <= spetra_imp)
							{
								perc = spetra_per;
								break;
							}
						}
						fat.fat_imballo = Math.Round(importo_merci * (perc / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
						fat.fat_imballo += app.facile_db_impo.dit_spetra;
					}

					if ((fat.fat_spese != 0) && (fat.fat_recalc_varie != 0))
					{
						fat.fat_varie = app.facile_db_impo.dit_fat_spese;
					}

					if ((fat.fat_recalc_incef != 0) || (fat.fat_recalc_bolli != 0))
					{
						try
						{
							var pag = await dbcon.GetAsync<Pagamenti>(fat.fat_pag);
							if (((fat.fat_spese != 0) && (pag.pag_banche != 0)) && (fat.fat_recalc_incef != 0))
							{
								fat.fat_inc_eff = pag.pag_tot_banche;
							}
							if (((fat.fat_bolli != 0) && (pag.pag_bolli != 0)) && (fat.fat_recalc_bolli != 0))
							{
								fat.fat_bolli_eff += pag.pag_tot_bolli;
							}
						}
						catch (System.Exception)
						{
							throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
						}
					}
				}
			}

			//-----------------24/03/02 15.30-------------------
			//	Fine Codice Aggiunto
			//--------------------------------------------------

			fat.fat_tot_non_docum = fat.fat_imballo + fat.fat_varie + fat.fat_inc_eff;

			// Ripartizione Spese
			// 
			// Se il codice iva spese non è valido o pari a zero la ripartizione avviene in proporzione alle 
			// aliquote iva presenti nel documento, altrimenti vengono assoggettate all' aliquota iva preimpo-
			// stata.
			if (app.facile_db_impo.dit_iva_spese == 0)
			{
				if (tot.TestIfZero(0) != true)
				{
					fat.fat_ripartizione_0 = fat.fat_tot_non_docum * (fat.fat_imponibile_0 / tot);
					fat.fat_ripartizione_0.MyFloor(2);

					fat.fat_ripartizione_1 = fat.fat_tot_non_docum * (fat.fat_imponibile_1 / tot);
					fat.fat_ripartizione_1.MyFloor(2);

					fat.fat_ripartizione_2 = fat.fat_tot_non_docum * (fat.fat_imponibile_2 / tot);
					fat.fat_ripartizione_2.MyFloor(2);

					fat.fat_ripartizione_3 = fat.fat_tot_non_docum * (fat.fat_imponibile_3 / tot);
					fat.fat_ripartizione_3.MyFloor(2);
				}
				tot = fat.fat_ripartizione_0 + fat.fat_ripartizione_1 + fat.fat_ripartizione_2 + fat.fat_ripartizione_3;
				tot = fat.fat_tot_non_docum - tot;
				int max = 0;
				var aliquota_max = fat.fat_aliquota_0;
				for (i = 1; i < 4; i++)
				{
					double aliquota;
					double imponibile;

					switch(i)
					{
						case 1:
							aliquota = fat.fat_aliquota_1;
							imponibile = fat.fat_imponibile_1;
							break;

						case 2:
							aliquota = fat.fat_aliquota_2;
							imponibile = fat.fat_imponibile_2;
							break;

						default:
							aliquota = fat.fat_aliquota_3;
							imponibile = fat.fat_imponibile_3;
							break;
					}
					if ((aliquota > aliquota_max) && (imponibile.TestIfZero(0) != true))
					{
						max = i;
						aliquota_max = aliquota;
					}
				}
				switch (max)
				{
					case  0: fat.fat_ripartizione_0 += tot; break;
					case  1: fat.fat_ripartizione_1 += tot; break;
					case  2: fat.fat_ripartizione_2 += tot; break;
					default: fat.fat_ripartizione_3 += tot; break;
				}
			}
			else
			{
				bool found = false;
				do
				{
					if (fat.fat_cod_iva_0 == app.facile_db_impo.dit_iva_spese)
					{
						fat.fat_ripartizione_0 = fat.fat_tot_non_docum;
						found = true;
						break;
					}
					if (fat.fat_cod_iva_1 == app.facile_db_impo.dit_iva_spese)
					{
						fat.fat_ripartizione_1 = fat.fat_tot_non_docum;
						found = true;
						break;
					}
					if (fat.fat_cod_iva_2 == app.facile_db_impo.dit_iva_spese)
					{
						fat.fat_ripartizione_2 = fat.fat_tot_non_docum;
						found = true;
						break;
					}
					if (fat.fat_cod_iva_3 == app.facile_db_impo.dit_iva_spese)
					{
						fat.fat_ripartizione_3 = fat.fat_tot_non_docum;
						found = true;
						break;
					}
				} while (false);

				if (found == false)
				{
					while (true)
					{
						if (fat.fat_cod_iva_0 == 0)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(app.facile_db_impo.dit_iva_spese);
								fat.fat_cod_iva_0 = app.facile_db_impo.dit_iva_spese;
								fat.fat_desc_iva_0 =  iva.iva_desc;
								fat.fat_tipo_iva_0 = iva.iva_tip;
								if (iva.iva_tip == 1) fat.fat_aliquota_0 = iva.iva_aliq;
								fat.fat_ripartizione_0 = fat.fat_tot_non_docum;
								break;

							}
							catch (System.Exception)
							{
								throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
							}
						}

						if (fat.fat_cod_iva_1 == 0)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(app.facile_db_impo.dit_iva_spese);
								fat.fat_cod_iva_1 = app.facile_db_impo.dit_iva_spese;
								fat.fat_desc_iva_1 = iva.iva_desc;
								fat.fat_tipo_iva_1 = iva.iva_tip;
								if (iva.iva_tip == 1) fat.fat_aliquota_1 = iva.iva_aliq;
								fat.fat_ripartizione_1 = fat.fat_tot_non_docum;
								break;

							}
							catch (System.Exception)
							{
								throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
							}
						}

						if (fat.fat_cod_iva_2 == 0)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(app.facile_db_impo.dit_iva_spese);
								fat.fat_cod_iva_2 = app.facile_db_impo.dit_iva_spese;
								fat.fat_desc_iva_2 = iva.iva_desc;
								fat.fat_tipo_iva_2 = iva.iva_tip;
								if (iva.iva_tip == 1) fat.fat_aliquota_2 = iva.iva_aliq;
								fat.fat_ripartizione_2 = fat.fat_tot_non_docum;
								break;

							}
							catch (System.Exception)
							{
								throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
							}
						}

						if (fat.fat_cod_iva_3 == 0)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(app.facile_db_impo.dit_iva_spese);
								fat.fat_cod_iva_3 = app.facile_db_impo.dit_iva_spese;
								fat.fat_desc_iva_3 = iva.iva_desc;
								fat.fat_tipo_iva_3 = iva.iva_tip;
								if (iva.iva_tip == 1) fat.fat_aliquota_3 = iva.iva_aliq;
								fat.fat_ripartizione_3 = fat.fat_tot_non_docum;
								break;
							}
							catch (System.Exception)
							{
								throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
							}
						}
						throw new RsaException(RsaException.TroppiMsg, RsaException.TroppiErr);
					} 
				}
			}

			//
			// calcoliamo i totali dell'IVA
			//

			// Prima Aliquota IVA
			do
			{
				fat.fat_importo_0 = 0.0;
				if (fat.fat_cod_iva_0 == 0L) break;

				fat.fat_ripartizione_0 += rip_cassa[0];

				fat.fat_tot_merce += fat.fat_imponibile_0.MyFloor((int)DecPlaces.TOT_DECIMAL_PLACES);

				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_0 = Math.Round(fat.des_imponibile_ivato_0 - fat.des_imponibile_ivato_0 * fat.fat_sconto / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					double xxxtot = fat.fat_importo_0;
					fat.fat_importo_0 = Math.Round(fat.fat_importo_0 / (1 + (fat.fat_aliquota_0 / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					double xxxiva = Math.Round(fat.fat_importo_0 * (1 + (fat.fat_aliquota_0 / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					xxxiva -= xxxtot;
					xxxiva = Math.Round(xxxiva, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					if (xxxiva.TestIfZero((int)DecPlaces.TOT_DECIMAL_PLACES) != true)
					{
						fat.fat_dec_iva_0 = (float)xxxiva;
					}
				}
				else
				{
					fat.fat_importo_0 = fat.fat_imponibile_0 - fat.fat_imponibile_0 * fat.fat_sconto / 100.0;
				}
//#ifdef BOLLICINE
//				if ((!fat.fat_vuo_cauzione.TestIfZero(dec)) && (fat.fat_cod_iva_0 == facile_db_impo.cod_iva_ese))
//				{
//					fat.fat_imponibile_0 += fat.fat_vuo_cauzione;
//					fat.des_imponibile_ivato_0 += fat.fat_vuo_cauzione;
//					fat.fat_importo_0 += fat.fat_vuo_cauzione;
//				}
//#endif

				fat.fat_importo_iva_0 = Math.Round((fat.fat_importo_0 + fat.fat_ripartizione_0) * fat.fat_aliquota_0 / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

				//
				// Codice aggiunto per adeguamento imponibile ed iva con terza cifra decimale = 5 (Es. 11.61/1.20) 
				//
				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_iva_0 -= fat.fat_dec_iva_0;

					double val = fat.fat_importo_iva_0 + fat.fat_importo_0;
					double target = Math.Round(fat.des_imponibile_ivato_0 - fat.des_imponibile_ivato_0 * fat.fat_sconto / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					val = Math.Round(val - target, 2, MidpointRounding.AwayFromZero);
					if (val <= 0.0100000000000000000)
					{
						fat.fat_importo_iva_0 -= val;
						fat.fat_dec_iva_0 -= (float)val;
					}
				}
				// Fine Codice Aggiunto

				//******************************************************************
				// Modificato Arrotondamento il 27/03/2001                        
				//fat->rec.importo[i]     = MyFloor(fat->rec.importo[i],impo.dec);
				fat.fat_importo_0 = Math.Round(fat.fat_importo_0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

				switch (fat.fat_tipo_iva_0)
				{
					case 3:
					case 6:
						fat.fat_tot_non_imp += fat.fat_importo_0 + fat.fat_ripartizione_0;
						fat.fat_totale_imponibile += fat.fat_importo_0 + fat.fat_ripartizione_0;
						break;

					case 4:
						fat.fat_tot_esclusa += fat.fat_importo_0 + fat.fat_ripartizione_0;
						fat.fat_totale_imponibile += fat.fat_importo_0 + fat.fat_ripartizione_0;
						break;

					case 2:
						fat.fat_tot_esente += fat.fat_importo_0 + fat.fat_ripartizione_0;
						fat.fat_totale_imponibile += fat.fat_importo_0 + fat.fat_ripartizione_0;
						break;

					default:
						fat.fat_imponibile += fat.fat_importo_0 + fat.fat_ripartizione_0;
						fat.fat_totale_imponibile += fat.fat_importo_0 + fat.fat_ripartizione_0;
						break;
				}
				fat.fat_tot_netto += fat.fat_importo_0;
				fat.fat_tot_iva += fat.fat_importo_iva_0;

				// Aggiunto il 27/03/2001 
				fat.fat_tot_fattura += fat.fat_importo_0;
				if (fat.fat_scorporo != 0)
				{
					double diff;

					fat.fat_tot_merce += Math.Round(fat.fat_imponibile_0 * fat.fat_aliquota_0 / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_tot_netto += Math.Round(fat.fat_importo_0 * fat.fat_aliquota_0 / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

					// Codice Aggiunto il 31/03/2004 Gestione importi a scorporo con terzo dec. = 5
					diff = fat.fat_tot_merce - fat.fat_tot_netto;
					fat.fat_tot_merce -= fat.fat_dec_iva_0;
					if (diff.TestIfZero(2) == true) fat.fat_tot_netto -= fat.fat_dec_iva_0;
					// Fine 
				}
				// Fine Codice Aggiunto   
			} while (false);
	

			// Seconda Aliquota IVA
			do
			{
				fat.fat_importo_1 = 0.0;
				if (fat.fat_cod_iva_1 == 0L) break;

				fat.fat_ripartizione_1 += rip_cassa[1];

				fat.fat_tot_merce += fat.fat_imponibile_1.MyFloor((int)DecPlaces.TOT_DECIMAL_PLACES);

				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_1 = Math.Round(fat.des_imponibile_ivato_1 - fat.des_imponibile_ivato_1 * fat.fat_sconto / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					double xxxtot = fat.fat_importo_1;
					fat.fat_importo_1 = Math.Round(fat.fat_importo_1 / (1 + (fat.fat_aliquota_1 / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					double xxxiva = Math.Round(fat.fat_importo_1 * (1 + (fat.fat_aliquota_1 / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					xxxiva -= xxxtot;
					xxxiva = Math.Round(xxxiva, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					if (xxxiva.TestIfZero((int)DecPlaces.TOT_DECIMAL_PLACES) != true)
					{
						fat.fat_dec_iva_1 = (float)xxxiva;
					}
				}
				else
				{
					fat.fat_importo_1 = fat.fat_imponibile_1 - fat.fat_imponibile_1 * fat.fat_sconto / 100.0;
				}
/*
	#ifdef BOLLICINE
				if ((TestIfZero(&fat->rec.vuo_cauzione, dec) == FALSE) && (fat->rec.cod_iva[i] == facile_db_impo.cod_iva_ese))
				{
					fat->rec.imponibile[i] += fat->rec.vuo_cauzione;
					fat->des.imponibile_ivato[i] += fat->rec.vuo_cauzione;
					fat->rec.importo[i] += fat->rec.vuo_cauzione;
				}
	#endif
*/
				fat.fat_importo_iva_1 = Math.Round((fat.fat_importo_1 + fat.fat_ripartizione_1) * fat.fat_aliquota_1 / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

				//
				// Codice aggiunto per adeguamento imponibile ed iva con terza cifra decimale = 5 (Es. 11.61/1.20) 
				//
				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_iva_1 -= fat.fat_dec_iva_1;

					double val = fat.fat_importo_iva_1 + fat.fat_importo_1;
					double target = Math.Round(fat.des_imponibile_ivato_1 - fat.des_imponibile_ivato_1 * fat.fat_sconto / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					val = Math.Round(val - target, 2, MidpointRounding.AwayFromZero);
					if (val <= 0.0100000000000000000)
					{
						fat.fat_importo_iva_1 -= val;
						fat.fat_dec_iva_1 -= (float)val;
					}
				}
				// Fine Codice Aggiunto

				//******************************************************************
				// Modificato Arrotondamento il 27/03/2001                        
				//fat->rec.importo[i]     = MyFloor(fat->rec.importo[i],impo.dec);
				fat.fat_importo_1 = Math.Round(fat.fat_importo_1, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

				switch (fat.fat_tipo_iva_1)
				{
					case 3:
					case 6:
						fat.fat_tot_non_imp += fat.fat_importo_1 + fat.fat_ripartizione_1;
						fat.fat_totale_imponibile += fat.fat_importo_1 + fat.fat_ripartizione_1;
						break;

					case 4:
						fat.fat_tot_esclusa += fat.fat_importo_1 + fat.fat_ripartizione_1;
						fat.fat_totale_imponibile += fat.fat_importo_1 + fat.fat_ripartizione_1;
						break;

					case 2:
						fat.fat_tot_esente += fat.fat_importo_1 + fat.fat_ripartizione_1;
						fat.fat_totale_imponibile += fat.fat_importo_1 + fat.fat_ripartizione_1;
						break;

					default:
						fat.fat_imponibile += fat.fat_importo_1 + fat.fat_ripartizione_1;
						fat.fat_totale_imponibile += fat.fat_importo_1 + fat.fat_ripartizione_1;
						break;
				}
				fat.fat_tot_netto += fat.fat_importo_1;
				fat.fat_tot_iva += fat.fat_importo_iva_1;

				// Aggiunto il 27/03/2001 
				fat.fat_tot_fattura += fat.fat_importo_1;
				if (fat.fat_scorporo != 0)
				{
					double diff;

					fat.fat_tot_merce += Math.Round(fat.fat_imponibile_1 * fat.fat_aliquota_1 / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_tot_netto += Math.Round(fat.fat_importo_1 * fat.fat_aliquota_1 / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

					// Codice Aggiunto il 31/03/2004 Gestione importi a scorporo con terzo dec. = 5
					diff = fat.fat_tot_merce - fat.fat_tot_netto;
					fat.fat_tot_merce -= fat.fat_dec_iva_1;
					if (diff.TestIfZero(2) == true) fat.fat_tot_netto -= fat.fat_dec_iva_1;
					// Fine 
				}
				// Fine Codice Aggiunto   

			} while (false);

			// Terza Aliquota IVA
			do
			{
				fat.fat_importo_2 = 0.0;
				if (fat.fat_cod_iva_2 == 0L) break;

				fat.fat_ripartizione_2 += rip_cassa[2];

				fat.fat_tot_merce += fat.fat_imponibile_2.MyFloor((int)DecPlaces.TOT_DECIMAL_PLACES);

				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_2 = Math.Round(fat.des_imponibile_ivato_2 - fat.des_imponibile_ivato_2 * fat.fat_sconto / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					double xxxtot = fat.fat_importo_2;
					fat.fat_importo_2 = Math.Round(fat.fat_importo_2 / (1 + (fat.fat_aliquota_2 / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					double xxxiva = Math.Round(fat.fat_importo_2 * (1 + (fat.fat_aliquota_2 / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					xxxiva -= xxxtot;
					xxxiva = Math.Round(xxxiva, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					if (xxxiva.TestIfZero((int)DecPlaces.TOT_DECIMAL_PLACES) != true)
					{
						fat.fat_dec_iva_2 = (float)xxxiva;
					}
				}
				else
				{
					fat.fat_importo_2 = fat.fat_imponibile_2 - fat.fat_imponibile_2 * fat.fat_sconto / 100.0;
				}
/*
	#ifdef BOLLICINE
				if ((TestIfZero(&fat->rec.vuo_cauzione, dec) == FALSE) && (fat->rec.cod_iva[i] == facile_db_impo.cod_iva_ese))
				{
					fat->rec.imponibile[i] += fat->rec.vuo_cauzione;
					fat->des.imponibile_ivato[i] += fat->rec.vuo_cauzione;
					fat->rec.importo[i] += fat->rec.vuo_cauzione;
				}
	#endif
*/
				fat.fat_importo_iva_2 = Math.Round((fat.fat_importo_2 + fat.fat_ripartizione_2) * fat.fat_aliquota_2 / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

				//
				// Codice aggiunto per adeguamento imponibile ed iva con terza cifra decimale = 5 (Es. 11.61/1.20) 
				//
				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_iva_2 -= fat.fat_dec_iva_2;

					double val = fat.fat_importo_iva_2 + fat.fat_importo_2;
					double target = Math.Round(fat.des_imponibile_ivato_2 - fat.des_imponibile_ivato_2 * fat.fat_sconto / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					val = Math.Round(val - target, 2, MidpointRounding.AwayFromZero);
					if (val <= 0.0100000000000000000)
					{
						fat.fat_importo_iva_2 -= val;
						fat.fat_dec_iva_2 -= (float)val;
					}
				}
				// Fine Codice Aggiunto

				//******************************************************************
				// Modificato Arrotondamento il 27/03/2001                        
				//fat->rec.importo[i]     = MyFloor(fat->rec.importo[i],impo.dec);
				fat.fat_importo_2 = Math.Round(fat.fat_importo_2, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

				switch (fat.fat_tipo_iva_2)
				{
					case 3:
					case 6:
						fat.fat_tot_non_imp += fat.fat_importo_2 + fat.fat_ripartizione_2;
						fat.fat_totale_imponibile += fat.fat_importo_2 + fat.fat_ripartizione_2;
						break;

					case 4:
						fat.fat_tot_esclusa += fat.fat_importo_2 + fat.fat_ripartizione_2;
						fat.fat_totale_imponibile += fat.fat_importo_2 + fat.fat_ripartizione_2;
						break;

					case 2:
						fat.fat_tot_esente += fat.fat_importo_2 + fat.fat_ripartizione_2;
						fat.fat_totale_imponibile += fat.fat_importo_2 + fat.fat_ripartizione_2;
						break;

					default:
						fat.fat_imponibile += fat.fat_importo_2 + fat.fat_ripartizione_2;
						fat.fat_totale_imponibile += fat.fat_importo_2 + fat.fat_ripartizione_2;
						break;
				}
				fat.fat_tot_netto += fat.fat_importo_2;
				fat.fat_tot_iva += fat.fat_importo_iva_2;

				// Aggiunto il 27/03/2001 
				fat.fat_tot_fattura += fat.fat_importo_2;
				if (fat.fat_scorporo != 0)
				{
					double diff;

					fat.fat_tot_merce += Math.Round(fat.fat_imponibile_2 * fat.fat_aliquota_2 / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_tot_netto += Math.Round(fat.fat_importo_2 * fat.fat_aliquota_2 / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

					// Codice Aggiunto il 31/03/2004 Gestione importi a scorporo con terzo dec. = 5
					diff = fat.fat_tot_merce - fat.fat_tot_netto;
					fat.fat_tot_merce -= fat.fat_dec_iva_2;
					if (diff.TestIfZero(2) == true) fat.fat_tot_netto -= fat.fat_dec_iva_2;
					// Fine 
				}
				// Fine Codice Aggiunto   

			} while (false);

			// Quarta Aliquota IVA
			do
			{
				fat.fat_importo_3 = 0.0;
				if (fat.fat_cod_iva_3 == 0L) break;

				fat.fat_ripartizione_3 += rip_cassa[3];

				fat.fat_tot_merce += fat.fat_imponibile_3.MyFloor((int)DecPlaces.TOT_DECIMAL_PLACES);

				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_3 = Math.Round(fat.des_imponibile_ivato_3 - fat.des_imponibile_ivato_3 * fat.fat_sconto / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					double xxxtot = fat.fat_importo_3;
					fat.fat_importo_3 = Math.Round(fat.fat_importo_3 / (1 + (fat.fat_aliquota_3 / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					double xxxiva = Math.Round(fat.fat_importo_3 * (1 + (fat.fat_aliquota_3 / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					xxxiva -= xxxtot;
					xxxiva = Math.Round(xxxiva, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					if (xxxiva.TestIfZero((int)DecPlaces.TOT_DECIMAL_PLACES) != true)
					{
						fat.fat_dec_iva_3 = (float)xxxiva;
					}
				}
				else
				{
					fat.fat_importo_3 = fat.fat_imponibile_3 - fat.fat_imponibile_3 * fat.fat_sconto / 100.0;
				}
/*
	#ifdef BOLLICINE
				if ((TestIfZero(&fat->rec.vuo_cauzione, dec) == FALSE) && (fat->rec.cod_iva[i] == facile_db_impo.cod_iva_ese))
				{
					fat->rec.imponibile[i] += fat->rec.vuo_cauzione;
					fat->des.imponibile_ivato[i] += fat->rec.vuo_cauzione;
					fat->rec.importo[i] += fat->rec.vuo_cauzione;
				}
	#endif
*/
				fat.fat_importo_iva_3 = Math.Round((fat.fat_importo_3 + fat.fat_ripartizione_3) * fat.fat_aliquota_3 / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

				//
				// Codice aggiunto per adeguamento imponibile ed iva con terza cifra decimale = 5 (Es. 11.61/1.20) 
				//
				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_iva_3 -= fat.fat_dec_iva_3;

					double val = fat.fat_importo_iva_3 + fat.fat_importo_3;
					double target = Math.Round(fat.des_imponibile_ivato_3 - fat.des_imponibile_ivato_3 * fat.fat_sconto / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					val = Math.Round(val - target, 2, MidpointRounding.AwayFromZero);
					if (val <= 0.0100000000000000000)
					{
						fat.fat_importo_iva_3 -= val;
						fat.fat_dec_iva_3 -= (float)val;
					}
				}
				// Fine Codice Aggiunto

				//******************************************************************
				// Modificato Arrotondamento il 27/03/2001                        
				//fat->rec.importo[i]     = MyFloor(fat->rec.importo[i],impo.dec);
				fat.fat_importo_3 = Math.Round(fat.fat_importo_3, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

				switch (fat.fat_tipo_iva_3)
				{
					case 3:
					case 6:
						fat.fat_tot_non_imp += fat.fat_importo_3 + fat.fat_ripartizione_3;
						fat.fat_totale_imponibile += fat.fat_importo_3 + fat.fat_ripartizione_3;
						break;

					case 4:
						fat.fat_tot_esclusa += fat.fat_importo_3 + fat.fat_ripartizione_3;
						fat.fat_totale_imponibile += fat.fat_importo_3 + fat.fat_ripartizione_3;
						break;

					case 2:
						fat.fat_tot_esente += fat.fat_importo_3 + fat.fat_ripartizione_3;
						fat.fat_totale_imponibile += fat.fat_importo_3 + fat.fat_ripartizione_3;
						break;

					default:
						fat.fat_imponibile += fat.fat_importo_3 + fat.fat_ripartizione_3;
						fat.fat_totale_imponibile += fat.fat_importo_3 + fat.fat_ripartizione_3;
						break;
				}
				fat.fat_tot_netto += fat.fat_importo_3;
				fat.fat_tot_iva += fat.fat_importo_iva_3;

				// Aggiunto il 27/03/2001 
				fat.fat_tot_fattura += fat.fat_importo_3;
				if (fat.fat_scorporo != 0)
				{
					double diff;

					fat.fat_tot_merce += Math.Round(fat.fat_imponibile_3 * fat.fat_aliquota_3 / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_tot_netto += Math.Round(fat.fat_importo_3 * fat.fat_aliquota_3 / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

					// Codice Aggiunto il 31/03/2004 Gestione importi a scorporo con terzo dec. = 5
					diff = fat.fat_tot_merce - fat.fat_tot_netto;
					fat.fat_tot_merce -= fat.fat_dec_iva_3;
					if (diff.TestIfZero(2) == true) fat.fat_tot_netto -= fat.fat_dec_iva_3;
					// Fine 
				}
				// Fine Codice Aggiunto   

			} while (false);
			//
			// Fine Calcolo Totali Iva
			//


			// Codice Aggiunto il 25/05/2006 per evitare che il totale merci sia inferiore al totale netto
			if ((fat.fat_tot_merce > 0.0) && (fat.fat_tot_merce < fat.fat_tot_netto)) fat.fat_tot_merce = fat.fat_tot_netto;
			// Fine

			//
			// Riporto automatico bollo per fatture con ritenuta acconto
			//
			if (fat.fat_tot_netto > FattureExtensions.LIMITE_BOLLO)
			{
				double val = fat.fat_tot_netto - FattureExtensions.LIMITE_BOLLO;
				if (val.TestIfZero((int)DecPlaces.TOT_DECIMAL_PLACES) != true) fat.fat_bolli_eff += app.facile_db_impo.dit_bolli;
			}
			// Fine

			//**********************************
			// Codice Modificato il 27/03/2001 
			// fat->rec.tot_fattura = fat->rec.tot_netto + fat->rec.tot_non_docum  + fat->rec.tot_iva + fat->rec.art15 + fat->rec.bolli_eff;

			//
			// Codice Modificato il 26/01/2004 Calcolo spese bolli per tratte
			//
			if (((fat.fat_tipo == (int)DocTipo.TIPO_FAT) || (fat.fat_tipo == (int)DocTipo.TIPO_ACC)) || (fat.fat_tipo == (int)DocTipo.TIPO_RIC))
			{
				if (((fat.fat_credito == 0) && (fat.fat_bolli != 0)) && (fat.fat_recalc_bolli != 0))
				{
					try
					{
						var pag = await dbcon.GetAsync<Pagamenti>(fat.fat_pag);
						if (pag.pag_tipo_pag == (int)TipoPagamento.PAG_TRATTA)
						{
							double totval = fat.fat_tot_fattura + fat.fat_tot_non_docum + fat.fat_tot_iva + fat.fat_art15 + fat.fat_bolli_eff;
							totval = Math.Round(totval * (app.facile_db_impo.dit_trat_perc_bolli / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
							fat.fat_bolli_eff += totval;
						}
					}
					catch 
					{
						throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
					}
				}
			}
			// Fine Variazione

			fat.fat_tot_fattura += fat.fat_tot_non_docum + fat.fat_tot_iva + fat.fat_art15 + fat.fat_bolli_eff + fat.fat_cassa;
			fat.fat_tot_fattura -= fat.fat_omaggi;
			fat.fat_tot_pagare = fat.fat_tot_fattura - fat.fat_abbuoni - fat.fat_anticipo - fat.fat_ritenuta_acconto;
			if (fat.fat_split_payment == true) fat.fat_tot_pagare -= fat.fat_tot_iva;
			fat.fat_tot_sconto += (float)(fat.fat_tot_merce - fat.fat_tot_netto);

/*
	#ifdef BOLLICINE
			for (i = 0; i < MAX_VUOTI; i++)
			{
				fat->rec.vuo_diff[i] = fat->rec.vuo_prec[i] + fat->rec.vuo_cons[i] - fat->rec.vuo_resi[i];
				fat->rec.vuo_tot_cau[i] = Round(fat->rec.vuo_diff[i] * fat->rec.vuo_pre_cau[i], dec);
			}
			fat->rec.tot_pagare -= fat->rec.vuo_cauzione_resa;
	#endif
*/
			if (fat.fat_rie == false)
			{
				fat.fat_com_imponibile = 0.0;
				fat.fat_com_iva = 0.0;
				fat.fat_com_importo = 0.0;
				if (fat.fat_com == true)
				{

					try
					{
						Codiva iva = await dbcon.GetAsync<Codiva>(app.facile_db_impo.dit_iva_age);
						double al_iva = 0.0;
						if (iva.iva_tip == 1) al_iva = iva.iva_aliq;
						if (fat.fat_imp != 0)
							fat.fat_com_imponibile = Math.Round(fat.fat_tot_fattura * (fat.fat_agz_perc / 100.0), 2, MidpointRounding.AwayFromZero);
						else
							fat.fat_com_imponibile = Math.Round(fat.fat_totale_imponibile * (fat.fat_agz_perc / 100.0), 2, MidpointRounding.AwayFromZero);
						fat.fat_com_iva = Math.Round(fat.fat_com_imponibile * (al_iva / 100.0), 2, MidpointRounding.AwayFromZero);
						fat.fat_com_importo = fat.fat_com_imponibile + fat.fat_com_iva;

					}
					catch (Exception e)
					{
						Debug.WriteLine(e.Message);
					}
				}
			}

			//
			// Rimuoviamo le aliquote Iva non Utilizzate e
			// 
			// 
			if (fat.fat_cod_iva_0 != 0)
			{
				var test = fat.fat_ripartizione_0.TestIfZero(2);
				if (fat.fat_imponibile_0.TestIfZero(2) != true) test = false;
				if (fat.fat_importo_0.TestIfZero(2) != true) test = false;
				if (fat.fat_importo_iva_0.TestIfZero(2) != true) test = false;
				if (test)
				{
					fat.fat_cod_iva_0 = 0;
					fat.fat_aliquota_0 = 0.0;
					fat.fat_tipo_iva_0 = 0;
					fat.fat_desc_iva_0 = String.Empty;
				}
			}
			if (fat.fat_cod_iva_1 != 0)
			{
				var test = fat.fat_ripartizione_1.TestIfZero(2);
				if (fat.fat_imponibile_1.TestIfZero(2) != true) test = false;
				if (fat.fat_importo_1.TestIfZero(2) != true) test = false;
				if (fat.fat_importo_iva_1.TestIfZero(2) != true) test = false;
				if (test)
				{
					fat.fat_cod_iva_1 = 0;
					fat.fat_aliquota_1 = 0.0;
					fat.fat_tipo_iva_1 = 0;
					fat.fat_desc_iva_1 = String.Empty;
				}
			}
			if (fat.fat_cod_iva_2 != 0)
			{
				var test = fat.fat_ripartizione_2.TestIfZero(2);
				if (fat.fat_imponibile_2.TestIfZero(2) != true) test = false;
				if (fat.fat_importo_2.TestIfZero(2) != true) test = false;
				if (fat.fat_importo_iva_2.TestIfZero(2) != true) test = false;
				if (test)
				{
					fat.fat_cod_iva_2 = 0;
					fat.fat_aliquota_2 = 0.0;
					fat.fat_tipo_iva_2 = 0;
					fat.fat_desc_iva_2 = String.Empty;
				}
			}
			if (fat.fat_cod_iva_3 != 0)
			{
				var test = fat.fat_ripartizione_3.TestIfZero(2);
				if (fat.fat_imponibile_3.TestIfZero(2) != true) test = false;
				if (fat.fat_importo_3.TestIfZero(2) != true) test = false;
				if (fat.fat_importo_iva_3.TestIfZero(2) != true) test = false;
				if (test)
				{
					fat.fat_cod_iva_3 = 0;
					fat.fat_aliquota_3 = 0.0;
					fat.fat_tipo_iva_3 = 0;
					fat.fat_desc_iva_3 = String.Empty;
				}
			}

			//
			// Rimuoviamo le aliquote vuote
			//
			if (fat.fat_cod_iva_0 == 0)
			{
				fat.fat_cod_iva_0 = fat.fat_cod_iva_1;
				fat.fat_ripartizione_0 = fat.fat_ripartizione_1;
				fat.fat_imponibile_0 = fat.fat_imponibile_1;
				fat.fat_importo_0 = fat.fat_importo_1;
				fat.fat_aliquota_0 = fat.fat_aliquota_1;
				fat.fat_importo_iva_0 = fat.fat_importo_iva_1;
				fat.fat_tipo_iva_0 = fat.fat_tipo_iva_1;
				fat.fat_desc_iva_0 = fat.fat_desc_iva_1;

				fat.fat_cod_iva_1 = 0;
				fat.fat_ripartizione_1 = 0.0;
				fat.fat_imponibile_1 = 0.0;
				fat.fat_importo_1 = 0.0;
				fat.fat_aliquota_1 = 0.0;
				fat.fat_importo_iva_1 = 0.0;
				fat.fat_tipo_iva_1 = 0;
				fat.fat_desc_iva_1 = string.Empty;
			}
			if (fat.fat_cod_iva_1 == 0)
			{
				fat.fat_cod_iva_1 = fat.fat_cod_iva_2;
				fat.fat_ripartizione_1 = fat.fat_ripartizione_2;
				fat.fat_imponibile_1 = fat.fat_imponibile_2;
				fat.fat_importo_1 = fat.fat_importo_2;
				fat.fat_aliquota_1 = fat.fat_aliquota_2;
				fat.fat_importo_iva_1 = fat.fat_importo_iva_2;
				fat.fat_tipo_iva_1 = fat.fat_tipo_iva_2;
				fat.fat_desc_iva_1 = fat.fat_desc_iva_2;

				fat.fat_cod_iva_2 = 0;
				fat.fat_ripartizione_2 = 0.0;
				fat.fat_imponibile_2 = 0.0;
				fat.fat_importo_2 = 0.0;
				fat.fat_aliquota_2 = 0.0;
				fat.fat_importo_iva_2 = 0.0;
				fat.fat_tipo_iva_2 = 0;
				fat.fat_desc_iva_2 = string.Empty;
			}
			if (fat.fat_cod_iva_2 == 0)
			{
				fat.fat_cod_iva_2 = fat.fat_cod_iva_3;
				fat.fat_ripartizione_2 = fat.fat_ripartizione_3;
				fat.fat_imponibile_2 = fat.fat_imponibile_3;
				fat.fat_importo_2 = fat.fat_importo_3;
				fat.fat_aliquota_2 = fat.fat_aliquota_3;
				fat.fat_importo_iva_2 = fat.fat_importo_iva_3;
				fat.fat_tipo_iva_2 = fat.fat_tipo_iva_3;
				fat.fat_desc_iva_2 = fat.fat_desc_iva_3;

				fat.fat_cod_iva_3 = 0;
				fat.fat_ripartizione_3 = 0.0;
				fat.fat_imponibile_3 = 0.0;
				fat.fat_importo_3 = 0.0;
				fat.fat_aliquota_3 = 0.0;
				fat.fat_importo_iva_3 = 0.0;
				fat.fat_tipo_iva_3 = 0;
				fat.fat_desc_iva_3 = string.Empty;
			}
		}

		public async static Task RecalcAsync(this Fatture fat)
		{
			var app = (App)Application.Current;
			if (app.facile_db_impo == null) throw new ArgumentNullException();
			SQLiteAsyncConnection dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();

			/*
			short dec = 0;
			if (app.facile_db_impo.dit_anno >= 2002) dec = 2;
			*/

			string peso_mis = "";
			var first = true;
			var calc = true;
			var cli = new Clienti();
			var age = new Agenti();

			fat.fat_scorporo = 0;
			if (fat.fat_age == 0) calc = false;
			if (fat.fat_rel == (int)MovRel.REL_FOR) calc = false;
			if (calc)
			{
				try
				{
					if (fat.fat_age != 0) age = await dbcon.GetAsync<Agenti>(fat.fat_age);	
				}
				catch 
				{
					throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
				}

				try
				{
					if (fat.fat_inte != 0) cli = await dbcon.GetAsync<Clienti>(fat.fat_inte);
				}
				catch 
				{
					throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
				}
			}

			//
			// Leggiamo la lista delle righe
			//
			string sql = String.Format("SELECT * FROM fatrow2 WHERE rig_tipo = {0} AND rig_n_doc = {1}", fat.fat_tipo, fat.fat_n_doc);
			var rigs = await dbcon.QueryAsync<FatRow>(sql);

			//
			// Azzeriamo i totali dell' iva
			//
			fat.fat_cod_iva_0 = 0;
			fat.fat_ripartizione_0 = 0.0;
			fat.fat_imponibile_0 = 0.0;
			fat.fat_importo_0 = 0.0;
			fat.fat_aliquota_0 = 0.0;
			fat.fat_importo_iva_0 = 0.0;
			fat.fat_tipo_iva_0 = 0;
			fat.fat_dec_iva_0 = 0;
			fat.fat_aliquota_cas_0 = 0.0;
			fat.fat_importo_cas_0 = 0.0;
			fat.fat_aliquota_rit_0 = 0.0;
			fat.fat_importo_rit_0 = 0.0;
			fat.fat_desc_iva_0 = "";
			fat.des_imponibile_ivato_0 = 0.0;

			fat.fat_cod_iva_1 = 0;
			fat.fat_ripartizione_1 = 0.0;
			fat.fat_imponibile_1 = 0.0;
			fat.fat_importo_1 = 0.0;
			fat.fat_aliquota_1 = 0.0;
			fat.fat_importo_iva_1 = 0.0;
			fat.fat_tipo_iva_1 = 0;
			fat.fat_dec_iva_1 = 0;
			fat.fat_aliquota_cas_1 = 0.0;
			fat.fat_importo_cas_1 = 0.0;
			fat.fat_aliquota_rit_1 = 0.0;
			fat.fat_importo_rit_1 = 0.0;
			fat.fat_desc_iva_1 = "";
			fat.des_imponibile_ivato_1 = 0.0;

			fat.fat_cod_iva_2 = 0;
			fat.fat_ripartizione_2 = 0.0;
			fat.fat_imponibile_2 = 0.0;
			fat.fat_importo_2 = 0.0;
			fat.fat_aliquota_2 = 0.0;
			fat.fat_importo_iva_2 = 0.0;
			fat.fat_tipo_iva_2 = 0;
			fat.fat_dec_iva_2 = 0;
			fat.fat_aliquota_cas_2 = 0.0;
			fat.fat_importo_cas_2 = 0.0;
			fat.fat_aliquota_rit_2 = 0.0;
			fat.fat_importo_rit_2 = 0.0;
			fat.fat_desc_iva_2 = "";
			fat.des_imponibile_ivato_2 = 0.0;

			fat.fat_cod_iva_3 = 0;
			fat.fat_ripartizione_3 = 0.0;
			fat.fat_imponibile_3 = 0.0;
			fat.fat_importo_3 = 0.0;
			fat.fat_aliquota_3 = 0.0;
			fat.fat_importo_iva_3 = 0.0;
			fat.fat_tipo_iva_3 = 0;
			fat.fat_dec_iva_3 = 0;
			fat.fat_aliquota_cas_3 = 0.0;
			fat.fat_importo_cas_3 = 0.0;
			fat.fat_aliquota_rit_3 = 0.0;
			fat.fat_importo_rit_3 = 0.0;
			fat.fat_desc_iva_3 = "";
			fat.des_imponibile_ivato_3 = 0.0;

			fat.fat_righe = 0;
			fat.fat_tot_merce = 0.0;
			fat.fat_tot_netto = 0.0;
			fat.fat_imponibile = 0.0;
			fat.fat_tot_esente = 0.0;
			fat.fat_tot_esclusa = 0.0;
			fat.fat_tot_non_imp = 0.0;
			fat.fat_totale_imponibile = 0.0;
			fat.fat_tot_iva = 0.0;
			fat.fat_tot_fattura = 0.0;
			fat.fat_tot_pagare = 0.0;
			fat.fat_tot_prov_age = 0.0;
			fat.fat_tot_prov_cpr = 0.0;
			fat.fat_provvigione = 0.0;
			fat.fat_tot_costo = 0.0;
			fat.fat_tot_commiss = 0.0;
			fat.fat_omaggi = 0.0;
			fat.fat_tot_qta = 0.0;
			fat.fat_tot_tara = 0.0;
			fat.fat_tot_qevasa = 0.0;
			fat.fat_ritenuta_acconto = 0.0;
			fat.fat_cassa = 0.0;
			fat.fat_tot_sconto = 0;
			fat.fat_tot_conai = 0.0;
			fat.fat_tot_raee = 0.0;
			fat.fat_tot_accise = 0.0;
			fat.fat_tot_contrassegni = 0.0;
			fat.fat_dep_origine = 0;

			if (fat.fat_recalc_colli == 1) fat.fat_colli = 0;
			if (fat.fat_recalc_peso == 1) fat.fat_peso = 0.0;

			if (app.facile_db_impo.dit_anno >= 2002)
			{
				if (((fat.fat_tipo != (short)DocTipo.TIPO_DDT) && (fat.fat_calc_abbuoni == 1)) && (fat.fat_rel == (int)MovRel.REL_CLI)) fat.fat_abbuoni = 0.0;
			}

			if ((app.facile_db_impo.dit_iva_inc == 1) && !app.facile_db_impo.dit_sco_iva_esc)
			{
				if (app.facile_db_impo.dit_no_scorporo && (fat.fat_tipo != (short)DocTipo.TIPO_RIC))
				{
					for (var idx = 0; idx < rigs.Count; idx++)
					{
						if (rigs[idx].rig_iva_inclusa == 1)
						{
							var rig = rigs[idx];
							var iva = new Codiva();
				
							rig.rig_iva_inclusa = 0;

							try
							{
								iva = await dbcon.GetAsync<Codiva>(rig.rig_iva);
							}
							catch
							{
								throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
							}

							rig.rig_prezzo = Math.Round((rig.rig_prezzo / (1 + iva.iva_aliq / 100.0)), 4, MidpointRounding.AwayFromZero);
							rig.rig_prezfor = Math.Round((rig.rig_prezfor / (1 + iva.iva_aliq / 100.0)), 4, MidpointRounding.AwayFromZero);
							await rig.RecalcAsync();
							await dbcon.UpdateAsync(rig);
						}
					}
				}
			}

			foreach (var rig_doc in rigs)
			{
				var rig = rig_doc;
				if (fat.fat_dep_origine == 0) fat.fat_dep_origine = rig.rig_dep;

				fat.fat_righe += rig.rig_righe;
				fat.fat_tot_qta += rig.rig_qta;
				fat.fat_tot_tara += rig.rig_tara;
				fat.fat_tot_qevasa += rig.rig_qevasa;
				fat.fat_tot_sconto += (float)(rig.rig_tot_sconto);
				fat.fat_tot_conai += rig.rig_conai * rig.rig_qta * rig.rig_coef_mol * rig.rig_coef_mol2;
				fat.fat_tot_raee += rig.rig_raee * rig.rig_qta * rig.rig_coef_mol * rig.rig_coef_mol2;
				fat.fat_tot_accise += rig.rig_accise * rig.rig_qta * rig.rig_coef_mol * rig.rig_coef_mol2;
				fat.fat_tot_contrassegni += rig.rig_contrassegni * rig.rig_qta * rig.rig_coef_mol * rig.rig_coef_mol2;

				var old_per_prov_age = rig.rig_provvig;
				var old_tot_prov_age = rig.rig_tot_prov_age;
				var old_per_prov_cpr = rig.rig_prov_cpr;
				var old_tot_prov_cpr = rig.rig_tot_prov_cpr;

				rig.rig_tot_prov_age = 0.0;
				rig.rig_tot_prov_cpr = 0.0;
				rig.rig_tot_prov = 0.0;

				double prezzo;
				var iva = new Codiva();
				try
				{
					iva = await dbcon.GetAsync<Codiva>(rig.rig_iva);
				}
				catch
				{
					throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
				}
				if (first)
				{
					if ((!rig.rig_qta.TestIfZero(3)) || (!rig.rig_prezzo.TestIfZero(4)))
					{
						first = false;
						fat.fat_scorporo = rig.rig_iva_inclusa;
					}
				}
				if ((!rig.rig_qta.TestIfZero(3)) || (!rig.rig_prezzo.TestIfZero(4)))
				{
					if (rig.rig_iva_inclusa != fat.fat_scorporo) throw new RsaException(RsaException.MixedMsg, RsaException.MixedErr);
				}

				if (rig.rig_iva_inclusa == 1)
				{
					prezzo = 1 + iva.iva_aliq / 100.0;
					if (app.facile_db_impo.dit_anno < 2002)
						prezzo = Math.Floor(100.0 * (rig.rig_prezzo / prezzo)) / 100.0;
					else
						prezzo = Math.Round(100.0 * (rig.rig_prezzo / prezzo), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero) / 100.0;
				}
				else prezzo = rig.rig_prezzo;

				double sco1;
				double sco2;
				double sco3;
				double sco4;
				double sco5;
				double sco6;
				double sco7;
				double totale;
				if ((calc && (rig.rig_sost == 0)) && (iva.iva_omaggi == 0))
				{
					var ana = new Artanag();
					if (!string.IsNullOrWhiteSpace(rig.rig_art))
					{
						try
						{
							ana = await dbcon.GetAsync<Artanag>(rig.rig_art);
						}
						catch 
						{
							throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
						}
					}
					var max_sconto = Math.Round(prezzo - prezzo * (ana.ana_sconto_agente / 100.0), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

					sco1 = Math.Round((prezzo * (rig.rig_sconto1 / 100.0)), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					sco2 = Math.Round(((prezzo - sco1) * (rig.rig_sconto2 / 100.0)), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					sco3 = Math.Round(((prezzo - sco1 - sco2) * (rig.rig_sconto3 / 100.0)), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					sco4 = Math.Round(((prezzo - sco1 - sco2 - sco3) * (rig.rig_sconto4 / 100.0)), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					sco5 = Math.Round(((prezzo - sco1 - sco2 - sco3 - sco4) * (rig.rig_sconto5 / 100.0)), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					sco6 = Math.Round(((prezzo - sco1 - sco2 - sco3 - sco4 - sco5) * (rig.rig_sconto6 / 100.0)), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					sco7 = Math.Round(((prezzo - sco1 - sco2 - sco3 - sco4 - sco5 - sco6) * (rig.rig_sconto7 / 100.0)), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					prezzo = Math.Round(prezzo - sco1 - sco2 - sco3 - sco4 - sco5 - sco6 - sco7 + rig.rig_spese, (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					totale = Math.Round((prezzo * rig.rig_coef_mol * rig.rig_coef_mol2 * (rig.rig_qta - rig.rig_tara - rig.rig_scomerce)) - rig.rig_scovalore, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

					var sconto = Math.Round(totale * (fat.fat_sconto / 100.0), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					totale = Math.Round(totale - sconto, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

					if (max_sconto <= prezzo || fat.fat_age == 0)
					{
						if (age.age_da.Trim() == "ARTICOLO")
						{
							if ((ana.ana_scaglione_agente > 0) && (ana.ana_scaglione_agente <= 15))
							{
								switch (ana.ana_scaglione_agente)
								{
									case 1: rig.rig_provvig = (float)age.age_pro_0; break;
									case 2: rig.rig_provvig = (float)age.age_pro_1; break;
									case 3: rig.rig_provvig = (float)age.age_pro_2; break;
									case 4: rig.rig_provvig = (float)age.age_pro_3; break;
									case 5: rig.rig_provvig = (float)age.age_pro_4; break;
									case 6: rig.rig_provvig = (float)age.age_pro_5; break;
									case 7: rig.rig_provvig = (float)age.age_pro_6; break;
									case 8: rig.rig_provvig = (float)age.age_pro_7; break;
									case 9: rig.rig_provvig = (float)age.age_pro_8; break;
									case 10: rig.rig_provvig = (float)age.age_pro_9; break;
									case 11: rig.rig_provvig = (float)age.age_pro_10; break;
									case 12: rig.rig_provvig = (float)age.age_pro_11; break;
									case 13: rig.rig_provvig = (float)age.age_pro_12; break;
									case 14: rig.rig_provvig = (float)age.age_pro_13; break;
									case 15: rig.rig_provvig = (float)age.age_pro_14; break;
									default: rig.rig_provvig = 0; break;
								}
								rig.rig_tot_prov_cpr = 0.0;
								rig.rig_tot_prov_age = Math.Round(totale * (rig.rig_provvig / 100.0), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
								rig.rig_tot_prov = rig.rig_tot_prov_cpr + rig.rig_tot_prov_age;
								fat.fat_tot_prov_cpr += rig.rig_tot_prov_cpr;
								fat.fat_tot_prov_age += rig.rig_tot_prov_age;
								fat.fat_provvigione += rig.rig_tot_prov;
							}
						}
						else if (age.age_da.Trim() == "CLIENTE")
						{
							if ((cli.cli_scaglione > 0) && (cli.cli_scaglione <= 15))
							{
								switch (cli.cli_scaglione)
								{
									case 1: rig.rig_provvig = (float)age.age_pro_0; break;
									case 2: rig.rig_provvig = (float)age.age_pro_1; break;
									case 3: rig.rig_provvig = (float)age.age_pro_2; break;
									case 4: rig.rig_provvig = (float)age.age_pro_3; break;
									case 5: rig.rig_provvig = (float)age.age_pro_4; break;
									case 6: rig.rig_provvig = (float)age.age_pro_5; break;
									case 7: rig.rig_provvig = (float)age.age_pro_6; break;
									case 8: rig.rig_provvig = (float)age.age_pro_7; break;
									case 9: rig.rig_provvig = (float)age.age_pro_8; break;
									case 10: rig.rig_provvig = (float)age.age_pro_9; break;
									case 11: rig.rig_provvig = (float)age.age_pro_10; break;
									case 12: rig.rig_provvig = (float)age.age_pro_11; break;
									case 13: rig.rig_provvig = (float)age.age_pro_12; break;
									case 14: rig.rig_provvig = (float)age.age_pro_13; break;
									case 15: rig.rig_provvig = (float)age.age_pro_14; break;
									default: rig.rig_provvig = 0; break;
								}
								rig.rig_tot_prov_cpr = 0.0;
								rig.rig_tot_prov_age = Math.Round(totale * (rig.rig_provvig / 100.0), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
								rig.rig_tot_prov = rig.rig_tot_prov_cpr + rig.rig_tot_prov_age;
								fat.fat_tot_prov_cpr += rig.rig_tot_prov_cpr;
								fat.fat_tot_prov_age += rig.rig_tot_prov_age;
								fat.fat_provvigione += rig.rig_tot_prov;
							}
						}
						else if (age.age_da.Trim() == "CLI_PRO")
						{
							switch (fat.fat_tipo_ven)
							{
								case (short)DocTipoVen.VEN_TRASFERT:
									rig.rig_provvig = cli.cli_pr2;
									break;

								case (short)DocTipoVen.VEN_CSERVIZI:
									rig.rig_provvig = cli.cli_pr4;
									break;

								case (short)DocTipoVen.VEN_DELIVERY:
									rig.rig_provvig = cli.cli_pr3;
									break;
								default:
									rig.rig_provvig = cli.cli_pr1;
									break;
							}
							rig.rig_tot_prov_cpr = 0.0;
							rig.rig_tot_prov_age = Math.Round(totale * rig.rig_provvig / 100, (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
							rig.rig_tot_prov = rig.rig_tot_prov_cpr + rig.rig_tot_prov_age;

							fat.fat_tot_prov_cpr += rig.rig_tot_prov_cpr;
							fat.fat_tot_prov_age += rig.rig_tot_prov_age;
							fat.fat_provvigione += rig.rig_tot_prov;
						}
						else // Listino
						{
							var normale = true;
							if ((ana.ana_complistino == 3) && (fat.fat_tipo_ven != (short)DocTipoVen.VEN_NORMALE))
							{
								if (!ana.ana_comp_age.TestIfZero((int)DecPlaces.MAX_DECIMAL_PLACES))
								{
									normale = false;
									var mul = 1.0;
									var pesomis = rig.rig_peso_mis.Trim().ToUpper();
									if (pesomis == "") mul = 0.0;
									if (pesomis == "GG.") mul = 0.001;
									if (pesomis == "GR.") mul = 0.001;
									if (pesomis == "HG.") mul = 0.01;
									if (pesomis == "KG.") mul = 1;
									if (pesomis == "Q.") mul = 100;
									if (pesomis == "T.") mul = 1000;
									var peso = rig.rig_peso * rig.rig_qta * rig.rig_coef_mol * rig.rig_coef_mol2 * mul;

									rig.rig_tot_prov_cpr = 0.0;
									rig.rig_tot_prov_age = Math.Round(ana.ana_comp_age * peso, (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
									rig.rig_tot_prov = rig.rig_tot_prov_cpr + rig.rig_tot_prov_age;

									fat.fat_tot_prov_cpr += rig.rig_tot_prov_cpr;
									fat.fat_tot_prov_age += rig.rig_tot_prov_age;
									fat.fat_provvigione += rig.rig_tot_prov;
								}
							}
							if ((ana.ana_complistino == 4) && (fat.fat_tipo_ven != (short)DocTipoVen.VEN_NORMALE))
							{
								sql = String.Format("SELECT * FROM agganci1 WHERE agg_forn = {0} AND agg_cli = {1} AND agg_dst = {2} LIMIT 1", ana.ana_for_abituale, fat.fat_inte, fat.fat_dest);
								var aggList = await dbcon.QueryAsync<Agganci>(sql);
								if ((aggList != null) && (aggList.Count > 0))
								{
									var agg = aggList[0];
									if (!agg.agg_comp_age.TestIfZero((int)DecPlaces.MAX_DECIMAL_PLACES))
									{
										normale = false;
										var mul = 1.0;
										var pesomis = rig.rig_peso_mis.Trim().ToUpper();
										if (pesomis == "") mul = 0.0;
										if (pesomis == "GG.") mul = 0.001;
										if (pesomis == "GR.") mul = 0.001;
										if (pesomis == "HG.") mul = 0.01;
										if (pesomis == "KG.") mul = 1;
										if (pesomis == "Q.") mul = 100;
										if (pesomis == "T.") mul = 1000;
										var peso = rig.rig_peso * rig.rig_qta * rig.rig_coef_mol * rig.rig_coef_mol2 * mul;
										rig.rig_tot_prov_cpr = 0.0;
										rig.rig_tot_prov_age = Math.Round(agg.agg_comp_age * peso, (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
										rig.rig_tot_prov = rig.rig_tot_prov_cpr + rig.rig_tot_prov_age;
										fat.fat_tot_prov_cpr += rig.rig_tot_prov_cpr;
										fat.fat_tot_prov_age += rig.rig_tot_prov_age;
										fat.fat_provvigione += rig.rig_tot_prov;
									}
								}
							}
							if (normale)
							{
								if (rig.rig_provvig.TestIfZero((int)DecPlaces.TOT_DECIMAL_PLACES) && (rig.rig_art.Trim() != ""))
								{
									sql = String.Format("SELECT * FROM listini1 WHERE lis_art = {0} AND lis_codice = {1} LIMIT 1", rig.rig_art.SqlQuote(false), fat.fat_listino);
									var lisList = await dbcon.QueryAsync<Listini>(sql);
									if ((lisList != null) && (lisList.Count > 0))
									{
										var lis = lisList[0];
										switch (fat.fat_tipo_ven)
										{
											case (short)DocTipoVen.VEN_TRASFERT:
												rig.rig_provvig = lis.lis_pr2;
												rig.rig_prov_cpr = lis.lis_cpr_pr2;
												break;
											case (short)DocTipoVen.VEN_CSERVIZI:
												rig.rig_provvig = lis.lis_pr4;
												rig.rig_prov_cpr = lis.lis_cpr_pr4;
												break;

											case (short)DocTipoVen.VEN_DELIVERY:
												rig.rig_provvig = lis.lis_pr3;
												rig.rig_prov_cpr = lis.lis_cpr_pr3;
												break;
											default:
												rig.rig_provvig = lis.lis_pr1;
												rig.rig_prov_cpr = lis.lis_cpr_pr1;
												break;
										}
									}
								}
								rig.rig_tot_prov_age = Math.Round(totale * (rig.rig_provvig / 100.0), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
								rig.rig_tot_prov_cpr = Math.Round(totale * (rig.rig_prov_cpr / 100.0), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
								rig.rig_tot_prov = rig.rig_tot_prov_cpr + rig.rig_tot_prov_age;
								fat.fat_tot_prov_cpr += rig.rig_tot_prov_cpr;
								fat.fat_tot_prov_age += rig.rig_tot_prov_age;
								fat.fat_provvigione += rig.rig_tot_prov;
							}
						}
					}
				}

				// Codice Aggiunto 11/02/2008 per calcolo totali provvigioni rigo
				old_per_prov_age = old_per_prov_age - rig.rig_provvig;
				old_tot_prov_age = old_tot_prov_age - rig.rig_tot_prov_age;
				old_per_prov_cpr = old_per_prov_cpr - rig.rig_prov_cpr;
				old_tot_prov_cpr = old_tot_prov_cpr - rig.rig_tot_prov_cpr;
				if (!old_per_prov_age.TestIfZero(4) || !old_tot_prov_age.TestIfZero(4) || !old_per_prov_cpr.TestIfZero(4) || !old_tot_prov_cpr.TestIfZero(4) || fat.fat_inte != rig.rig_inte)
				{
					rig.rig_inte = fat.fat_inte;
					await dbcon.UpdateAsync(rig);
				}
				// Fine 

				sco1 = Math.Round((rig.rig_prezzo * (rig.rig_sconto1 / 100.0)), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				sco2 = Math.Round(((rig.rig_prezzo - sco1) * (rig.rig_sconto2 / 100.0)), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				sco3 = Math.Round(((rig.rig_prezzo - sco1 - sco2) * (rig.rig_sconto3 / 100.0)), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				sco4 = Math.Round(((rig.rig_prezzo - sco1 - sco2 - sco3) * (rig.rig_sconto4 / 100.0)), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				sco5 = Math.Round(((rig.rig_prezzo - sco1 - sco2 - sco3 - sco4) * (rig.rig_sconto5 / 100.0)), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				sco6 = Math.Round(((rig.rig_prezzo - sco1 - sco2 - sco3 - sco4 - sco5) * (rig.rig_sconto6 / 100.0)), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				sco7 = Math.Round(((rig.rig_prezzo - sco1 - sco2 - sco3 - sco4 - sco5 - sco6) * (rig.rig_sconto7 / 100.0)), (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				totale = Math.Round(rig.rig_prezzo - sco1 - sco2 - sco3 - sco4 - sco5 - sco6 - sco7 + rig.rig_spese, (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				if ((fat.fat_sco_iva_esc && (app.facile_db_impo.dit_iva_inc == 1)) && !app.facile_db_impo.dit_no_scorporo)
				{
					totale = Math.Round(rig.rig_prezzo - rig.rig_prezzo * (rig.rig_sco_iva_esc / 100.0) + rig.rig_spese, (int)DecPlaces.MAX_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				}


			//	totale = ((rig.rig_qta - rig.rig_tara - rig.rig_scomerce) * rig.rig_coef_mol * rig.rig_coef_mol2 * totale) - rig.rig_scovalore;

				totale = Math.Round(((rig.rig_qta - rig.rig_tara - rig.rig_scomerce) * rig.rig_coef_mol * rig.rig_coef_mol2 * totale) - rig.rig_scovalore, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

				var skip_iva = false;
				if (app.facile_db_impo.dit_anno >= 2002)
				{
					if (fat.fat_calc_abbuoni == 1 && rig.rig_sost != 0 && fat.fat_rel == (int)MovRel.REL_CLI)
					{
						if (!app.facile_db_impo.dit_nocalc_sostituzioni)
						{

							fat.fat_abbuoni = rig.rig_iva_inclusa == 0 ? Math.Round(totale * (1 + (iva.iva_aliq / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero) : totale;
						}
						else
						{
							fat.fat_abbuoni = 0.0;
							skip_iva = true;
						}
					}
				}

				var ultpracq = rig.rig_ultpracq;
				if ((fat.fat_tipo == (short)DocTipo.TIPO_DDT) && (((fat.fat_tipo_ven == (short)DocTipoVen.VEN_TRASFERT) || (fat.fat_tipo_ven == (short)DocTipoVen.VEN_CSERVIZI)) || (fat.fat_tipo_ven == (short)DocTipoVen.VEN_DELIVERY)))
				{
					ultpracq = rig.rig_prezfor;
				}
				if (((app.facile_db_impo.dit_iva_inc == 1) && app.facile_db_impo.dit_no_scorporo) || app.facile_db_impo.dit_sco_iva_esc)
				{
					ultpracq = Math.Round(ultpracq / (1 + (iva.iva_aliq / 100.0)), 4, MidpointRounding.AwayFromZero);
				}
				// Variazione Introdotta per mangano il 05/03/2005
				//fat.tot_costo   += row.rec.qta * row.rec.coef_mol * row.rec.coef_mol2 * ultpracq;
				fat.fat_tot_costo += (rig.rig_qta - rig.rig_tara) * ultpracq;
				// Fine
				fat.fat_tot_commiss += rig.rig_commiss * (rig.rig_qta - rig.rig_tara);

				if (!skip_iva)
				{
					if ((fat.fat_cod_iva_0 == rig.rig_iva) || (fat.fat_cod_iva_0 == 0))
					{
						if (rig.rig_iva == 0) throw new RsaException(RsaException.IvaZeroMsg, RsaException.IvaZeroErr);
						fat.fat_cod_iva_0 = rig.rig_iva;
						fat.fat_imponibile_0 += totale;
						if (fat.fat_scorporo == 1) fat.des_imponibile_ivato_0 += totale;
					}
					else if ((fat.fat_cod_iva_1 == rig.rig_iva) || (fat.fat_cod_iva_1 == 0))
					{
						if (rig.rig_iva == 0) throw new RsaException(RsaException.IvaZeroMsg, RsaException.IvaZeroErr);
						fat.fat_cod_iva_1 = rig.rig_iva;
						fat.fat_imponibile_1 += totale;
						if (fat.fat_scorporo == 1) fat.des_imponibile_ivato_1 += totale;
					}
					else if ((fat.fat_cod_iva_2 == rig.rig_iva) || (fat.fat_cod_iva_2 == 0))
					{
						if (rig.rig_iva == 0) throw new RsaException(RsaException.IvaZeroMsg, RsaException.IvaZeroErr);
						fat.fat_cod_iva_2 = rig.rig_iva;
						fat.fat_imponibile_2 += totale;
						if (fat.fat_scorporo == 1) fat.des_imponibile_ivato_2 += totale;
					}
					else if ((fat.fat_cod_iva_3 == rig.rig_iva) || (fat.fat_cod_iva_3 == 0))
					{
						if (rig.rig_iva == 0) throw new RsaException(RsaException.IvaZeroMsg, RsaException.IvaZeroErr);
						fat.fat_cod_iva_3 = rig.rig_iva;
						fat.fat_imponibile_3 += totale;
						if (fat.fat_scorporo == 3) fat.des_imponibile_ivato_3 += totale;
					}
					else throw new RsaException(RsaException.TroppiMsg, RsaException.TroppiErr);
				}

				if (fat.fat_credito == 0)
				{
					if (fat.fat_recalc_colli == 1) fat.fat_colli += rig.rig_colli;
					rig.rig_peso_mis = rig.rig_peso_mis.Trim();
					if (string.CompareOrdinal(peso_mis, rig.rig_peso_mis) < 0) peso_mis = rig.rig_peso_mis;
					var peso_un = 0.0;
					switch (rig.rig_peso_mis)
					{
						case "Gg.":
							peso_un = rig.rig_peso;
							break;
						case "Gr.":
							peso_un = rig.rig_peso;
							break;
						case "Hg.":
							peso_un = rig.rig_peso * 100;
							break;
						case "Kg.":
							peso_un = rig.rig_peso * 1000;
							break;
						case "Q.":
							peso_un = rig.rig_peso * 100000;
							break;
						case "T.":
							peso_un = rig.rig_peso * 1000000;
							break;
					}
					if (fat.fat_recalc_peso == 1) fat.fat_peso += rig.rig_coef_mol * rig.rig_coef_mol2 * rig.rig_qta * peso_un;
				}
			}

			// Variazione Introdotta per il calcolo delle fatture con sconti iva inclusa 24/03/2009
			if ((fat.fat_sco_iva_esc && (app.facile_db_impo.dit_iva_inc == 1)) && !app.facile_db_impo.dit_no_scorporo) fat.fat_scorporo = 0;

#if BOLLICINE
			if (!fat.fat_vuo_cauzione.TestIfZero(dec))
			{
				if ((fat.fat_cod_iva_0 == app.facile_db_impo.dit_cod_iva_ese) || (fat.fat_cod_iva_0 == 0))
				{
					if (app.facile_db_impo.ditg_cod_iva_ese == 0) throw new RsaException(RsaException.IvaZeroMsg, RsaException.IvaZeroErr);
					fat.fat_cod_iva_0 = app.facile_db_impo.dit_cod_iva_ese;
				}
				else if ((fat.fat_cod_iva_1 == app.facile_db_impo.dit_cod_iva_ese) || (fat.fat_cod_iva_1 == 0))
				{
					if (app.facile_db_impo.dit_cod_iva_ese == 0) throw new RsaException(RsaException.IvaZeroMsg, RsaException.IvaZeroErr);
					fat.fat_cod_iva_1 = app.facile_db_impo.dit_cod_iva_ese;
				}
				else if ((fat.fat_cod_iva_2 == app.facile_db_impo.dit_cod_iva_ese) || (fat.fat_cod_iva_2 == 0))
				{
					if (app.facile_db_impo.dit_cod_iva_ese == 0) throw new RsaException(RsaException.IvaZeroMsg, RsaException.IvaZeroErr);
					fat.fat_cod_iva_2 = app.facile_db_impo.dit_cod_iva_ese;
				}
				else if ((fat.fat_cod_iva_3 == app.facile_db_impo.dit_cod_iva_ese) || (fat.fat_cod_iva_3 == 0))
				{
					if (app.facile_db_impo.dit_cod_iva_ese == 0) throw new RsaException(RsaException.IvaZeroMsg, RsaException.IvaZeroErr);
					fat.fat_cod_iva_3 = App.facile_db_impo.DIT_cod_iva_ese;
				}
				else throw new RsaException(RsaException.TroppiMsg, RsaException.TroppiMsg);
			}
#endif // BOLLICINE

			//
			//  Prima aliquota IVA
			//
			do
			{
				var iva = new Codiva();

				fat.fat_aliquota_0 = 0.0;
				if (fat.fat_cod_iva_0 == 0) break;
				try
				{
					iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_0);
				}
				catch
				{
					throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
				}
				fat.fat_desc_iva_0 = iva.iva_desc;
				fat.fat_tipo_iva_0 = iva.iva_tip;

				if (iva.iva_tip == 1)
				{
					//sprintf(fat.desc_iva[i],"IVA AL %.2g%%",iva.aliq); Rimosso per il problema di Nunzio Miragliotta
					fat.fat_aliquota_0 = iva.iva_aliq;
					if (fat.fat_scorporo == 1)
					{
						if (app.facile_db_impo.dit_anno < 2002)
						{
							fat.fat_imponibile_0 = fat.fat_imponibile_0 / (1 + (iva.iva_aliq / 100.0));
							fat.fat_imponibile_0 = fat.fat_imponibile_0.MyFloor((int)DecPlaces.TOT_DECIMAL_PLACES);
						}
						else
						{
							var xxxtot = fat.fat_imponibile_0;
							fat.fat_imponibile_0 = Math.Round(fat.fat_imponibile_0 / (1 + (iva.iva_aliq / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
							var xxxiva = Math.Round(fat.fat_imponibile_0 * (1 + (iva.iva_aliq / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
							xxxiva -= xxxtot;
							if (!xxxiva.TestIfZero((int)DecPlaces.TOT_DECIMAL_PLACES))
							{
								fat.fat_dec_iva_0 = (float)xxxiva;
							}
						}
					}
				}

				// 
				// Codice Aggiunto 27/09/2007 Gestione Ritenuta Acconto e cassa previdenziale professionisti
				//
				if (!iva.iva_ritenuta.TestIfZero(2))
				{
					fat.fat_aliquota_rit_0 = iva.iva_ritenuta;
					fat.fat_perc_rit_0 = iva.iva_ritenuta_perc;
					fat.fat_importo_rit_0 += Math.Round((fat.fat_imponibile_0 * (iva.iva_ritenuta_perc / 100.0)) * (iva.iva_ritenuta / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_ritenuta_acconto += Math.Round((fat.fat_imponibile_0 * (iva.iva_ritenuta_perc / 100.0)) * (iva.iva_ritenuta / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				}

				if (!iva.iva_cassa.TestIfZero(2))
				{
					fat.fat_aliquota_cas_0 = iva.iva_cassa;
					fat.fat_importo_cas_0 += Math.Round(fat.fat_imponibile_0 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_cassa += Math.Round(fat.fat_imponibile_0 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_ripartizione_0 += Math.Round(fat.fat_imponibile_0 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				}
				// Fine Gestione Ritenuta Acconto e cassa previdenziale professionisti

				// 
				// Codice Aggiunto 16/12/2003 Gestione Omaggi 
				//
				if (iva.iva_omaggi == 1)
				{
					fat.fat_desc_iva_0 = iva.iva_desc;
					fat.fat_omaggi += Math.Round(fat.fat_imponibile_0 - fat.fat_imponibile_0 * fat.fat_sconto / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				}
				// Fine Codice aggiunto
			} while (false);

			//
			//  Seconda aliquota IVA
			//
			do
			{
				var iva = new Codiva();

				fat.fat_aliquota_1 = 0.0;
				if (fat.fat_cod_iva_1 == 0) break;
				try
				{
					iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_1);
				}
				catch
				{
					throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
				}
				fat.fat_desc_iva_1 = iva.iva_desc;
				fat.fat_tipo_iva_1 = iva.iva_tip;

				if (iva.iva_tip == 1)
				{
					//sprintf(fat.desc_iva[i],"IVA AL %.2g%%",iva.aliq); Rimosso per il problema di Nunzio Miragliotta
					fat.fat_aliquota_1 = iva.iva_aliq;
					if (fat.fat_scorporo == 1)
					{
						if (app.facile_db_impo.dit_anno < 2002)
						{
							fat.fat_imponibile_1 = fat.fat_imponibile_1 / (1 + (iva.iva_aliq / 100.0));
							fat.fat_imponibile_1 = fat.fat_imponibile_1.MyFloor((int)DecPlaces.TOT_DECIMAL_PLACES);
						}
						else
						{
							var xxxtot = fat.fat_imponibile_1;
							fat.fat_imponibile_1 = Math.Round(fat.fat_imponibile_1 / (1 + (iva.iva_aliq / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
							var xxxiva = Math.Round(fat.fat_imponibile_1 * (1 + (iva.iva_aliq / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
							xxxiva -= xxxtot;
							if (!xxxiva.TestIfZero((int)DecPlaces.TOT_DECIMAL_PLACES))
							{
								fat.fat_dec_iva_1 = (float)xxxiva;
							}
						}
					}
				}

				// 
				// Codice Aggiunto 27/09/2007 Gestione Ritenuta Acconto e cassa previdenziale professionisti
				//
				if (!iva.iva_ritenuta.TestIfZero(2))
				{
					fat.fat_aliquota_rit_1 = iva.iva_ritenuta;
					fat.fat_perc_rit_1 = iva.iva_ritenuta_perc;
					fat.fat_importo_rit_1 += Math.Round((fat.fat_imponibile_1 * (iva.iva_ritenuta_perc / 100.0)) * (iva.iva_ritenuta / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_ritenuta_acconto += Math.Round((fat.fat_imponibile_1 * (iva.iva_ritenuta_perc / 100.0)) * (iva.iva_ritenuta / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				}

				if (!iva.iva_cassa.TestIfZero(2))
				{
					fat.fat_aliquota_cas_1 = iva.iva_cassa;
					fat.fat_importo_cas_1 += Math.Round(fat.fat_imponibile_1 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_cassa += Math.Round(fat.fat_imponibile_1 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_ripartizione_1 += Math.Round(fat.fat_imponibile_1 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				}
				// Fine Gestione Ritenuta Acconto e cassa previdenziale professionisti

				// 
				// Codice Aggiunto 16/12/2003 Gestione Omaggi 
				//
				if (iva.iva_omaggi == 1)
				{
					fat.fat_desc_iva_1 = iva.iva_desc;
					fat.fat_omaggi += Math.Round(fat.fat_imponibile_1 - fat.fat_imponibile_1 * fat.fat_sconto / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				}
				// Fine Codice aggiunto
			} while (false);

			//
			//  Terza aliquota IVA
			//
			do
			{
				var iva = new Codiva();
				fat.fat_aliquota_2 = 0.0;
				if (fat.fat_cod_iva_2 == 0) break;

				try
				{
					iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_2);
				}
				catch
				{
					throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
				}

				fat.fat_desc_iva_2 = iva.iva_desc;
				fat.fat_tipo_iva_2 = iva.iva_tip;

				if (iva.iva_tip == 1)
				{
					//sprintf(fat.desc_iva[i],"IVA AL %.2g%%",iva.aliq); Rimosso per il problema di Nunzio Miragliotta
					fat.fat_aliquota_2 = iva.iva_aliq;
					if (fat.fat_scorporo == 1)
					{
						if (app.facile_db_impo.dit_anno < 2002)
						{
							fat.fat_imponibile_2 = fat.fat_imponibile_2 / (1 + (iva.iva_aliq / 100.0));
							fat.fat_imponibile_2 = fat.fat_imponibile_2.MyFloor((int)DecPlaces.TOT_DECIMAL_PLACES);
						}
						else
						{
							var xxxtot = fat.fat_imponibile_2;
							fat.fat_imponibile_2 = Math.Round(fat.fat_imponibile_2 / (1 + (iva.iva_aliq / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
							var xxxiva = Math.Round(fat.fat_imponibile_2 * (1 + (iva.iva_aliq / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
							xxxiva -= xxxtot;
							if (!xxxiva.TestIfZero((int)DecPlaces.TOT_DECIMAL_PLACES))
							{
								fat.fat_dec_iva_2 = (float)xxxiva;
							}
						}
					}
				}

				// 
				// Codice Aggiunto 27/09/2007 Gestione Ritenuta Acconto e cassa previdenziale professionisti
				//
				if (!iva.iva_ritenuta.TestIfZero(2))
				{
					fat.fat_aliquota_rit_2 = iva.iva_ritenuta;
					fat.fat_perc_rit_2 = iva.iva_ritenuta_perc;
					fat.fat_importo_rit_2 += Math.Round((fat.fat_imponibile_2 * (iva.iva_ritenuta_perc / 100.0)) * (iva.iva_ritenuta / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_ritenuta_acconto += Math.Round((fat.fat_imponibile_2 * (iva.iva_ritenuta_perc / 100.0)) * (iva.iva_ritenuta / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				}

				if (!iva.iva_cassa.TestIfZero(2))
				{
					fat.fat_aliquota_cas_2 = iva.iva_cassa;
					fat.fat_importo_cas_2 += Math.Round(fat.fat_imponibile_2 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_cassa += Math.Round(fat.fat_imponibile_2 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_ripartizione_2 += Math.Round(fat.fat_imponibile_2 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				}
				// Fine Gestione Ritenuta Acconto e cassa previdenziale professionisti

				// 
				// Codice Aggiunto 16/12/2003 Gestione Omaggi 
				//
				if (iva.iva_omaggi == 1)
				{
					fat.fat_desc_iva_2 = iva.iva_desc;
					fat.fat_omaggi += Math.Round(fat.fat_imponibile_2 - fat.fat_imponibile_2 * fat.fat_sconto / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				}
				// Fine Codice aggiunto
			} while (false);

			//
			//  Quarta aliquota IVA
			//
			do
			{
				var iva = new Codiva();

				fat.fat_aliquota_3 = 0.0;
				if (fat.fat_cod_iva_3 == 0) break;
				try
				{
					iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_3);
				}
				catch
				{
					throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
				}
				fat.fat_desc_iva_3 = iva.iva_desc;
				fat.fat_tipo_iva_3 = iva.iva_tip;

				if (iva.iva_tip == 1)
				{
					//sprintf(fat.desc_iva[i],"IVA AL %.2g%%",iva.aliq); Rimosso per il problema di Nunzio Miragliotta
					fat.fat_aliquota_3 = iva.iva_aliq;
					if (fat.fat_scorporo == 1)
					{
						if (app.facile_db_impo.dit_anno < 2002)
						{
							fat.fat_imponibile_3 = fat.fat_imponibile_3 / (1 + (iva.iva_aliq / 100.0));
							fat.fat_imponibile_3 = fat.fat_imponibile_3.MyFloor((int)DecPlaces.TOT_DECIMAL_PLACES);
						}
						else
						{
							var xxxtot = fat.fat_imponibile_3;
							fat.fat_imponibile_3 = Math.Round(fat.fat_imponibile_3 / (1 + (iva.iva_aliq / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
							var xxxiva = Math.Round(fat.fat_imponibile_3 * (1 + (iva.iva_aliq / 100.0)), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
							xxxiva -= xxxtot;
							if (!xxxiva.TestIfZero((int)DecPlaces.TOT_DECIMAL_PLACES))
							{
								fat.fat_dec_iva_3 = (float)xxxiva;
							}
						}
					}
				}

				// 
				// Codice Aggiunto 27/09/2007 Gestione Ritenuta Acconto e cassa previdenziale professionisti
				//
				if (!iva.iva_ritenuta.TestIfZero(2))
				{
					fat.fat_aliquota_rit_3 = iva.iva_ritenuta;
					fat.fat_perc_rit_3 = iva.iva_ritenuta_perc;
					fat.fat_importo_rit_3 += Math.Round((fat.fat_imponibile_3 * (iva.iva_ritenuta_perc / 100.0)) * (iva.iva_ritenuta / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_ritenuta_acconto += Math.Round((fat.fat_imponibile_3 * (iva.iva_ritenuta_perc / 100.0)) * (iva.iva_ritenuta / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				}

				if (!iva.iva_cassa.TestIfZero(2))
				{
					fat.fat_aliquota_cas_3 = iva.iva_cassa;
					fat.fat_importo_cas_3 += Math.Round(fat.fat_imponibile_3 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_cassa += Math.Round(fat.fat_imponibile_3 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
					fat.fat_ripartizione_3 += Math.Round(fat.fat_imponibile_3 * (iva.iva_cassa / 100.0), (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				}
				// Fine Gestione Ritenuta Acconto e cassa previdenziale professionisti

				// 
				// Codice Aggiunto 16/12/2003 Gestione Omaggi 
				//
				if (iva.iva_omaggi == 1)
				{
					fat.fat_desc_iva_3 = iva.iva_desc;
					fat.fat_omaggi += Math.Round(fat.fat_imponibile_3 - fat.fat_imponibile_3 * fat.fat_sconto / 100.0, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
				}
				// Fine Codice aggiunto
			} while (false);

			if (fat.fat_credito == 0)
			{
				if (fat.fat_recalc_peso == 1)
				{
					if (peso_mis == "" || peso_mis == "Gg." || peso_mis == "Gr.")
						fat.fat_peso_mis = "Gr.";
					else if (peso_mis == "Hg.")
					{
						fat.fat_peso_mis = "Hg.";
						fat.fat_peso = fat.fat_peso / 100;
					}
					else if (peso_mis == "Kg.")
					{
						fat.fat_peso_mis = "Kg.";
						fat.fat_peso = fat.fat_peso / 1000;
					}
					else if (peso_mis == "Q.")
					{
						fat.fat_peso_mis = "Q.";
						fat.fat_peso = fat.fat_peso / 100000;
					}
					else
					{
						fat.fat_peso_mis = "T.";
						fat.fat_peso = fat.fat_peso / 1000000;
					}
				}
			}
			fat.fat_provvigione = Math.Round(fat.fat_provvigione, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
			fat.fat_tot_costo = Math.Round(fat.fat_tot_costo, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
			fat.fat_tot_commiss = Math.Round(fat.fat_tot_commiss, (int)DecPlaces.TOT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);

			await GetTotaliAsync(fat);
		}
	}
}

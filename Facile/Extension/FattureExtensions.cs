using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Facile.Interfaces;
using Facile.Models;
using Facile.Utils;
using SQLite;
using Xamarin.Forms;

namespace Facile.Extension
{
	public static class FattureExtensions
	{
		private static readonly double LIMITE_BOLLO = 77.47;

		public async static Task GetTotali(this Fatture fat)
		{
			int i;
			double [] rip_cassa = new double[4];
			var app = (App)Application.Current;
			SQLiteAsyncConnection dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();
			if (app.facile_db_impo == null) throw new ArgumentNullException();

			short dec = 2;
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
						if (fat.fat_importo_cas_0.TestIfZero(dec) != true)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_0);
								rip_cassa[i] = Math.Round(fat.fat_imponibile_0 * (iva.iva_cassa / 100.0), dec, MidpointRounding.AwayFromZero);
							}
							catch 
							{
								throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
							}
						}
						break;

					case 1:
						fat.fat_ripartizione_1 = 0.0;
						if (fat.fat_importo_cas_1.TestIfZero(dec) != true)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_1);
								rip_cassa[i] = Math.Round(fat.fat_imponibile_1 * (iva.iva_cassa / 100.0), dec, MidpointRounding.AwayFromZero);
							}
							catch (System.Exception)
							{
								throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
							}
						}
						break;

					case 2:
						fat.fat_ripartizione_2 = 0.0;
						if (fat.fat_importo_cas_2.TestIfZero(dec) != true)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_2);
								rip_cassa[i] = Math.Round(fat.fat_imponibile_2 * (iva.iva_cassa / 100.0), dec, MidpointRounding.AwayFromZero);
							}
							catch (System.Exception)
							{
								throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
							}
						}
						break;

					case 3:
						fat.fat_ripartizione_3 = 0.0;
						if (fat.fat_importo_cas_3.TestIfZero(dec) != true)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_3);
								rip_cassa[i] = Math.Round(fat.fat_imponibile_3 * (iva.iva_cassa / 100.0), dec, MidpointRounding.AwayFromZero);
							}
							catch (System.Exception)
							{
								throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
							}
						}
						break;
				}
			}

			fat.fat_tot_conai = Math.Round(fat.fat_tot_conai, dec, MidpointRounding.AwayFromZero);
			fat.fat_tot_raee = Math.Round(fat.fat_tot_raee, dec, MidpointRounding.AwayFromZero);
			fat.fat_tot_accise = Math.Round(fat.fat_tot_accise, dec, MidpointRounding.AwayFromZero);
			fat.fat_tot_contrassegni = Math.Round(fat.fat_tot_contrassegni, dec, MidpointRounding.AwayFromZero);


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
						fat.fat_imballo = Math.Round(importo_merci * (perc / 100.0), dec,MidpointRounding.AwayFromZero);
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

				fat.fat_tot_merce += fat.fat_imponibile_0.MyFloor(dec);

				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_0 = Math.Round(fat.des_imponibile_ivato_0 - fat.des_imponibile_ivato_0 * fat.fat_sconto / 100.0, dec, MidpointRounding.AwayFromZero);
					double xxxtot = fat.fat_importo_0;
					fat.fat_importo_0 = Math.Round(fat.fat_importo_0 / (1 + (fat.fat_aliquota_0 / 100.0)), dec, MidpointRounding.AwayFromZero);
					double xxxiva = Math.Round(fat.fat_importo_0 * (1 + (fat.fat_aliquota_0 / 100.0)), dec, MidpointRounding.AwayFromZero);
					xxxiva -= xxxtot;
					xxxiva = Math.Round(xxxiva, dec, MidpointRounding.AwayFromZero);
					if (xxxiva.TestIfZero(dec) != true)
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

				fat.fat_importo_iva_0 = Math.Round((fat.fat_importo_0 + fat.fat_ripartizione_0) * fat.fat_aliquota_0 / 100.0, dec, MidpointRounding.AwayFromZero);

				//
				// Codice aggiunto per adeguamento imponibile ed iva con terza cifra decimale = 5 (Es. 11.61/1.20) 
				//
				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_iva_0 -= fat.fat_dec_iva_0;

					double val = fat.fat_importo_iva_0 + fat.fat_importo_0;
					double target = Math.Round(fat.des_imponibile_ivato_0 - fat.des_imponibile_ivato_0 * fat.fat_sconto / 100.0, dec, MidpointRounding.AwayFromZero);
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
				fat.fat_importo_0 = Math.Round(fat.fat_importo_0, dec, MidpointRounding.AwayFromZero);

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

					fat.fat_tot_merce += Math.Round(fat.fat_imponibile_0 * fat.fat_aliquota_0 / 100.0, dec, MidpointRounding.AwayFromZero);
					fat.fat_tot_netto += Math.Round(fat.fat_importo_0 * fat.fat_aliquota_0 / 100.0, dec, MidpointRounding.AwayFromZero);

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

				fat.fat_tot_merce += fat.fat_imponibile_1.MyFloor(dec);

				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_1 = Math.Round(fat.des_imponibile_ivato_1 - fat.des_imponibile_ivato_1 * fat.fat_sconto / 100.0, dec, MidpointRounding.AwayFromZero);
					double xxxtot = fat.fat_importo_1;
					fat.fat_importo_1 = Math.Round(fat.fat_importo_1 / (1 + (fat.fat_aliquota_1 / 100.0)), dec, MidpointRounding.AwayFromZero);
					double xxxiva = Math.Round(fat.fat_importo_1 * (1 + (fat.fat_aliquota_1 / 100.0)), dec, MidpointRounding.AwayFromZero);
					xxxiva -= xxxtot;
					xxxiva = Math.Round(xxxiva, dec, MidpointRounding.AwayFromZero);
					if (xxxiva.TestIfZero(dec) != true)
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
				fat.fat_importo_iva_1 = Math.Round((fat.fat_importo_1 + fat.fat_ripartizione_1) * fat.fat_aliquota_1 / 100.0, dec, MidpointRounding.AwayFromZero);

				//
				// Codice aggiunto per adeguamento imponibile ed iva con terza cifra decimale = 5 (Es. 11.61/1.20) 
				//
				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_iva_1 -= fat.fat_dec_iva_1;

					double val = fat.fat_importo_iva_1 + fat.fat_importo_1;
					double target = Math.Round(fat.des_imponibile_ivato_1 - fat.des_imponibile_ivato_1 * fat.fat_sconto / 100.0, dec, MidpointRounding.AwayFromZero);
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
				fat.fat_importo_1 = Math.Round(fat.fat_importo_1, dec, MidpointRounding.AwayFromZero);

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

					fat.fat_tot_merce += Math.Round(fat.fat_imponibile_1 * fat.fat_aliquota_1 / 100.0, dec, MidpointRounding.AwayFromZero);
					fat.fat_tot_netto += Math.Round(fat.fat_importo_1 * fat.fat_aliquota_1 / 100.0, dec, MidpointRounding.AwayFromZero);

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

				fat.fat_tot_merce += fat.fat_imponibile_2.MyFloor(dec);

				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_2 = Math.Round(fat.des_imponibile_ivato_2 - fat.des_imponibile_ivato_2 * fat.fat_sconto / 100.0, dec, MidpointRounding.AwayFromZero);
					double xxxtot = fat.fat_importo_2;
					fat.fat_importo_2 = Math.Round(fat.fat_importo_2 / (1 + (fat.fat_aliquota_2 / 100.0)), dec, MidpointRounding.AwayFromZero);
					double xxxiva = Math.Round(fat.fat_importo_2 * (1 + (fat.fat_aliquota_2 / 100.0)), dec, MidpointRounding.AwayFromZero);
					xxxiva -= xxxtot;
					xxxiva = Math.Round(xxxiva, dec, MidpointRounding.AwayFromZero);
					if (xxxiva.TestIfZero(dec) != true)
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
				fat.fat_importo_iva_2 = Math.Round((fat.fat_importo_2 + fat.fat_ripartizione_2) * fat.fat_aliquota_2 / 100.0, dec, MidpointRounding.AwayFromZero);

				//
				// Codice aggiunto per adeguamento imponibile ed iva con terza cifra decimale = 5 (Es. 11.61/1.20) 
				//
				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_iva_2 -= fat.fat_dec_iva_2;

					double val = fat.fat_importo_iva_2 + fat.fat_importo_2;
					double target = Math.Round(fat.des_imponibile_ivato_2 - fat.des_imponibile_ivato_2 * fat.fat_sconto / 100.0, dec, MidpointRounding.AwayFromZero);
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
				fat.fat_importo_2 = Math.Round(fat.fat_importo_2, dec, MidpointRounding.AwayFromZero);

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

					fat.fat_tot_merce += Math.Round(fat.fat_imponibile_2 * fat.fat_aliquota_2 / 100.0, dec, MidpointRounding.AwayFromZero);
					fat.fat_tot_netto += Math.Round(fat.fat_importo_2 * fat.fat_aliquota_2 / 100.0, dec, MidpointRounding.AwayFromZero);

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

				fat.fat_tot_merce += fat.fat_imponibile_3.MyFloor(dec);

				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_3 = Math.Round(fat.des_imponibile_ivato_3 - fat.des_imponibile_ivato_3 * fat.fat_sconto / 100.0, dec, MidpointRounding.AwayFromZero);
					double xxxtot = fat.fat_importo_3;
					fat.fat_importo_3 = Math.Round(fat.fat_importo_3 / (1 + (fat.fat_aliquota_3 / 100.0)), dec, MidpointRounding.AwayFromZero);
					double xxxiva = Math.Round(fat.fat_importo_3 * (1 + (fat.fat_aliquota_3 / 100.0)), dec, MidpointRounding.AwayFromZero);
					xxxiva -= xxxtot;
					xxxiva = Math.Round(xxxiva, dec, MidpointRounding.AwayFromZero);
					if (xxxiva.TestIfZero(dec) != true)
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
				fat.fat_importo_iva_3 = Math.Round((fat.fat_importo_3 + fat.fat_ripartizione_3) * fat.fat_aliquota_3 / 100.0, dec, MidpointRounding.AwayFromZero);

				//
				// Codice aggiunto per adeguamento imponibile ed iva con terza cifra decimale = 5 (Es. 11.61/1.20) 
				//
				if (fat.fat_scorporo != 0)
				{
					fat.fat_importo_iva_3 -= fat.fat_dec_iva_3;

					double val = fat.fat_importo_iva_3 + fat.fat_importo_3;
					double target = Math.Round(fat.des_imponibile_ivato_3 - fat.des_imponibile_ivato_3 * fat.fat_sconto / 100.0, dec, MidpointRounding.AwayFromZero);
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
				fat.fat_importo_3 = Math.Round(fat.fat_importo_3, dec, MidpointRounding.AwayFromZero);

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

					fat.fat_tot_merce += Math.Round(fat.fat_imponibile_3 * fat.fat_aliquota_3 / 100.0, dec, MidpointRounding.AwayFromZero);
					fat.fat_tot_netto += Math.Round(fat.fat_importo_3 * fat.fat_aliquota_3 / 100.0, dec, MidpointRounding.AwayFromZero);

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
				if (val.TestIfZero(dec) != true) fat.fat_bolli_eff += app.facile_db_impo.dit_bolli;
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
							totval = Math.Round(totval * (app.facile_db_impo.dit_trat_perc_bolli / 100.0), dec, MidpointRounding.AwayFromZero);
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




	}
}

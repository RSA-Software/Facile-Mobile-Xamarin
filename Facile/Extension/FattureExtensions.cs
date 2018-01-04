using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Xamarin.Forms;

namespace Facile.Extension
{
	public static class FattureExtensions
	{
		public const double LIMITE_BOLLO = 77.47;

		public enum TipoDocumento
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
		}

		public enum ErroreDocumento
		{
			NoError = 0,
			IvaNotFound = 1,
			DittaNotFound = 2,
			TroppeAliquote = 3,
			IvaSpeseNotFound = 4,
			PagamentoNotFound = 5,
			IvaAgenteNotFound = 6,
		}


		public async static Task<ErroreDocumento> GetTotali(this Fatture fat)
		{
			int i;
			double [] rip_cassa = new double[4];
			List<Ditte> ditte = null;
			SQLiteAsyncConnection dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();

			try
			{
				ditte = await dbcon.QueryAsync<Ditte>("SELECT * FROM impostazioni ORDER BY impo_id DESC LIMIT 1");
			}
			catch
			{
				return (ErroreDocumento.DittaNotFound);
			}

			var dec = 2;
			fat.fat_tot_merce = 0.0;
			fat.fat_tot_netto = 0.0;
			fat.fat_tot_imponibile = 0.0;
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
							catch (System.Exception)
							{
								return (ErroreDocumento.IvaNotFound);
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
								return (ErroreDocumento.IvaNotFound);
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
								return (ErroreDocumento.IvaNotFound);
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
								return (ErroreDocumento.IvaNotFound);
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

			if (((fat.fat_tipo == (int)TipoDocumento.TIPO_FAT) || (fat.fat_tipo == (int)TipoDocumento.TIPO_ACC)) || (fat.fat_tipo == (int)TipoDocumento.TIPO_RIC))
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
									spetra_imp = ditte[0].impo_spetra_imp_0;
									spetra_per = ditte[0].impo_spetra_per_0;
									break;

								case  1: 
									spetra_imp = ditte[0].impo_spetra_imp_1;
									spetra_per = ditte[0].impo_spetra_per_1;
									break;

								case  2: 
									spetra_imp = ditte[0].impo_spetra_imp_2;
									spetra_per = ditte[0].impo_spetra_per_2;
									break;

								case  3: 
									spetra_imp = ditte[0].impo_spetra_imp_3;
									spetra_per = ditte[0].impo_spetra_per_3;
									break;

								default: 
									spetra_imp = ditte[0].impo_spetra_imp_4;
									spetra_per = ditte[0].impo_spetra_per_4;
									break;
							}
							if (importo_merci <= spetra_imp)
							{
								perc = spetra_per;
								break;
							}
						}
						fat.fat_imballo = Math.Round(importo_merci * (perc / 100.0), dec,MidpointRounding.AwayFromZero);
						fat.fat_imballo += ditte[0].impo_spetra;
					}

					if ((fat.fat_spese != 0) && (fat.fat_recalc_varie != 0))
					{
						fat.fat_varie = ditte[0].impo_fat_spese;
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
							return (ErroreDocumento.PagamentoNotFound);
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
			if (ditte[0].impo_iva_spese == 0L)
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
				var aliquota_max = fat.fat_aliquota_iva_0;
				for (i = 1; i < 4; i++)
				{
					double aliquota;
					double imponibile;

					switch(i)
					{
						case 1:
							aliquota = fat.fat_aliquota_iva_1;
							imponibile = fat.fat_imponibile_1;
							break;

						case 2:
							aliquota = fat.fat_aliquota_iva_2;
							imponibile = fat.fat_imponibile_2;
							break;

						default:
							aliquota = fat.fat_aliquota_iva_3;
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
					if (fat.fat_cod_iva_0 == ditte[0].impo_iva_spese)
					{
						fat.fat_ripartizione_0 = fat.fat_tot_non_docum;
						found = true;
						break;
					}
					if (fat.fat_cod_iva_1 == ditte[0].impo_iva_spese)
					{
						fat.fat_ripartizione_1 = fat.fat_tot_non_docum;
						found = true;
						break;
					}
					if (fat.fat_cod_iva_2 == ditte[0].impo_iva_spese)
					{
						fat.fat_ripartizione_2 = fat.fat_tot_non_docum;
						found = true;
						break;
					}
					if (fat.fat_cod_iva_3 == ditte[0].impo_iva_spese)
					{
						fat.fat_ripartizione_3 = fat.fat_tot_non_docum;
						found = true;
						break;
					}
				} while (false);

				if (found == false)
				{
					do
					{
						if (fat.fat_cod_iva_0 == 0)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(ditte[0].impo_iva_spese);
								fat.fat_cod_iva_0 = ditte[0].impo_iva_spese;
								fat.fat_desc_iva_0 =  iva.iva_desc;
								fat.fat_tipo_iva_0 = iva.iva_tip;
								if (iva.iva_tip == 1) fat.fat_aliquota_iva_0 = iva.iva_aliq;
								fat.fat_ripartizione_0 = fat.fat_tot_non_docum;
								found = true;
								break;

							}
							catch (System.Exception)
							{
								return (ErroreDocumento.IvaSpeseNotFound);
							}
						}

						if (fat.fat_cod_iva_1 == 0)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(ditte[0].impo_iva_spese);
								fat.fat_cod_iva_1 = ditte[0].impo_iva_spese;
								fat.fat_desc_iva_1 = iva.iva_desc;
								fat.fat_tipo_iva_1 = iva.iva_tip;
								if (iva.iva_tip == 1) fat.fat_aliquota_iva_1 = iva.iva_aliq;
								fat.fat_ripartizione_1 = fat.fat_tot_non_docum;
								found = true;
								break;

							}
							catch (System.Exception)
							{
								return (ErroreDocumento.IvaSpeseNotFound);
							}
						}

						if (fat.fat_cod_iva_2 == 0)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(ditte[0].impo_iva_spese);
								fat.fat_cod_iva_2 = ditte[0].impo_iva_spese;
								fat.fat_desc_iva_2 = iva.iva_desc;
								fat.fat_tipo_iva_2 = iva.iva_tip;
								if (iva.iva_tip == 1) fat.fat_aliquota_iva_2 = iva.iva_aliq;
								fat.fat_ripartizione_2 = fat.fat_tot_non_docum;
								found = true;
								break;

							}
							catch (System.Exception)
							{
								return (ErroreDocumento.IvaSpeseNotFound);
							}
						}

						if (fat.fat_cod_iva_3 == 0)
						{
							try
							{
								var iva = await dbcon.GetAsync<Codiva>(ditte[0].impo_iva_spese);
								fat.fat_cod_iva_3 = ditte[0].impo_iva_spese;
								fat.fat_desc_iva_3 = iva.iva_desc;
								fat.fat_tipo_iva_3 = iva.iva_tip;
								if (iva.iva_tip == 1) fat.fat_aliquota_iva_3 = iva.iva_aliq;
								fat.fat_ripartizione_3 = fat.fat_tot_non_docum;
								found = true;
								break;
							}
							catch (System.Exception)
							{
								return (ErroreDocumento.IvaSpeseNotFound);
							}
						}
						return (ErroreDocumento.TroppeAliquote);
					} while (false);
				}
			}
			/*
			
			// Calcoliamo i totali dell' iva 
			for (i = 0; i < 4; i++)
			{
				fat->rec.importo[i] = 0.0;
				if (fat->rec.cod_iva[i] == 0L) continue;

				fat->rec.ripartizione[i] += rip_cassa[i];

				if (facile_db_impo.anno < 2002)
					fat->rec.tot_merce += floor(fat->rec.imponibile[i]);
				else
					fat->rec.tot_merce += MyFloor(fat->rec.imponibile[i], dec);
				//	fat->rec.tot_merce     += fat->rec.imponibile[i]; 

				if (fat->rec.scorporo == TRUE)
				{
					fat->rec.importo[i] = Round(fat->des.imponibile_ivato[i] - fat->des.imponibile_ivato[i] * fat->rec.sconto / 100.0, dec);

					if (facile_db_impo.anno < 2002)
						fat->rec.importo[i] = MyFloor(fat->rec.importo[i] / (1 + (fat->rec.aliquota_iva[i] / 100.0)), dec);
					else
					{
						double xxxiva;

						const auto xxxtot = fat->rec.importo[i];
						fat->rec.importo[i] = Round(fat->rec.importo[i] / (1 + (fat->rec.aliquota_iva[i] / 100.0)), dec);
						xxxiva = Round(fat->rec.importo[i] * (1 + (fat->rec.aliquota_iva[i] / 100.0)), dec);
						xxxiva -= xxxtot;
						xxxiva = Round(xxxiva, dec);
						if (TestIfZero(&xxxiva, dec) == FALSE)
						{
							fat->rec.dec_iva[i] = static_cast<float>(xxxiva);
						}
					}
				}
				else
				{
					fat->rec.importo[i] = fat->rec.imponibile[i] - fat->rec.imponibile[i] * fat->rec.sconto / 100.0;
				}

	# ifdef BOLLICINE
				if ((TestIfZero(&fat->rec.vuo_cauzione, dec) == FALSE) && (fat->rec.cod_iva[i] == facile_db_impo.cod_iva_ese))
				{
					fat->rec.imponibile[i] += fat->rec.vuo_cauzione;
					fat->des.imponibile_ivato[i] += fat->rec.vuo_cauzione;
					fat->rec.importo[i] += fat->rec.vuo_cauzione;
				}
	#endif

				if (facile_db_impo.anno < 2002)
					fat->rec.importo_iva[i] = MyCeil((fat->rec.importo[i] + fat->rec.ripartizione[i]) * fat->rec.aliquota_iva[i] / 100.0, dec);
				else
					fat->rec.importo_iva[i] = Round((fat->rec.importo[i] + fat->rec.ripartizione[i]) * fat->rec.aliquota_iva[i] / 100.0, dec);

				//
				// Codice aggiunto per adeguamento imponibile ed iva con terza cifra decimale = 5 (Es. 11.61/1.20) 
				//
				if ((facile_db_impo.anno >= 2002) && (fat->rec.scorporo == TRUE))
				{
					fat->rec.importo_iva[i] -= fat->rec.dec_iva[i];

					auto val = fat->rec.importo_iva[i] + fat->rec.importo[i];
					const auto target = Round(fat->des.imponibile_ivato[i] - fat->des.imponibile_ivato[i] * fat->rec.sconto / 100.0, dec);
					val = Round(val - target, 2);
					if (val <= 0.0100000000000000000)
					{
						fat->rec.importo_iva[i] -= val;
						fat->rec.dec_iva[i] -= static_cast<float>(val);
					}
				}
				// Fine Codice Aggiunto

				//******************************************************************
				// Modificato Arrotondamento il 27/03/2001                        
				//fat->rec.importo[i]     = MyFloor(fat->rec.importo[i],impo.dec);
				fat->rec.importo[i] = Round(fat->rec.importo[i], dec);

				switch (fat->rec.tipo_iva[i])
				{
					case 3:
					case 6:
						fat->rec.tot_non_imp += fat->rec.importo[i] + fat->rec.ripartizione[i];
						fat->rec.totale_imponibile += fat->rec.importo[i] + fat->rec.ripartizione[i];
						break;

					case 4:
						fat->rec.tot_esclusa += fat->rec.importo[i] + fat->rec.ripartizione[i];
						fat->rec.totale_imponibile += fat->rec.importo[i] + fat->rec.ripartizione[i];
						break;

					case 2:
						fat->rec.tot_esente += fat->rec.importo[i] + fat->rec.ripartizione[i];
						fat->rec.totale_imponibile += fat->rec.importo[i] + fat->rec.ripartizione[i];
						break;

					default:
						fat->rec.tot_imponibile += fat->rec.importo[i] + fat->rec.ripartizione[i];
						fat->rec.totale_imponibile += fat->rec.importo[i] + fat->rec.ripartizione[i];
						break;
				}
				fat->rec.tot_netto += fat->rec.importo[i];
				fat->rec.tot_iva += fat->rec.importo_iva[i];

				// Aggiunto il 27/03/2001 
				fat->rec.tot_fattura += fat->rec.importo[i];
				if (fat->rec.scorporo == TRUE)
				{
					if (facile_db_impo.anno < 2002)
					{
						fat->rec.tot_merce += MyCeil(fat->rec.imponibile[i] * fat->rec.aliquota_iva[i] / 100.0, dec);
						fat->rec.tot_netto += MyCeil(fat->rec.importo[i] * fat->rec.aliquota_iva[i] / 100.0, dec);
					}
					else
					{
						double diff;

						fat->rec.tot_merce += Round(fat->rec.imponibile[i] * fat->rec.aliquota_iva[i] / 100.0, dec);
						fat->rec.tot_netto += Round(fat->rec.importo[i] * fat->rec.aliquota_iva[i] / 100.0, dec);

						// Codice Aggiunto il 31/03/2004 Gestione importi a scorporo con terzo dec. = 5
						diff = fat->rec.tot_merce - fat->rec.tot_netto;
						fat->rec.tot_merce -= fat->rec.dec_iva[i];
						if (TestIfZero(&diff, 2) == TRUE) fat->rec.tot_netto -= fat->rec.dec_iva[i];
						// Fine 
					}
				}
				// Fine Codice Aggiunto   
			}
			*/

			// Codice Aggiunto il 25/05/2006 per evitare che il totale merci sia inferiore al totale netto
			if ((fat.fat_tot_merce > 0.0) && (fat.fat_tot_merce < fat.fat_tot_netto)) fat.fat_tot_merce = fat.fat_tot_netto;
			// Fine

			//
			// Riporto automatico bollo per fatture con ritenuta acconto
			//
			if (fat.fat_tot_netto > FattureExtensions.LIMITE_BOLLO)
			{
				double val = fat.fat_tot_netto - FattureExtensions.LIMITE_BOLLO;
				if (val.TestIfZero(dec) != true) fat.fat_bolli_eff += ditte[0].impo_bolli;
			}
			// Fine

			//**********************************
			// Codice Modificato il 27/03/2001 
			// fat->rec.tot_fattura = fat->rec.tot_netto + fat->rec.tot_non_docum  + fat->rec.tot_iva + fat->rec.art15 + fat->rec.bolli_eff;

			//
			// Codice Modificato il 26/01/2004 Calcolo spese bolli per tratte
			//
			if (((fat.fat_tipo == (int)TipoDocumento.TIPO_FAT) || (fat.fat_tipo == (int)TipoDocumento.TIPO_ACC)) || (fat.fat_tipo == (int)TipoDocumento.TIPO_RIC))
			{
				if (((fat.fat_credito == 0) && (fat.fat_bolli != 0)) && (fat.fat_recalc_bolli != 0))
				{
					try
					{
						var pag = await dbcon.GetAsync<Pagamenti>(fat.fat_pag);
						if (pag.pag_tipo_pag == (int)TipoPagamento.PAG_TRATTA)
						{
							double totval = fat.fat_tot_fattura + fat.fat_tot_non_docum + fat.fat_tot_iva + fat.fat_art15 + fat.fat_bolli_eff;
							totval = Math.Round(totval * (ditte[0].impo_trat_perc_bolli / 100.0), dec, MidpointRounding.AwayFromZero);
							fat.fat_bolli_eff += totval;
						}
					}
					catch (System.Exception)
					{
						return (ErroreDocumento.PagamentoNotFound);
					}
				}
			}
			// Fine Variazione

			fat.fat_tot_fattura += fat.fat_tot_non_docum + fat.fat_tot_iva + fat.fat_art15 + fat.fat_bolli_eff + fat.fat_cassa;
			fat.fat_tot_fattura -= fat.fat_omaggi;
			fat.fat_tot_pagare = fat.fat_tot_fattura - fat.fat_abbuoni - fat.fat_anticipo - fat.fat_ritenuta_acconto;
			if (fat.fat_split_payment == true) fat.fat_tot_pagare -= fat.fat_tot_iva;
			fat.fat_tot_sconto += (fat.fat_tot_merce - fat.fat_tot_netto);

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
						Codiva iva = await dbcon.GetAsync<Codiva>(ditte[0].impo_iva_age);
						double al_iva = 0.0;
						if (iva.iva_tip == 1) al_iva = iva.iva_aliq;
						if (fat.fat_imp != 0)
							fat.fat_com_imponibile = Math.Round(fat.fat_tot_fattura * (fat.fat_agz_perc / 100.0), 2, MidpointRounding.AwayFromZero);
						else
							fat.fat_com_imponibile = Math.Round(fat.fat_totale_imponibile * (fat.fat_agz_perc / 100.0), 2, MidpointRounding.AwayFromZero);
						fat.fat_com_iva = Math.Round(fat.fat_com_imponibile * (al_iva / 100.0), 2, MidpointRounding.AwayFromZero);
						fat.fat_com_importo = fat.fat_com_imponibile + fat.fat_com_iva;

					}
					catch (System.Exception)
					{
						return (ErroreDocumento.IvaAgenteNotFound);
					}
				}
			}

			//
			// Rimuoviamo le aliquote Iva non Utilizzate e
			// 
			// 
			if (fat.fat_cod_iva_0 != 0L)
			{
				bool test = true;
				if (fat.fat_ripartizione_0.TestIfZero(0) != true) test = false;
				if (fat.fat_imponibile_0.TestIfZero(0) != true) test = false;
				if (fat.fat_importo_0.TestIfZero(0) != true) test = false;
				if (fat.fat_importo_iva_0.TestIfZero(0) != true) test = false;
				if (test == true)
				{
					fat.fat_cod_iva_0 = 0;
					fat.fat_aliquota_iva_0 = 0.0;
					fat.fat_tipo_iva_0 = 0;
					fat.fat_desc_iva_0 = String.Empty;
				}
			}
			if (fat.fat_cod_iva_1 != 0L)
			{
				bool test = true;
				if (fat.fat_ripartizione_1.TestIfZero(0) != true) test = false;
				if (fat.fat_imponibile_1.TestIfZero(0) != true) test = false;
				if (fat.fat_importo_1.TestIfZero(0) != true) test = false;
				if (fat.fat_importo_iva_1.TestIfZero(0) != true) test = false;
				if (test == true)
				{
					fat.fat_cod_iva_1 = 0;
					fat.fat_aliquota_iva_1 = 0.0;
					fat.fat_tipo_iva_1 = 0;
					fat.fat_desc_iva_1 = String.Empty;
				}
			}
			if (fat.fat_cod_iva_2 != 0L)
			{
				bool test = true;
				if (fat.fat_ripartizione_2.TestIfZero(0) != true) test = false;
				if (fat.fat_imponibile_2.TestIfZero(0) != true) test = false;
				if (fat.fat_importo_2.TestIfZero(0) != true) test = false;
				if (fat.fat_importo_iva_2.TestIfZero(0) != true) test = false;
				if (test == true)
				{
					fat.fat_cod_iva_2 = 0;
					fat.fat_aliquota_iva_2 = 0.0;
					fat.fat_tipo_iva_2 = 0;
					fat.fat_desc_iva_2 = String.Empty;
				}
			}
			if (fat.fat_cod_iva_3 != 0L)
			{
				bool test = true;
				if (fat.fat_ripartizione_3.TestIfZero(0) != true) test = false;
				if (fat.fat_imponibile_3.TestIfZero(0) != true) test = false;
				if (fat.fat_importo_3.TestIfZero(0) != true) test = false;
				if (fat.fat_importo_iva_3.TestIfZero(0) != true) test = false;
				if (test == true)
				{
					fat.fat_cod_iva_3 = 0;
					fat.fat_aliquota_iva_3 = 0.0;
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
				fat.fat_aliquota_iva_0 = fat.fat_aliquota_iva_1;
				fat.fat_importo_iva_0 = fat.fat_importo_iva_1;
				fat.fat_tipo_iva_0 = fat.fat_tipo_iva_1;
				fat.fat_desc_iva_0 = fat.fat_desc_iva_1;

				fat.fat_cod_iva_1 = 0;
				fat.fat_ripartizione_1 = 0.0;
				fat.fat_imponibile_1 = 0.0;
				fat.fat_importo_1 = 0.0;
				fat.fat_aliquota_iva_1 = 0.0;
				fat.fat_importo_iva_1 = 0.0;
				fat.fat_tipo_iva_1 = 0;
				fat.fat_desc_iva_1 = String.Empty;
			}
			if (fat.fat_cod_iva_1 == 0)
			{
				fat.fat_cod_iva_1 = fat.fat_cod_iva_2;
				fat.fat_ripartizione_1 = fat.fat_ripartizione_2;
				fat.fat_imponibile_1 = fat.fat_imponibile_2;
				fat.fat_importo_1 = fat.fat_importo_2;
				fat.fat_aliquota_iva_1 = fat.fat_aliquota_iva_2;
				fat.fat_importo_iva_1 = fat.fat_importo_iva_2;
				fat.fat_tipo_iva_1 = fat.fat_tipo_iva_2;
				fat.fat_desc_iva_1 = fat.fat_desc_iva_2;

				fat.fat_cod_iva_2 = 0;
				fat.fat_ripartizione_2 = 0.0;
				fat.fat_imponibile_2 = 0.0;
				fat.fat_importo_2 = 0.0;
				fat.fat_aliquota_iva_2 = 0.0;
				fat.fat_importo_iva_2 = 0.0;
				fat.fat_tipo_iva_2 = 0;
				fat.fat_desc_iva_2 = String.Empty;
			}
			if (fat.fat_cod_iva_2 == 0)
			{
				fat.fat_cod_iva_2 = fat.fat_cod_iva_3;
				fat.fat_ripartizione_2 = fat.fat_ripartizione_3;
				fat.fat_imponibile_2 = fat.fat_imponibile_3;
				fat.fat_importo_2 = fat.fat_importo_3;
				fat.fat_aliquota_iva_2 = fat.fat_aliquota_iva_3;
				fat.fat_importo_iva_2 = fat.fat_importo_iva_3;
				fat.fat_tipo_iva_2 = fat.fat_tipo_iva_3;
				fat.fat_desc_iva_2 = fat.fat_desc_iva_3;

				fat.fat_cod_iva_3 = 0;
				fat.fat_ripartizione_3 = 0.0;
				fat.fat_imponibile_3 = 0.0;
				fat.fat_importo_3 = 0.0;
				fat.fat_aliquota_iva_3 = 0.0;
				fat.fat_importo_iva_3 = 0.0;
				fat.fat_tipo_iva_3 = 0;
				fat.fat_desc_iva_3 = String.Empty;
			}

			return (ErroreDocumento.NoError);
		}

	}
}

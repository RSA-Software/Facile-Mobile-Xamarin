using System;
using System.Threading.Tasks;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Xamarin.Forms;

namespace Facile.Extension
{
	public static class FattureExtensions
	{
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

		public async static Task<int> GetTotali(this Fatture fat)
		{
			int i;
			double [] rip_cassa = new double[4];
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
								SQLiteAsyncConnection dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();
								var iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_0);
								rip_cassa[i] = Math.Round(fat.fat_imponibile_0 * (iva.iva_cassa / 100.0), dec, MidpointRounding.AwayFromZero);
							}
							catch (System.Exception)
							{
								return (1);
							}
						}
						break;

					case 1:
						fat.fat_ripartizione_1 = 0.0;
						if (fat.fat_importo_cas_1.TestIfZero(dec) != true)
						{
							try
							{
								SQLiteAsyncConnection dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();
								var iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_1);
								rip_cassa[i] = Math.Round(fat.fat_imponibile_1 * (iva.iva_cassa / 100.0), dec, MidpointRounding.AwayFromZero);
							}
							catch (System.Exception)
							{
								return (1);
							}
						}
						break;

					case 2:
						fat.fat_ripartizione_2 = 0.0;
						if (fat.fat_importo_cas_2.TestIfZero(dec) != true)
						{
							try
							{
								SQLiteAsyncConnection dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();
								var iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_2);
								rip_cassa[i] = Math.Round(fat.fat_imponibile_2 * (iva.iva_cassa / 100.0), dec, MidpointRounding.AwayFromZero);
							}
							catch (System.Exception)
							{
								return (1);
							}
						}
						break;

					case 3:
						fat.fat_ripartizione_3 = 0.0;
						if (fat.fat_importo_cas_3.TestIfZero(dec) != true)
						{
							try
							{
								SQLiteAsyncConnection dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();
								var iva = await dbcon.GetAsync<Codiva>(fat.fat_cod_iva_3);
								rip_cassa[i] = Math.Round(fat.fat_imponibile_3 * (iva.iva_cassa / 100.0), dec, MidpointRounding.AwayFromZero);
							}
							catch (System.Exception)
							{
								return (1);
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
			var tot = fat.fat_imponibile_0 + fat.fat_imponibile_1 + fat.fat_imponibile_2 + fat.fat_imponibile_3;

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
/*
				if (fat.fat_credito == FALSE)
				{
					if ((fat.fat_spese == TRUE) && (fat.fat_recalc_trasp == true))
					{
						auto perc = 0.0;
						const auto importo_merci = tot - tot * (fat.fat_sconto / 100.0);
						for (auto xxx = 0; xxx < 5; xxx++)
						{
							if (importo_merci <= facile_db_impo.spetra_imp[xxx])
							{
								perc = facile_db_impo.spetra_per[xxx];
								break;
							}
						}
						fat.fat_imballo = Round(importo_merci * (perc / 100.0), dec);
						fat.fat_imballo += facile_db_impo.spetra;
					}

					if ((fat.fat_spese == TRUE) && (fat.fat_recalc_varie == true))
					{
						fat.fat_varie = facile_db_impo.fat_spese;
					}

					if ((fat.fat_recalc_incef) || (fat.fat_recalc_bolli))
					{
						PAGAMENTI pag;

						ScanPagamento(fat->rec.pag, &pag);
						if (((fat.fat_spese == TRUE) && (pag.banche == TRUE)) && (fat.fat_recalc_incef == true))
						{
							fat.fat_inc_eff = pag.tot_banche;
						}
						if (((fat.fat_bolli == TRUE) && (pag.bolli == TRUE)) && (fat.fat_recalc_bolli == true))
						{
							fat.fat_bolli_eff += pag.tot_bolli;
						}
					}
				}
*/
			}

			/*
			//-----------------24/03/02 15.30-------------------
			//	Fine Codice Aggiunto
			//--------------------------------------------------

			fat->rec.tot_non_docum = fat->rec.imballo + fat->rec.varie + fat->rec.inc_eff;

			// Ripartizione Spese
			// 
			// Se il codice iva spese non è valido o pari a zero la ripartizione avviene in proporzione alle 
			// aliquote iva presenti nel documento, altrimenti vengono assoggettate all' aliquota iva preimpo-
			// stata.
			if (facile_db_impo.iva_spese == 0L)
			{
				if (tot != 0.0)
				{
					//
					// Correzione per problema Etna Frutta su ripartizione ticket 10314 (25/06/2011)
					// La correzione viene fatta decorrere da 01/08/2012 per non alterare i totali delle fatture emesse in precedenza
					// 
					const COleDateTime d(2012, 7, 9, 0, 0, 0);

					for (i = 0; i < 4; i++)
					{
						if (fat->rec.d_doc >= DateToJulian(DATE(d)))
							fat->rec.ripartizione[i] = MyFloor(fat->rec.tot_non_docum * (fat->rec.imponibile[i] / tot), 2);
						else
							fat->rec.ripartizione[i] = floor(fat->rec.tot_non_docum * (fat->rec.imponibile[i] / tot));
					}
				}
				tot = fat->rec.ripartizione[0] + fat->rec.ripartizione[1] + fat->rec.ripartizione[2] + fat->rec.ripartizione[3];
				tot = fat->rec.tot_non_docum - tot;
				int max = 0;
				for (i = 1; i < 4; i++)
				{
					if ((fat->rec.aliquota_iva[i] > fat->rec.aliquota_iva[max]) && (fat->rec.imponibile[i] != 0.0)) max = i;
				}
				fat->rec.ripartizione[max] += tot;
			}
			else
			{
				BOOL found = FALSE;
				for (i = 0; i < 4; i++)
				{
					if (fat->rec.cod_iva[i] == facile_db_impo.iva_spese)
					{
						fat->rec.ripartizione[i] = fat->rec.tot_non_docum;
						found = TRUE;
						break;
					}
				}
				if (found == FALSE)
				{
					for (i = 0; i < 4; i++)
					{
						if (fat->rec.cod_iva[i] == 0L)
						{
							CODIVA iva;

							found = TRUE;
							DBISAM(ScanCodiva(facile_db_impo.iva_spese, &iva), ATFILE);
							fat->rec.cod_iva[i] = facile_db_impo.iva_spese;
							zerocpy(fat->rec.desc_iva[i], iva.desc);
							fat->rec.tipo_iva[i] = iva.tip;
							if (iva.tip == 1) fat->rec.aliquota_iva[i] = iva.aliq;
							fat->rec.ripartizione[i] = fat->rec.tot_non_docum;
							break;
						}
					}
					if (found == FALSE) return (IDS_ERR_TROPPI);
				}
			}


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

			// Codice Aggiunto il 25/05/2006 per evitare che il totale merci sia inferiore al totale netto
			if ((fat->rec.tot_merce > 0.0) && (fat->rec.tot_merce < fat->rec.tot_netto)) fat->rec.tot_merce = fat->rec.tot_netto;
			// Fine

			//
			// Riporto automatico bollo per fatture con ritenuta acconto
			//
			if (fat->rec.tot_netto > LIMITE_BOLLO)
			{
				auto val = fat->rec.tot_netto - LIMITE_BOLLO;
				if (TestIfZero(&val, dec) != TRUE) fat->rec.bolli_eff += facile_db_impo.bolli;
			}
			// Fine


			//**********************************
			// Codice Modificato il 27/03/2001 
			// fat->rec.tot_fattura = fat->rec.tot_netto + fat->rec.tot_non_docum  + fat->rec.tot_iva + fat->rec.art15 + fat->rec.bolli_eff;

			//
			// Codice Modificato il 26/01/2004 Calcolo spese bolli per tratte
			//
			if (((fat->rec.tipo == TIPO_FAT) || (fat->rec.tipo == TIPO_ACC)) || (fat->rec.tipo == TIPO_RIC))
			{
				if (((fat->rec.credito == FALSE) && (fat->rec.bolli == TRUE)) && (fat->rec.recalc_bolli == true))
				{
					PAGAMENTI pag;

					BOOL test = FALSE;
					ScanPagamento(fat->rec.pag, &pag);
					if (pag.tipo_pag == PAG_TRATTA) test = TRUE;
					if (test == TRUE)
					{
						auto totval = fat->rec.tot_fattura + fat->rec.tot_non_docum + fat->rec.tot_iva + fat->rec.art15 + fat->rec.bolli_eff;
						totval = Round(totval * (facile_db_impo.trat_perc_bolli / 100.0), dec);
						fat->rec.bolli_eff += totval;
					}
				}
			}
			// Fine Variazione

			fat->rec.tot_fattura += fat->rec.tot_non_docum + fat->rec.tot_iva + fat->rec.art15 + fat->rec.bolli_eff + fat->rec.cassa;
			fat->rec.tot_fattura -= fat->rec.omaggi;
			if (facile_db_impo.anno < 2002)
			{
				if (fat->rec.calc_abbuoni == FALSE) fat->rec.abbuoni = fat->rec.tot_fattura - floor(fat->rec.tot_fattura);
			}
			fat->rec.tot_pagare = fat->rec.tot_fattura - fat->rec.abbuoni - fat->rec.anticipo - fat->rec.ritenuta_acconto;
			if (fat->rec.split_payment == true) fat->rec.tot_pagare -= fat->rec.tot_iva;
			fat->rec.tot_sconto += static_cast<float>(fat->rec.tot_merce - fat->rec.tot_netto);
	# ifdef BOLLICINE
			for (i = 0; i < MAX_VUOTI; i++)
			{
				fat->rec.vuo_diff[i] = fat->rec.vuo_prec[i] + fat->rec.vuo_cons[i] - fat->rec.vuo_resi[i];
				fat->rec.vuo_tot_cau[i] = Round(fat->rec.vuo_diff[i] * fat->rec.vuo_pre_cau[i], dec);
			}
			fat->rec.tot_pagare -= fat->rec.vuo_cauzione_resa;
	#endif

			if (fat->rec.rie == FALSE)
			{
				fat->rec.com_imponibile = 0.0;
				fat->rec.com_iva = 0.0;
				fat->rec.com_importo = 0.0;
				if (fat->rec.com == true)
				{
					CODIVA iva;

					auto al_iva = 0.0;
					ScanCodiva(facile_db_impo.iva_age, &iva);
					if (iva.tip == 1) al_iva = iva.aliq;
					if (facile_db_impo.anno >= 2002)
					{
						if (fat->rec.imp == TRUE)
							fat->rec.com_imponibile = Round(fat->rec.tot_fattura * (fat->rec.agz_perc / 100.0), 2);
						else
							fat->rec.com_imponibile = Round(fat->rec.totale_imponibile * (fat->rec.agz_perc / 100.0), 2);
						fat->rec.com_iva = Round(fat->rec.com_imponibile * (al_iva / 100.0), 2);
						fat->rec.com_importo = fat->rec.com_imponibile + fat->rec.com_iva;
					}
					else
					{
						if (fat->rec.imp == TRUE)
							fat->rec.com_imponibile = MyCeil(fat->rec.tot_fattura * (fat->rec.agz_perc / 100.0), 2);
						else
							fat->rec.com_imponibile = MyCeil(fat->rec.totale_imponibile * (fat->rec.agz_perc / 100.0), 2);
						fat->rec.com_iva = Round(fat->rec.com_imponibile * (al_iva / 100.0), 2);
						fat->rec.com_importo = fat->rec.com_imponibile + fat->rec.com_iva;
					}
				}
			}


			//
			// Rimuoviamo le aliquote Iva non Utilizzate e
			// 
			// 
			for (i = 0; i < 4; i++)
			{
				if (fat->rec.cod_iva[i] != 0L)
				{
					BOOL test = TRUE;
					if (fat->rec.ripartizione[i] != 0.0) test = FALSE;
					if (fat->rec.imponibile[i] != 0.0) test = FALSE;
					if (fat->rec.importo[i] != 0.0) test = FALSE;
					if (fat->rec.importo_iva[i] != 0.0) test = FALSE;
					if (test == TRUE)
					{
						fat->rec.cod_iva[i] = 0;
						fat->rec.aliquota_iva[i] = 0.0;
						fat->rec.tipo_iva[i] = 0;
						zerocpy(fat->rec.desc_iva[i], "\x0");
					}
				}
			}
			for (i = 0; i < 3; i++)
			{
				if (fat->rec.cod_iva[i] == 0L)
				{
					fat->rec.cod_iva[i] = fat->rec.cod_iva[i + 1];
					fat->rec.ripartizione[i] = fat->rec.ripartizione[i + 1];
					fat->rec.imponibile[i] = fat->rec.imponibile[i + 1];
					fat->rec.importo[i] = fat->rec.importo[i + 1];
					fat->rec.aliquota_iva[i] = fat->rec.aliquota_iva[i + 1];
					fat->rec.importo_iva[i] = fat->rec.importo_iva[i + 1];
					fat->rec.tipo_iva[i] = fat->rec.tipo_iva[i + 1];
					zerocpy(fat->rec.desc_iva[i], fat->rec.desc_iva[i + 1]);

					fat->rec.cod_iva[i + 1] = 0L;
					fat->rec.ripartizione[i + 1] = 0.0;
					fat->rec.imponibile[i + 1] = 0.0;
					fat->rec.importo[i + 1] = 0.0;
					fat->rec.aliquota_iva[i + 1] = 0.0;
					fat->rec.importo_iva[i + 1] = 0.0;
					fat->rec.tipo_iva[i + 1] = 0L;
					zerocpy(fat->rec.desc_iva[i + 1], "\x0");
				}
			}
			return (NO_ERROR);
			*/

			return (0);
		}

	}
}

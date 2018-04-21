using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Facile.Extension;
using Facile.Interfaces;
using Facile.Models;
using Facile.Utils;
using LinkOS.Plugin;
using LinkOS.Plugin.Abstractions;
using PCLStorage;
using SQLite;
using Xamarin.Forms;
using static Facile.Extension.FattureExtensions;

namespace Facile
{
	public class ZebraPrn
	{
		private Page _parent;
		private IConnection _con;
		private IZebraPrinter _prn;
		private readonly SQLiteAsyncConnection dbcon_;
		private int _mod_len;
		private List<FatRow> _riglist;
		private string _codcli;
		private string _coddst;
		private string _codocr;

		public ZebraPrn(Page par)
		{
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			_parent = par;
			_con = null;
			_prn = null;
			_riglist = null;
			_mod_len = 200 * 8;  // Lunghezza Modulo inziale 20 cm
			_codcli = "";
			_coddst = "";
			_codocr = "";
		}

		public void Initialize()
		{
			SetZplPrinterLanguage();
			PrePrintCheckStatus();
		}

		public async Task<int> PrintDocHeaderAsync(Fatture doc, int row, bool stprice)
		{
			Destinazioni dst = null;
			Agenti age = null;
			var cli = await dbcon_.GetAsync<Clienti>(doc.fat_inte);
			if (doc.fat_dest != 0) dst = await dbcon_.GetAsync<Destinazioni>(doc.fat_dest);
			if (doc.fat_age != 0) age = await dbcon_.GetAsync<Agenti>(doc.fat_age);

			int col = 0;
			int last_hor = 0;
			string logo = "";


			IFolder rootFolder = FileSystem.Current.LocalStorage;
			string path = rootFolder.Path + "/images/" + "logo.prn";

			IFile file = await rootFolder.CreateFileAsync(path, CreationCollisionOption.OpenIfExists);
			if (file != null)
			{
				
				logo = await file.ReadAllTextAsync();
			}


			string str = "^XA" +      // Inizializziamo la stampa
				"^PW800" +            // Settiamo la Larghezza
				"^MN" +               // Settiamo la stampa in continuos mode
				"^POI" +
				"^LH0,0";             // Settiamo la posizione Iniziale
			

			// Settiamo la lunghezza iniziale del modulo
			str = str + $"^LL{_mod_len}";

			if (logo != "") str = str + logo;

			col = 0;
			row= 30 * 8;

			// Scriviamo il Tipo di Documento
			string num = "";
			switch (doc.fat_tipo)
			{
				case (int)TipoDocumento.TIPO_BOL: num = "B O L L A"; break;
				case (int)TipoDocumento.TIPO_DDT: num = "DOCUMENTO GENERALE DI TRASPORTO"; break;
				case (int)TipoDocumento.TIPO_BUO: num = "B U O N O  D I  C O N S E G N A"; break;
				case (int)TipoDocumento.TIPO_ORD: num = "O R D I N E"; break;
				default: num = "FATTURA/NOTA CONSEGNA TENTATA VENDITA D.P.R.472/96 art.1 comma 3"; break;
			}
			str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB784,1,0,R,0^FD{num}^FS";

			// Disegniamo il box
			col = 0;
			row += 3 * 8;
			last_hor = row;
			str = str + $"^FO{col},{row}" + "^GB800,388,2^FS";

			row += 1 * 8;
			col = 3;
			str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDNum. Doc.^FS";

			col = 16 * 8;
			str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDData^FS";

			col = 32 * 8;
			str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDCod.Cli.^FS";

			col = 42 * 8;
			str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDCod.Age.^FS";

			col = 52 * 8;
			str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDTarga^FS";

			col = 73 * 8;
			str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDTelefono^FS";


			// Tracciamo le linee Verticali
			col =  15 * 8;
			str = str + $"^FO{col},{last_hor}" + $"^GB1,{7 * 8},2^FS";

			col = 31 * 8;
			str = str + $"^FO{col},{last_hor}" + $"^GB1,{7 * 8},2^FS";

			col = 41 * 8;
			str = str + $"^FO{col},{last_hor}" + $"^GB1,{7 * 8},2^FS";

			col = 51 * 8;
			str = str + $"^FO{col},{last_hor}" + $"^GB1,{7 * 8},2^FS";

			col = 71 * 8;
			str = str + $"^FO{col},{last_hor}" + $"^GB1,{7 * 8},2^FS";


			row += 3 * 8;
			col = 1;
			num = string.Format("{0:#,#}/{1}", RsaUtils.GetShowedNumDoc(doc.fat_n_doc), RsaUtils.GetRegistro(doc.fat_n_doc));
			str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{14 * 8},1,0,R,0^FD{num}^FS";

			col = 16 * 8;
			str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{14 * 8},1,0,C,0^FD{doc.fat_d_doc:dd/MM/yyy}^FS";

			col = 32 * 8;
			num = string.Format("{0:#}", doc.fat_inte);
			str = str + $"^FO{col},{row}" + "^A0,N,25,25" + $"^FB{8 * 8},1,0,R,0^FD{num}^FS";

			col = 42 * 8;
			num = string.Format("{0:#}", doc.fat_age);
			str = str + $"^FO{col},{row}" + "^A0,N,25,25" + $"^FB{8 * 8},1,0,R,0^FD{num}^FS";

			col = 52 * 8;
			if (age != null)
			{
				str = str + $"^FO{col},{row}" + "^A0,N,25,25" + $"^FB{18 * 8},1,0,L,0^FD{age.age_targa.Trim()}^FS";
			}

			col = 73 * 8;
			if (age != null)
			{
				str = str + $"^FO{col},{row}" + "^A0,N,25,25" + $"^FD{age.age_targa.Trim()}^FS";
			}

			// Tracciamo la linea orizzontale
			col = 0;
			row += 3 * 8;
			last_hor = row;
			str = str + $"^FO{col},{row}" + $"^GB800,1,2^FS";

			// Intestazione Cliente
			row += 1 * 8;
			col = 1 * 8;
			str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDCliente / Cessionario^FS";

			col = 80 * 8;
			str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{18 * 8},1,0,R,0^FD{_codcli}^FS";

			row += 4 * 8;
			col = 1 * 8;
			str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FD{cli.cli_rag_soc1.Trim()}^FS";

			row += 3 * 8;
			str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FD{cli.cli_rag_soc2.Trim()}^FS";

			row += 3 * 8;
			str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FD{cli.cli_indirizzo.Trim()}^FS";

			row += 3 * 8;
			num = string.Format("{0} {1} {2}", cli.cli_cap, cli.cli_citta, cli.cli_prov);
			str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FD{num.Trim()}^FS";

			row += 3 * 8;
			num = "";
			if (!string.IsNullOrWhiteSpace(cli.cli_piva)) num = $"P.IVA {cli.cli_piva}";
			num = num + "       ";
			if (!string.IsNullOrWhiteSpace(cli.cli_codfis)) num = $"Cod. Fiscale {cli.cli_codfis}";
			str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FD{num.Trim()}^FS";

			// Tracciamo la linea orizzontale
			col = 0;
			row += 3 * 8;
			last_hor = row;
			str = str + $"^FO{col},{row}" + $"^GB800,1,2^FS";


			// Intestazione Destinazione
			row += 1 * 8;
			col = 1 * 8;
			str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDDestinatario e Luogo di Consegna (se diverso dal cessionario) ^FS";

			col = 80 * 8;
			str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{18 * 8},1,0,R,0^FD{_coddst}^FS";

			col = 1 * 8;
			row += 4 * 8;
			if (dst != null)
			{
				str = str + $"^FO{col},{row}" + "^A0,N,25,25" + $"^FD{dst.dst_rag_soc1.Trim()}^FS";
			}

			row += 3 * 8;
			if (dst != null)
			{
				str = str + $"^FO{col},{row}" + "^A0,N,25,25" + $"^FD{dst.dst_rag_soc2.Trim()}^FS";
			}

			row += 3 * 8;
			if (dst != null)
			{
				str = str + $"^FO{col},{row}" + "^A0,N,25,25" + $"^FD{dst.dst_indirizzo.Trim()}^FS";
			}

			row += 3 * 8;
			if (dst != null)
			{
				num = string.Format("{0} {1} {2}", dst.dst_cap, dst.dst_citta, dst.dst_prov);
				str = str + $"^FO{col},{row}" + "^A0,N,25,25" + $"^FD{num.Trim()}^FS";
			}

			// Tracciamo la linea orizzontale
			col = 0;
			row += 3 * 8;
			last_hor = row;
			str = str + $"^FO{col},{row}" + $"^GB800,1,2^FS";

			col = 1*8;
			row += 1 * 8;

			str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDCodice^FS";

			col = 15*8;
			str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDDescrizione^FS";

			if (stprice)
			{
				col = 30*8;
				str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDU.M.^FS";

				col = 43*8;
				str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDQuantita'^FS";

				col = 58*8;
				str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDPrezzo^FS";

				col = 70*8;
				str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDSconti^FS";

				col = 83 * 8;
				str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDTotale^FS";

				col = 92*8;
				str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDCod.Iva^FS";
			}
			else
			{
				col = 77*8;
				str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDU.M.^FS";

				col = 92*8;
				str = str + $"^FO{col},{row}" + "^A0,N,19,19" + "^FDQuantita'^FS";
			}

			//str = str + "^XZ";
			var t = new UTF8Encoding().GetBytes(str);
			_con.Write(t);

			PostPrintCheckStatus();
			row += 3;
			return (82 * 8);
		}

		public async Task<int> PrintDocBodyAsync(Fatture doc, int row, bool stprice)
		{
			string str = "";
			int col = 0;

			row += 1 * 8;
			if (!string.IsNullOrWhiteSpace(_codocr))
			{
				col += 1 * 8;
				row += 1 * 8;
				str = $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{98 * 8},1,0,C,0^FD{_codocr.Trim()}^FS";
				row += 5 * 8;
			}

			foreach(var rig in _riglist)
			{
				Misure mis = null;

				if (rig.rig_mis != 0) mis = await dbcon_.GetAsync<Misure>(rig.rig_mis);

				if (stprice)
				{
					string num;

					col = 1*8;
					str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{14 * 8},1,0,L,0^FD{rig.rig_art}^FS";

					col = 15*8;

					rig.rig_newdes = rig.rig_newdes.Replace("\n\r", "\n");
					rig.rig_newdes = rig.rig_newdes.Replace("\r\n", "\n");
					var desarr = rig.rig_newdes.Split('\n');
					foreach (var des in desarr)
					{
						str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{84 * 8},1,0,L,0^FD{des.Trim()}^FS";
						row += 3 * 8;
					}

					if (!string.IsNullOrWhiteSpace(rig.rig_lotto))
					{
						string dati;

						if (rig.rig_scadenza.Year > 1900)
							dati = string.Format("Lotto {0} Scadenza {1:dd/MM/yyyy}", rig.rig_lotto.Trim(), rig.rig_scadenza);
						else
							dati = string.Format("Lotto {0}", rig.rig_lotto.Trim());

						str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FD{dati}^FS";
						row += 3 * 8;
					}

					// Misura
					col = 30 * 8;
					if (mis != null)
					{
						str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FD{mis.mis_abbr}^FS";
					}

					// Quantità
					col = 37 * 8;
					num = string.Format("{0:0.000}", rig.rig_qta);
					str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{14 * 8},1,0,R,0^FD{num}^FS";

					// Prezzo
					col = 52 * 8;
					num = string.Format("{0:0.0000}", rig.rig_prezzo);
					str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{13 * 8},1,0,R,0^FD{num}^FS";

					if (!rig.rig_sconto1.TestIfZero(2))
					{
						col = 66 * 8;
						num = string.Format("{0:0.00}", rig.rig_sconto1);
						str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{6 * 8},1,0,R,0^FD{num}^FS";
					}

					if (!rig.rig_sconto2.TestIfZero(2))
					{
						col = 73 * 8;
						num = string.Format("{0:0.00}", rig.rig_sconto2);
						str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{6 * 8},1,0,R,0^FD{num}^FS";
					}

					col = 80 * 8;
					if (rig.rig_sconto1 < 100.0)
					{
						num = string.Format("{0:0.00}", rig.rig_importo);
						str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{13 * 8},1,0,R,0^FD{num}^FS";
					}
					else
					{
						str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FDSCO.MERCE^FS";
					}

					col = 93 * 8;
					num = string.Format("{0}", rig.rig_iva);
					str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{6 * 8},1,0,R,0^FD{num}^FS";
				}
				else
				{
					string num;
					bool first = true;

					col += 1 * 8;
					str = $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{13 * 8},1,0,L,0^FD{rig.rig_art}^FS";

					rig.rig_newdes = rig.rig_newdes.Replace("\n\r", "\n");
					rig.rig_newdes = rig.rig_newdes.Replace("\r\n", "\n");
					var desarr = rig.rig_newdes.Split('\n');
					foreach (var des in desarr)
					{
						col = 15 * 8;
						str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{84 * 8},1,0,L,0^FD{des.Trim()}^FS";

						if (first == true)
						{
							// Misura
							row += 77;
							col = 30 * 8;
							if (mis != null)
							{
								str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FD{mis.mis_abbr}^FS";
							}

							// Quantità
							col = 91 * 8;
							num = string.Format("{0,9:0.000}", rig.rig_qta);
							str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FB{9 * 8},1,0,R,0^FD{num}^FS";

						}
						first = false;
						row += 3 * 8;
					}


					if (!string.IsNullOrWhiteSpace(rig.rig_lotto))
					{
						string dati;

						if (rig.rig_scadenza.Year > 1900)
							dati = string.Format("Lotto {0} Scadenza {1:dd/MM/yyyy}", rig.rig_lotto.Trim(), rig.rig_scadenza);
						else
							dati = string.Format("Lotto {0}}", rig.rig_lotto.Trim());
						str = str + $"^FO{col},{row}" + "^A0,N,23,23" + $"^FD{dati}^FS";
						row += 3 * 8;
					}
				}

				row += 4 * 8;

				var t = new UTF8Encoding().GetBytes(str);
				_con.Write(t);
				PostPrintCheckStatus();
			}
			return (row);
		}

		//
		// Lunghezza piede 55 mm
		//
		public async Task<int> PrintDocFooterAsync(Fatture doc, int row, bool stprice)   
		{
			string num = "";
			string str = "";
			Pagamenti pag = null;
			if (doc.fat_pag != 0) pag = await dbcon_.GetAsync<Pagamenti>(doc.fat_pag);

			if ((row + 55 * 8) < _mod_len) row = _mod_len - 55 * 8;


			// Disegniamo il Box
			int col = 0;
			str = str + $"^FO{col},{row}" + $"^GB800,{40 * 8},2^FS";

			// Disegniamo la linea
			str = str + $"^FO{col},{row + 30 * 8}" + $"^GB800,1,2^FS";

			// Stampiamo il codice del pagamento
			col = 1;
			str = str + $"^FO{col},{row + 31 * 8}" + "^A0,N,19,19" + $"^FDCod. Pag.^FS";

			if (pag != null)
			{
				col = 2 * 8;
				num = string.Format("{0,5}", doc.fat_pag);
				str = str + $"^FO{col},{row + 35 * 8}" + "^A0,N,23,23" + $"^FB{7 * 8},1,0,R,0^FD{num}^FS";
			}

			// Stampiamo la Linea Vericale
			col = 10 * 8;
			str = str + $"^FO{col},{row + 30 * 8}" + $"^GB1,{10 * 8},2^FS";

			col = 11 * 8;
			str = str + $"^FO{col},{row + 31 * 8}" + "^A0,N,19,19" + $"^FDDes. Pag.^FS";

			if (pag != null)
			{
				str = str + $"^FO{col},{row + 35 * 8}" + "^A0,N,23,23" + $"^FD{pag.pag_desc.Trim()}^FS";
			}

			// Stampiamo la Linea Vericale
			col = 57 * 8;
			str = str + $"^FO{col},{row + 30 * 8}" + $"^GB1,{10 * 8},2^FS";

			col = 58 * 8;
			str = str + $"^FO{col},{row + 31 * 8}" + "^A0,N,19,19" + $"^FDFirma per Accettazione di Quanto Sopra^FS";

			if (doc.fat_tipo == (int)TipoDocumento.TIPO_FAT)
			{
				col = 35 * 8;
				str = str + $"^FO{col},{row + 41 * 8}" + "^A0,N,19,19" + $"^FDCONTRIBUTO CONAI ASSOLTO^FS";
			}

			if (doc.fat_tipo != (int)TipoDocumento.TIPO_DDT)
			{
				//
				// Prima Riga Totali Fattura
				//

				col = 0;
				str = str + $"^FO{col},{row + 7 * 8}" + $"^GB800,1,2^FS";

				col = 20 * 8;
				str = str + $"^FO{col},{row}" + $"^GB1,{7 * 8},2^FS";  // Sconto

				col = 30 * 8;
				str = str + $"^FO{col},{row}" + $"^GB1,{7 * 8},2^FS";  // Totale Netto

				col = 50 * 8;
				str = str + $"^FO{col},{row}" + $"^GB1,{7 * 8},2^FS";  // Trasporto Imballo

				col = 66 * 8;
				str = str + $"^FO{col},{row}" + $"^GB1,{7 * 8},2^FS";  // Varie

				col = 83 * 8;
				str = str + $"^FO{col},{row}" + $"^GB1,{7 * 8},2^FS";  // Incasso Effetti

				col = 1 * 8;
				str = str + $"^FO{col},{row + 1 * 8}" + "^A0,N,19,19" + $"^FDTotale Merce^FS";

				col = 21 * 8;
				str = str + $"^FO{col},{row + 1 * 8}" + "^A0,N,19,19" + $"^FDSconto %^FS";

				col = 31 * 8;
				str = str + $"^FO{col},{row + 1 * 8}" + "^A0,N,19,19" + $"^FDTotale Netto^FS";

				col = 51 * 8;
				str = str + $"^FO{col},{row + 1 * 8}" + "^A0,N,19,19" + $"^FDTrasp. Imballo^FS";

				col = 67 * 8;
				str = str + $"^FO{col},{row + 1 * 8}" + "^A0,N,19,19" + $"^FDVarie^FS";

				col = 84 * 8;
				str = str + $"^FO{col},{row + 1 * 8}" + "^A0,N,19,19" + $"^FDInc. Effetti^FS";

				col = 2 * 8;
				num = string.Format("{0:#,##0.00}", doc.fat_tot_merce);
				str = str + $"^FO{col},{row + 4 * 8}" + "^A0,N,23,23" + $"^FB{17 * 8},1,0,R,0^FD{num}^FS";

				if (!doc.fat_sconto.TestIfZero(2))
				{
					col = 22 * 8;
					num = string.Format("{0:0.00}", doc.fat_sconto);
					str = str + $"^FO{col},{row + 4 * 8}" + "^A0,N,23,23" + $"^FB{7 * 8},1,0,R,0^FD{num}^FS";
				}

				col = 32 * 8;
				num = string.Format("{0:#,##0.00}", doc.fat_tot_netto);
				str = str + $"^FO{col},{row + 4 * 8}" + "^A0,N,23,23" + $"^FB{17 * 8},1,0,R,0^FD{num}^FS";

				if (!doc.fat_imballo.TestIfZero(2))
				{
					col = 52 * 8;
					num = string.Format("{0:#,##0.00}", doc.fat_imballo);
					str = str + $"^FO{col},{row + 4 * 8}" + "^A0,N,23,23" + $"^FB{13 * 8},1,0,R,0^FD{num}^FS";
				}

				if (!doc.fat_varie.TestIfZero(2))
				{
					col = 68 * 8;
					num = string.Format("{0:#,##0.00}", doc.fat_varie);
					str = str + $"^FO{col},{row + 4 * 8}" + "^A0,N,23,23" + $"^FB{14 * 8},1,0,R,0^FD{num}^FS";
				}

				if (!doc.fat_inc_eff.TestIfZero(2))
				{
					col = 85 * 8;
					num = string.Format("{0:#,##0.00}", doc.fat_inc_eff);
					str = str + $"^FO{col},{row + 4 * 8}" + "^A0,N,23,23" + $"^FB{13 * 8},1,0,R,0^FD{num}^FS";
				}
						
				//
				// Castelletto IVA
				//
				col = 2 * 8;
				str = str + $"^FO{col},{row + 8 * 8}" + "^A0,N,19,19" + $"^FDCod.^FS";

				col = 11 * 8;
				str = str + $"^FO{col},{row + 8 * 8}" + "^A0,N,19,19" + $"^FDImponibile^FS";

				col = 25 * 8;
				str = str + $"^FO{col},{row + 8 * 8}" + "^A0,N,19,19" + $"^FDDescrizione^FS";

				col = 53 * 8;
				str = str + $"^FO{col},{row + 8 * 8}" + "^A0,N,19,19" + $"^FDImposta^FS";

				col = 0;
				str = str + $"^FO{col},{row + 10 * 8}" + $"^GB{66 * 8},1,2^FS";   



				str = str + $"^FO{col},{row + 23 * 8}" + $"^GB800,1,2^FS";

				col = 7 * 8;
				str = str + $"^FO{col},{row + 7 * 8}" + $"^GB1,{16 * 8},2^FS";  // Codice Iva

				col = 24 * 8;
				str = str + $"^FO{col},{row + 7 * 8}" + $"^GB1,{16 * 8},2^FS";  // Imponibile

				col = 49 * 8;
				str = str + $"^FO{col},{row + 7 * 8}" + $"^GB1,{16 * 8},2^FS";  // Descrizione

				col = 66 * 8;
				str = str + $"^FO{col},{row + 7 * 8}" + $"^GB1,{16 * 8},2^FS";  // Imposta

				col = 1*8;
				num = string.Format("{0:#}", doc.fat_cod_iva_0);
				str = str + $"^FO{col},{row + 11 * 8}" + "^A0,N,23,23" + $"^FB{5 * 8},1,0,R,0^FD{num}^FS";

				num = string.Format("{0:#}", doc.fat_cod_iva_1);
				str = str + $"^FO{col},{row + 14 * 8}" + "^A0,N,23,23" + $"^FB{5 * 8},1,0,R,0^FD{num}^FS";

				num = string.Format("{0:#}", doc.fat_cod_iva_2);
				str = str + $"^FO{col},{row + 17 * 8}" + "^A0,N,23,23" + $"^FB{5 * 8},1,0,R,0^FD{num}^FS";

				num = string.Format("{0:#}", doc.fat_cod_iva_3);
				str = str + $"^FO{col},{row + 20 * 8}" + "^A0,N,23,23" + $"^FB{5 * 8},1,0,R,0^FD{num}^FS";

				col = 8 * 8;
				if (doc.fat_cod_iva_0 != 0)
				{
					num = string.Format("{0:#,##0.00}", doc.fat_ripartizione_0 + doc.fat_importo_0 );
					str = str + $"^FO{col},{row + 11 * 8}" + "^A0,N,23,23" + $"^FB{15 * 8},1,0,R,0^FD{num}^FS";
				}

				if (doc.fat_cod_iva_1 != 0)
				{
					num = string.Format("{0:#,##0.00}", doc.fat_ripartizione_1 + doc.fat_importo_1);
					str = str + $"^FO{col},{row + 14 * 8}" + "^A0,N,23,23" + $"^FB{15 * 8},1,0,R,0^FD{num}^FS";
				}

				if (doc.fat_cod_iva_2 != 0)
				{
					num = string.Format("{0:#,##0.00}", doc.fat_ripartizione_2 + doc.fat_importo_2);
					str = str + $"^FO{col},{row + 17 * 8}" + "^A0,N,23,23" + $"^FB{15 * 8},1,0,R,0^FD{num}^FS";
				}

				if (doc.fat_cod_iva_3 != 0)
				{
					num = string.Format("{0:#,##0.00}", doc.fat_ripartizione_3 + doc.fat_importo_3);
					str = str + $"^FO{col},{row + 20 * 8}" + "^A0,N,23,23" + $"^FB{15 * 8},1,0,R,0^FD{num}^FS";
				}

				col = 25 * 8;
				str = str + $"^FO{col},{row + 11 * 8}" + "^A0,N,23,23" + $"^FB{23 * 8},1,0,L,0^FD{doc.fat_desc_iva_0.Trim()}^FS";
				str = str + $"^FO{col},{row + 14 * 8}" + "^A0,N,23,23" + $"^FB{23 * 8},1,0,L,0^FD{doc.fat_desc_iva_1.Trim()}^FS";
				str = str + $"^FO{col},{row + 17 * 8}" + "^A0,N,23,23" + $"^FB{23 * 8},1,0,L,0^FD{doc.fat_desc_iva_2.Trim()}^FS";
				str = str + $"^FO{col},{row + 20 * 8}" + "^A0,N,23,23" + $"^FB{23 * 8},1,0,L,0^FD{doc.fat_desc_iva_3.Trim()}^FS";

				col = 50 * 8;
				if (doc.fat_cod_iva_0 != 0)
				{
					num = string.Format("{0:#,##0.00}", doc.fat_importo_iva_0);
					str = str + $"^FO{col},{row + 11 * 8}" + "^A0,N,23,23" + $"^FB{15 * 8},1,0,R,0^FD{num}^FS";
				}

				if (doc.fat_cod_iva_1 != 0)
				{
					num = string.Format("{0:#,##0.00}", doc.fat_importo_iva_1);
					str = str + $"^FO{col},{row + 14 * 8}" + "^A0,N,23,23" + $"^FB{15 * 8},1,0,R,0^FD{num}^FS";
				}

				if (doc.fat_cod_iva_2 != 0)
				{
					num = string.Format("{0:#,##0.00}", doc.fat_importo_iva_2);
					str = str + $"^FO{col},{row + 17 * 8}" + "^A0,N,23,23" + $"^FB{15 * 8},1,0,R,0^FD{num}^FS";
				}

				if (doc.fat_cod_iva_3 != 0)
				{
					num = string.Format("{0:#,##0.00}", doc.fat_importo_iva_3);
					str = str + $"^FO{col},{row + 20 * 8}" + "^A0,N,23,23" + $"^FB{15 * 8},1,0,R,0^FD{num}^FS";
				}

				//
				// Ultima riga totali
				//
				col = 1 * 8;
				str = str + $"^FO{col},{row + 24 * 8}" + "^A0,N,19,19" + $"^FDTot. Imponibile^FS";

				col = 17 * 8;
				str = str + $"^FO{col},{row + 24 * 8}" + "^A0,N,19,19" + $"^FDTot. IVA^FS";

				col = 33 * 8;
				str = str + $"^FO{col},{row + 24 * 8}" + "^A0,N,19,19" + $"^FDTot. Bolli Eff.^FS";

				col = 49 * 8;
				str = str + $"^FO{col},{row + 24 * 8}" + "^A0,N,19,19" + $"^FDAnticipo^FS";

				col = 65 * 8;
				str = str + $"^FO{col},{row + 24 * 8}" + "^A0,N,19,19" + $"^FDAbbuoni^FS";

				col = 81 * 8;
				str = str + $"^FO{col},{row + 24 * 8}" + "^A0,N,19,19" + $"^FDTotale da Pagare^FS";

				//
				// Tracciamo le linee verticali
				//
				col = 16 * 8;
				str = str + $"^FO{col},{row + 23 * 8}" + $"^GB1,{7 * 8},2^FS";  // Totale Iva

				col = 32 * 8;
				str = str + $"^FO{col},{row + 23 * 8}" + $"^GB1,{7 * 8},2^FS";  // Totale Bolli Effetti

				col = 48 * 8;
				str = str + $"^FO{col},{row + 23 * 8}" + $"^GB1,{7 * 8},2^FS";  // Anticipo

				col = 64 * 8;
				str = str + $"^FO{col},{row + 23 * 8}" + $"^GB1,{7 * 8},2^FS";  // Abbuoni

				col = 80 * 8;
				str = str + $"^FO{col},{row + 23 * 8}" + $"^GB1,{7 * 8},2^FS";  // Totale da Pagare

				//
				// Stampiamo i Valori
				//

				col = 2 * 8;
				num = string.Format("{0:#,##0.00}", doc.fat_totale_imponibile);
				str = str + $"^FO{col},{row + 27 * 8}" + "^A0,N,23,23" + $"^FB{13 * 8},1,0,R,0^FD{num}^FS";

				col = 17 * 8;
				num = string.Format("{0:#,##0.00}", doc.fat_tot_iva);
				str = str + $"^FO{col},{row + 27 * 8}" + "^A0,N,23,23" + $"^FB{14 * 8},1,0,R,0^FD{num}^FS";

				if (!doc.fat_bolli_eff.TestIfZero(2))
				{
					col = 33 * 8;
					num = string.Format("{0:#,##0.00}", doc.fat_bolli_eff);
					str = str + $"^FO{col},{row + 27 * 8}" + "^A0,N,23,23" + $"^FB{14 * 8},1,0,R,0^FD{num}^FS";
				}

				if (!doc.fat_anticipo.TestIfZero(2))
				{
					col = 49 * 8;
					num = string.Format("{0:#,##0.00}", doc.fat_anticipo);
					str = str + $"^FO{col},{row + 27 * 8}" + "^A0,N,23,23" + $"^FB{14 * 8},1,0,R,0^FD{num}^FS";
				}

				if (!doc.fat_abbuoni.TestIfZero(2))
				{
					col = 65 * 8;
					num = string.Format("{0:#,##0.00}", doc.fat_abbuoni);
					str = str + $"^FO{col},{row + 27 * 8}" + "^A0,N,23,23" + $"^FB{14 * 8},1,0,R,0^FD{num}^FS";
				}

				col = 81 * 8;
				num = string.Format("{0:#,##0.00}", doc.fat_tot_pagare);
				str = str + $"^FO{col},{row + 27 * 8}" + "^A0,N,23,23" + $"^FB{16 * 8},1,0,R,0^FD{num}^FS";


				//
				// Scriviamo il Totale
				//
				col = 66 * 8;
				str = str + $"^FO{col},{row + 10 * 8}" + "^A0,N,25,25" + $"^FB{34 * 8},1,0,C,0^FDTOTALE DOCUMENTO ^FS";

				col = 66 * 8;
				num = string.Format("{0:#,##0.00}", doc.fat_tot_fattura);
				str = str + $"^FO{col},{row + 16 * 8}" + "^A0,N,35,35" + $"^FB{34 * 8},1,0,C,0^FD{num}^FS";
			}
			else  // DDT
			{
				Trasporti tra = null;
				if (doc.fat_tra != 0) tra = await dbcon_.GetAsync<Trasporti>(doc.fat_tra);

				col = 1 * 8;
				str = str + $"^FO{col},{row + 1 * 8}" + "^A0,N,19,19" + $"^FDCausale Trasporto^FS";

				col = 84 * 8;
				str = str + $"^FO{col},{row + 1 * 8}" + "^A0,N,19,19" + $"^FDColli^FS";

				col = 0;
				str = str + $"^FO{col},{row + 7 * 8}" + $"^GB800,1,2^FS";

				col = 83 * 8;
				str = str + $"^FO{col},{row}" + $"^GB1,{7 * 8},2^FS";

				col = 1 * 8;
				str = str + $"^FO{col},{row + 8 * 8}" + "^A0,N,19,19" + $"^FDAnnotazioni^FS";

				col = 2 * 8;
				str = str + $"^FO{col},{row + 4 * 8}" + "^A0,N,23,23" + $"^FD{tra.tra_desc}^FS";

				col = 84 * 8;
				num = string.Format("{0,5:#######}", doc.fat_colli);
				str = str + $"^FO{col},{row + 4 * 8}" + "^A0,N,23,23" + $"^FD{num}^FS";

				col = 2 * 8;
				str = str + $"^FO{col},{row + 11 * 8}" + "^A0,N,23,23" + $"^FD{doc.fat_annotaz}^FS";
				str = str + $"^FO{col},{row + 14 * 8}" + "^A0,N,23,23" + $"^FD{doc.fat_desc_varie_0}^FS";
				str = str + $"^FO{col},{row + 17 * 8}" + "^A0,N,23,23" + $"^FD{doc.fat_desc_varie_1}^FS";
				str = str + $"^FO{col},{row + 20 * 8}" + "^A0,N,23,23" + $"^FD{doc.fat_desc_varie_2}^FS";
				str = str + $"^FO{col},{row + 23 * 8}" + "^A0,N,23,23" + $"^FD{doc.fat_desc_varie_3}^FS";

				// VEN_CARICO    4
				// VEN_RIMANENZA 5
				if ((doc.fat_tipo_ven != 4) && (doc.fat_tipo_ven != 5))
				{
					col = 64 * 8;
					str = str + $"^FO{col},{row + 27 * 8}" + "^A0,N,23,23" + $"^FDSEGUE REGOLARE FATTURA^FS";
				}
			}

			if ((doc.fat_tipo == (int)TipoDocumento. TIPO_FAT) || (doc.fat_tipo == (int)TipoDocumento.TIPO_DDT))
			{
				if (pag.pag_nota_alimentari)
				{
					col = 20 * 8;
					str = str + $"^FO{col},{row + 44 * 8}" + "^A0,N,19,19" + $"^FDAssolve agli obblighi di cui all' art. 62 co. 1 D.L 24/1/2012 n. 1^FS";

					col = 23 * 8;
					str = str + $"^FO{col},{row + 47 * 8}" + "^A0,N,19,19" + $"^FDconvertito con modificazioni dalla legge 24/3/2012 n. 27^FS";

					col = 20 * 8;
					if (doc.fat_tipo == (int)TipoDocumento.TIPO_FAT)
						str = str + $"^FO{col},{row + 50 * 8}" + "^A0,N,19,19" + $"^FDLa durata del rapporto commerciale si riferisce alla singola fattura^FS";
					else
						str = str + $"^FO{col},{row + 50 * 8}" + "^A0,N,19,19" + $"^FDLa durata del rapporto commerciale si riferisce al singolo documento^FS";
				}
			}

			str = str + "^XZ";
			var t = new UTF8Encoding().GetBytes(str);
			_con.Write(t);


			str = "^XA" +           // Inizializziamo la stampa
				"^PW800" +          // Settiamo la Larghezza
				"^MNN" +            // Settiamo la stampa in continuos mode
				"^LH0,0" +          // Settiamo la posizione Iniziale
				"^LL80" +           // Settiamo la posizione Iniziale
				"^XZ";

			str = str + "^XZ";
			t = new UTF8Encoding().GetBytes(str);
			_con.Write(t);

			return (row);
		}

		int CalcLength(bool stprice)
		{
			int len = 82;	// Dimensione Header
			len += 55;      // Dimensione Footer

			len += 1; // Millimetro lasciato prima della prima riga del body
			if (!string.IsNullOrWhiteSpace(_codocr)) len += 1 + 5;

			if (stprice)
			{
				foreach(var rig in _riglist)
				{
					len += 3;   // Prima riga di descrizione
					len += 4;   // Riga prezzi e quantita
					if (!string.IsNullOrWhiteSpace(rig.rig_lotto)) len += 3;

					rig.rig_newdes = rig.rig_newdes.Replace("\n\r", "\n");
					rig.rig_newdes = rig.rig_newdes.Replace("\r\n", "\n");

					var desarr = rig.rig_newdes.Split('\n');
					len += 3 * (desarr.Length - 1);
				}
			}

			return (len * 8);
		}

		public async Task<bool> PrintDoc(Fatture doc, short copie = 1)
		{
			var app = (App)Application.Current;
			var result = false;
			bool stprice = true;
			bool rewrite = false;

		
			if (app.printer == null)
			{
				await _parent.DisplayAlert("Attenzione...", "Non è stata selezionata alcuna stampante!", "OK");
				await _parent.Navigation.PushAsync(new SetupPage());
				return (false);
			}

			//
			// Ricalcolare il documento
			//


			if (doc.fat_tipo == (int)TipoDocumento.TIPO_DDT)
			{
				stprice = await _parent.DisplayAlert("Facile", "Vuoi Stampare i Prezzi ?", "SI", "NO");
			}

			try
			{
				_con = app.printer.Connection;
				_con.Open();
				_prn = ZebraPrinterFactory.Current.GetInstance(_con);


				//
				// Leggiamo le righe del documento
				//
				string sql = String.Format("SELECT * FROM fatrow2 WHERE rig_tipo = {0} AND rig_n_doc = {1}", doc.fat_tipo, doc.fat_n_doc);
				_riglist = await dbcon_.QueryAsync<FatRow>(sql);

				//
				// Controlliamo che tutte le righe siano dello stesso fornitore
				//
				int codfor = -1;
				if (doc.fat_tipo == (int)TipoDocumento.TIPO_DDT)
				{
					foreach(var rig in _riglist)
					{
						if (!string.IsNullOrWhiteSpace(rig.rig_art))
						{
							sql = String.Format("SELECT * FROM artanag WHERE ana_codice = {0} LIMIT 1", rig.rig_art.Trim().SqlQuote(false));
							var ana = await dbcon_.QueryAsync<Artanag>(sql);
							if (ana.Count > 0)
							{
								if (codfor == -1) codfor = ana[0].ana_for_abituale;
								if (codfor != ana[0].ana_for_abituale)
								{
									codfor = -1;
									break;
								}
							}
						}
					}
					if (codfor == -1) codfor = 0;


					_codcli = "C.F. ";
					_coddst = "C.C. ";
					sql = String.Format("SELECT * FROM agganci WHERE agg_forn = {0} AND agg_cli = {1} AND agg_dst {2} LIMIT 1", codfor, doc.fat_inte, 0);
					var agg = await dbcon_.QueryAsync<Agganci>(sql);
					if (agg.Count > 0)
					{
						_codcli = _codcli + agg[0].agg_codice;
						_coddst = _coddst + agg[0].agg_codice;
					}
					sql = String.Format("SELECT * FROM agganci WHERE agg_forn = {0} AND agg_cli = {1} AND agg_dst {2} LIMIT 1", codfor, doc.fat_inte, doc.fat_dest);
					agg = await dbcon_.QueryAsync<Agganci>(sql);
					if (agg.Count > 0)
					{
						_coddst = "C.C. " + agg[0].agg_codice;
					}
					_codcli = _codcli.Trim();
					_coddst = _coddst.Trim();
					if (_codcli == "C.F.") _codcli = "";
					if (_coddst == "C.C.") _coddst = "";

					_codocr = "";
					if (doc.fat_tipo_ven == (int)TipoVendita.VEN_TRASFERT && codfor != 0) 
					{
						sql = String.Format("SELECT * FROM fornito1 WHERE for_codice = {0} LIMIT 1", codfor);
						var forList = await dbcon_.QueryAsync<Fornitori>(sql);
						if (forList.Count > 0 && forList[0].for_nota != 0)
						{
							sql = String.Format("SELECT * FROM descriz1 WHERE des_codice = {0} LIMIT 1", forList[0].for_nota);
							var desList = await dbcon_.QueryAsync<Descrizioni>(sql);
							if (desList.Count > 0)
							{
								desList[0].des_newdes = desList[0].des_newdes.Trim();
								doc.fat_n_from = forList[0].for_codice;
								if (desList[0].des_newdes.Length < 130)
									doc.fat_annotaz = desList[0].des_newdes;
								else
								{
									doc.fat_annotaz = desList[0].des_newdes.Substring(0, 129);
									if (desList[0].des_newdes.Length > 130) doc.fat_desc_varie_0 = desList[0].des_newdes.Substring(130, desList[0].des_newdes.Length >= 260 ? 259 : desList[0].des_newdes.Length - 1);
									if (desList[0].des_newdes.Length > 260) doc.fat_desc_varie_1 = desList[0].des_newdes.Substring(260, desList[0].des_newdes.Length >= 390 ? 389 : desList[0].des_newdes.Length - 1);
									if (desList[0].des_newdes.Length > 390) doc.fat_desc_varie_2 = desList[0].des_newdes.Substring(390, desList[0].des_newdes.Length - 1);
								}
								doc.fat_annotaz = doc.fat_annotaz.Trim();
								doc.fat_desc_varie_0 = doc.fat_desc_varie_0.Trim();
								doc.fat_desc_varie_1 = doc.fat_desc_varie_1.Trim();
								doc.fat_desc_varie_2 = doc.fat_desc_varie_2.Trim();
								rewrite = true;
							}
						}
						if (codfor == 1L) // NESTLE
						{
							int ocr = 0;
							_codocr = string.Format("<{0}{1}{2:00}", RsaUtils.GetShowedNumDoc(doc.fat_n_doc), doc.fat_registro, 1);
							for (int y = 1; y < _codocr.Length; y++)
							{
								ocr += _codocr[y]*y;
							}
							ocr = ocr % 93;
							_codocr = _codocr + $"{ocr:00}>";
						}
					}			
				}

				if (rewrite) await dbcon_.UpdateAsync(doc);

				Initialize();
				_mod_len = CalcLength(stprice) > _mod_len ? CalcLength(stprice) : _mod_len;
				for (short idx = 0; idx < copie; idx++)
				{
					int row = 0;

					if (idx != 0) PrePrintCheckStatus();

					row = await PrintDocHeaderAsync(doc, row, stprice);
					row = await PrintDocBodyAsync(doc, row, stprice);
					row = await PrintDocFooterAsync(doc, row, stprice);
				}
				result = true;
			}
			catch (ZebraExceptions ex)
			{
				await _parent.DisplayAlert("Errore!", ex.Message, "OK");
			}
			catch (Exception ex)
			{
				// Connection Exceptions and issues are caught here
				Debug.WriteLine(ex.Message);
				await _parent.DisplayAlert("Errore!", ex.Message, "OK");
			}
			finally
			{
				_con.Open();
				if ((_con != null) && (_con.IsConnected)) _con.Close();
				_con = null;
				_prn = null;
			}

			return(result);
		}


		protected void SetZplPrinterLanguage()
		{
			if (!_con.IsConnected) _con.Open();


			//  Check the current printer language
			byte[] response = _con.SendAndWaitForResponse(new UTF8Encoding().GetBytes("! U1 getvar \"device.languages\"\r\n"), 500, 100);
			string language = Encoding.UTF8.GetString(response, 0, response.Length);
			if (language.Contains("line_print"))
			{
				Debug.WriteLine("Switching printer to ZPL Control Language.", "Notification");
			}
			// printer is already in zpl mode
			else if (language.Contains("zpl"))
			{
				return;
			}

			//  Set the printer command languege
			_con.Write(new UTF8Encoding().GetBytes("! U1 setvar \"device.languages\" \"zpl\"\r\n"));
			response = _con.SendAndWaitForResponse(new UTF8Encoding().GetBytes("! U1 getvar \"device.languages\"\r\n"), 500, 100);
			language = Encoding.UTF8.GetString(response, 0, response.Length);
			if (!language.Contains("zpl"))
			{
				Debug.WriteLine("Printer language not set. Not a ZPL printer.");
				throw new ZebraExceptions(ZebraExceptions.MsgNoZplPrinter, ZebraExceptions.ErrNoZplPrinter);
			}
			return;
		}

		protected void PrePrintCheckStatus()
		{
			// Check the printer status
			IPrinterStatus status = _prn.CurrentStatus;
			if (!status.IsReadyToPrint)
			{
				Debug.WriteLine("Unable to print. Printer is " + status.Status);
				throw new ZebraExceptions(ZebraExceptions.MsgUnableToPrint + " " + status.Status, ZebraExceptions.ErrUnableToPrint);
			}
		}

		//Check what happens to the printer after print command was sent
		protected void PostPrintCheckStatus()
		{
			// Check the status again to verify print happened successfully
			IPrinterStatus status = _prn.CurrentStatus;

			// Wait while the printer is printing
			while ((status.NumberOfFormatsInReceiveBuffer > 0) && (status.IsReadyToPrint))
			{
				status = _prn.CurrentStatus;
			}
			// verify the print didn't have errors like running out of paper
			if (!status.IsReadyToPrint)
			{
				Debug.WriteLine("Error durring print. Printer is " + status.Status);
				throw new ZebraExceptions(ZebraExceptions.MsgDuringPrint + " " + status.Status, ZebraExceptions.ErrDuringPrint);
			}
		}



	}
}

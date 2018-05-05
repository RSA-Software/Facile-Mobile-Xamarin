using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Facile.ExportModels;
using Facile.Interfaces;
using Facile.Models;
using Facile.Utils;
using Newtonsoft.Json;
using PCLStorage;
using SQLite;
using Syncfusion.SfBusyIndicator.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UploadPage : ContentPage
	{
		protected bool first;
		private LocalImpo lim;
		private readonly SQLiteAsyncConnection dbcon_;

		public UploadPage()
		{
			first  = true;
			lim    = null;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			InitializeComponent();
		}

		protected async override void OnAppearing()
		{
			if (first)
			{
				var response = await DisplayAlert("Facile", "L'invio dei documenti richiede una connessione internet e potrebbero essere necessari diversi minuti.\n\nVuoi proseguire?", "Si", "No");
				if (response)
				{
					try
					{
						await Upload();
					}
					catch (Exception ex)
					{
						await DisplayAlert("Errore", ex.Message, "OK");
						await Navigation.PopModalAsync();
					}
				}
				else
					await Navigation.PopModalAsync();
			}
			first = false;
		}


		private async Task Upload()
		{
			int cli_rec = 0;
			int dst_rec = 0;
			int rig_rec = 0;

			//
			// Leggiamo le impostazioni
			//
			try
			{
				lim = await dbcon_.GetAsync<LocalImpo>(1);
			}
			catch
			{
				await DisplayAlert("Attenzione!", "Impostazioni locali non trovate!\nRiavviare l'App.", "OK");
				await Navigation.PopModalAsync();
				return;
			}

			if (string.IsNullOrWhiteSpace(lim.ftpServer))
			{
				await DisplayAlert("Attenzione!", "Server non impostato o non valido.", "OK");
				await Navigation.PopModalAsync();
				return;
			}
			if (string.IsNullOrWhiteSpace(lim.user))
			{
				await DisplayAlert("Attenzione!", "Utente non impostato o non valido.", "OK");
				await Navigation.PopModalAsync();
				return;
			}
			if (lim.age == 0)
			{
				await DisplayAlert("Attenzione!", "Agente non impostato o non valido.", "OK");
				await Navigation.PopModalAsync();
				return;
			}

			busyIndicator.IsBusy = true;
			busyIndicator.AnimationType = AnimationTypes.Ball;
			busyIndicator.Title = "Estrazione Documenti";

			var docList = new List<Documento>();


			m_doc_ext.Source = "ic_hourglass_full_white.png";
			m_rig_ext.Source = "ic_hourglass_full_white.png";
			m_cli_ext.Source = "ic_hourglass_full_white.png";
			m_dst_ext.Source = "ic_hourglass_full_white.png";


			m_doc.FontSize = m_doc.FontSize + 3;
			m_doc.TextColor = Color.Red;

			var sql = "SELECT * FROM fatture2 WHERE fat_local_doc = 1";
			var fatList = await dbcon_.QueryAsync<Fatture>(sql);
			if (fatList.Count == 0)
			{
				busyIndicator.IsBusy = false;
				await DisplayAlert("Attenzione!", "Non ci sono documenti da inviare alla sede.", "OK");
				await Navigation.PopModalAsync();
				return;
			}

			m_doc_rec.Text = $"{fatList.Count}";
			m_doc.FontSize = m_doc.FontSize - 3;
			m_doc.TextColor = Color.White;

			busyIndicator.AnimationType = AnimationTypes.Gear;
			busyIndicator.Title = "Estrazione Dati Documenti";
			foreach (var fat in fatList)
			{
				var doc = new Documento();
				doc.documento = fat;

				// 
				// Inseriamo i dati del cliente
				//
				m_cli.FontSize = m_cli.FontSize + 3;
				m_cli.TextColor = Color.Red;
				try
				{
					cli_rec++;
					doc.cliente = await dbcon_.GetAsync<Clienti>(fat.fat_inte);
				}
				catch
				{
					throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
				}
				m_cli_rec.Text = $"{cli_rec}";
				m_cli.FontSize = m_cli.FontSize - 3;
				m_cli.TextColor = Color.White;


				//
				// Inseriamo i dati del destinatario
				//
				if (fat.fat_dest != 0)
				{
					m_dst.FontSize = m_dst.FontSize + 3;
					m_dst.TextColor = Color.Red;

					try
					{
						dst_rec++;
						doc.destinazione = await dbcon_.GetAsync<Destinazioni>(fat.fat_dest);
					}
					catch
					{
						throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
					}
					m_dst.FontSize = m_dst.FontSize - 3;
					m_dst.TextColor = Color.White;
				}
				m_dst_rec.Text = $"{dst_rec}";

				//
				// Inseriamo le righe dei documenti
				//
				m_rig.FontSize = m_rig.FontSize + 3;
				m_rig.TextColor = Color.Red;

				sql = string.Format("SELECT * from fatrow2 WHERE rig_tipo = {0} AND rig_n_doc = {1} ORDER BY rig_tipo, rig_n_doc, rig_d_ins, rig_t_ins", fat.fat_tipo, fat.fat_n_doc);
				doc.righe = await dbcon_.QueryAsync<FatRow>(sql);

				m_rig.FontSize = m_rig.FontSize - 3;
				m_rig.TextColor = Color.White;

				rig_rec += doc.righe.Count;
				m_rig_rec.Text = $"{rig_rec}";

				if (doc.righe.Count == 0)
				{
					sql = "";
					busyIndicator.IsBusy = true;
					switch(fat.fat_tipo)
					{
						case (short)DocTipo.TIPO_FAT :
							sql = string.Format("La fattura N. {0}/{1} non contiene righe!\n\nVuoi continuare?", RsaUtils.GetShowedNumDoc(fat.fat_n_doc), fat.fat_registro);
							break;

						case (short)DocTipo.TIPO_DDT:
							sql = string.Format("Il DDT N. {0}/{1} non contiene righe!\n\nVuoi continuare?", RsaUtils.GetShowedNumDoc(fat.fat_n_doc), fat.fat_registro);
							break;

						case (short)DocTipo.TIPO_ORD:
							sql = string.Format("L' Ordine N. {0}/{1} non contiene righe!\n\nVuoi continuare?", RsaUtils.GetShowedNumDoc(fat.fat_n_doc), fat.fat_registro);
							break;

						default :
							sql = string.Format("Il Documento N. {0}/{1} (Tipo - {2}) non contiene righe!\n\nVuoi continuare?", RsaUtils.GetShowedNumDoc(fat.fat_n_doc), fat.fat_registro, fat.fat_tipo);
							break;
					}
					var test = await DisplayAlert("Attenzone", sql, "SI", "NO");
					if (!test) 
					{
						await Navigation.PopModalAsync();
						return;
					}
					
				}

				//
				// Aggiungiamo il documento alla lista di quelli esportati
				//
				docList.Add(doc);
			}
			m_doc_ext.Source = "ic_storage_black.png";
			m_rig_ext.Source = "ic_storage_black.png";
			m_cli_ext.Source = "ic_storage_black.png";
			m_dst_ext.Source = "ic_storage_black.png";

			//
			// Creiamo il file
			//
			m_doc_json.Source = "ic_hourglass_full_white.png";
			m_rig_json.Source = "ic_hourglass_full_white.png";
			m_cli_json.Source = "ic_hourglass_full_white.png";
			m_dst_json.Source = "ic_hourglass_full_white.png";

			busyIndicator.AnimationType = AnimationTypes.DoubleCircle;
			busyIndicator.Title = "Serializzazione";

			//sql = JsonConvert.SerializeObject(docList, Formatting.Indented);

			IFolder rootFolder = FileSystem.Current.LocalStorage;
			String localJson = rootFolder.Path + "/" + "DOCUMENTI" + ".JSON";

			IFile json_file = await rootFolder.CreateFileAsync(localJson, CreationCollisionOption.ReplaceExisting);
			await json_file.WriteAllTextAsync(JsonConvert.SerializeObject(docList, Formatting.Indented));

			m_doc_json.Source = "ic_code_black.png";
			m_rig_json.Source = "ic_code_black.png";
			m_cli_json.Source = "ic_code_black.png";
			m_dst_json.Source = "ic_code_black.png";

			m_doc_upload.Source = "ic_file_download_black.png";
			m_rig_upload.Source = "ic_file_download_black.png";
			m_cli_upload.Source = "ic_file_download_black.png";
			m_dst_upload.Source = "ic_file_download_black.png";

			//
			// Trasmettiamo il file
			// 
			busyIndicator.AnimationType = AnimationTypes.Globe;
			busyIndicator.Title = "Trasmissione";

			string password = "";
			string remoteServer = "";

			if (lim.ftpServer == "Facile - 01")
				remoteServer = "ftp://www.facile2013.it";

			if (lim.ftpServer == "Facile - 02")
				remoteServer = "ftp://www.rsaweb.com";

			if (lim.ftpServer == "Facile - 03")
				remoteServer = "ftp://www.facilecloud.com";

			if (remoteServer == "")
				throw new Exception("Server non impostato o non valido");

			if (lim.user != "demo2017")
				password = $"$_{lim.user}_$";
			else
				password = lim.user;

			var remotePath = $"/{lim.age}/out";

			m_doc_upload.Source = "ic_hourglass_full_white.png";
			m_rig_upload.Source = "ic_hourglass_full_white.png";
			m_cli_upload.Source = "ic_hourglass_full_white.png";
			m_dst_upload.Source = "ic_hourglass_full_white.png";

			var ftp = DependencyService.Get<IFtpWebRequest>();
			string result = await ftp.UploadFile(remoteServer, localJson, lim.user, password, remotePath);
			busyIndicator.IsBusy = false;
			if (result.StartsWith("221", StringComparison.CurrentCulture))
			{
				m_doc_upload.Source = "ic_cloud_black.png";
				m_rig_upload.Source = "ic_cloud_black.png";
				m_cli_upload.Source = "ic_cloud_black.png";
				m_dst_upload.Source = "ic_cloud_black.png";


				m_doc.TextColor = Color.Black;
				m_rig.TextColor = Color.Black;
				m_cli.TextColor = Color.Black;
				m_dst.TextColor = Color.Black;

				//
				// Marchiamo i documenti come non più editabili
				//
				foreach (var fat in fatList)
				{
					fat.fat_editable = false;
				}
				await dbcon_.UpdateAllAsync(fatList);
				await DisplayAlert("Facile", "Invio documenti concluso con successo!", "OK");
			}
			else if (result.StartsWith("530", StringComparison.CurrentCulture))
			{
				await DisplayAlert("Facile", "Parametri di Login non validi!\nVerificare il nome utente configurato.", "OK");
			}
			else if (result.StartsWith("System.Net.WebException", StringComparison.CurrentCulture))
			{
				await DisplayAlert("Facile", result, "OK");
			}
			else
			{
				await DisplayAlert("Facile", "Impossibile caricare il file sul server!", "OK");
			}

			//
			// Rimuoviamo il file
			//
			await json_file.DeleteAsync();
			await Navigation.PopModalAsync();
		}
	}
}

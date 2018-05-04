using System;
using System.Collections.Generic;
using System.IO;
using Facile.ExportModels;
using Facile.Interfaces;
using Facile.Models;
using Facile.Utils;
using Newtonsoft.Json;
using PCLStorage;
using SQLite;
using Syncfusion.SfBusyIndicator.XForms;
using Xamarin.Forms;

namespace Facile
{
	public partial class UploadPage : ContentPage
	{
		private LocalImpo lim;
		private readonly SQLiteAsyncConnection dbcon_;

		public UploadPage()
		{
			lim = null;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			InitializeComponent();
		}

		async void OnInviaClicked(object sender, System.EventArgs e)
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
				return;
			}

			if (string.IsNullOrWhiteSpace(lim.ftpServer))
			{
				await DisplayAlert("Attenzione!", "Server non impostato o non valido.", "OK");
				return;
			}
			if (string.IsNullOrWhiteSpace(lim.user))
			{
				await DisplayAlert("Attenzione!", "Utente non impostato o non valido.", "OK");
				return;
			}
			if (lim.age == 0)
			{
				await DisplayAlert("Attenzione!", "Agente non impostato o non valido.", "OK");
				return;
			}

			busyIndicator.IsBusy = true;
			busyIndicator.AnimationType = AnimationTypes.Ball;
			busyIndicator.Title = "Estrazione Documenti";

			var docList = new List<Documento>();

			var sql = "SELECT * FROM fatture2 WHERE fat_local_doc = 1";
			var fatList = await dbcon_.QueryAsync<Fatture>(sql);
			if (fatList.Count == 0)
			{
				busyIndicator.IsBusy = false;
				await DisplayAlert("Attenzione!", "Non ci sono documenti da inviare alla sede.", "OK");
				return;
			}

			m_doc_rec.Text = $"{fatList.Count}";


			busyIndicator.AnimationType = AnimationTypes.Gear;
			busyIndicator.Title = "Estrazione Dati Documenti";
			foreach (var fat in fatList)
			{
				var doc = new Documento();
				doc.documento = fat;

				// 
				// Inseriamo i dati del cliente
				//
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


				//
				// Inseriamo i dati del destinatario
				//
				if (fat.fat_dest != 0)
				{
					try
					{
						dst_rec++;
						doc.destinazione = await dbcon_.GetAsync<Destinazioni>(fat.fat_dest);
					}
					catch
					{
						throw new RsaException(RsaException.NotFoundMsg, RsaException.NotFoundErr);
					}
				}
				m_dst_rec.Text = $"{dst_rec}";

				//
				// Inseriamo le righe dei documenti
				//
				sql = string.Format("SELECT * from fatrow2 WHERE rig_tipo = {0} AND rig_n_doc = {1}", fat.fat_tipo, fat.fat_n_doc);
				doc.righe = await dbcon_.QueryAsync<FatRow>(sql);

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
					if (!test) return;
					
				}

				//
				// Aggiungiamo il documento alla lista di quelli esportati
				//
				docList.Add(doc);
			}

			//
			// Creiamo il file
			//
			busyIndicator.AnimationType = AnimationTypes.DoubleCircle;
			busyIndicator.Title = "Serializzazione";

			//sql = JsonConvert.SerializeObject(docList, Formatting.Indented);

			IFolder rootFolder = FileSystem.Current.LocalStorage;
			String localJson = rootFolder.Path + "/" + "DOCUMENTI" + ".JSON";

			IFile json_file = await rootFolder.CreateFileAsync(localJson, CreationCollisionOption.ReplaceExisting);
			await json_file.WriteAllTextAsync(JsonConvert.SerializeObject(docList, Formatting.Indented));

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


				//
				// Marchiamo i documenti come non più editabili
				//
				foreach (var fat in fatList)
				{
					fat.fat_editable = false;
				}
				await dbcon_.UpdateAllAsync(fatList);

				//
				// Rimuoviamo il file
				//
				await json_file.DeleteAsync();
				await DisplayAlert("Facile", "Invio documenti concluso con successo!", "OK");
			}
			else if (result.StartsWith("530", StringComparison.CurrentCulture))
			{
				//
				// Rimuoviamo il file
				//
				await json_file.DeleteAsync();
				await DisplayAlert("Facile", "Parametri di Login non validi!\nVerificare il nome utente configurato.", "OK");
			}
			else if (result.StartsWith("System.Net.WebException", StringComparison.CurrentCulture))
			{
				//
				// Rimuoviamo il file
				//
				await json_file.DeleteAsync();
				await DisplayAlert("Facile", result, "OK");
			}
			else
			{
				//
				// Rimuoviamo il file
				//
				await json_file.DeleteAsync();
				await DisplayAlert("Facile", "Impossibile caricare il file sul server!", "OK");
			}
		}
	}
}

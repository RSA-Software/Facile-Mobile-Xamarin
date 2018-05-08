using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facile.Extension;
using Facile.Interfaces;
using Facile.Models;
using PCLStorage;
using SQLite;
using Xamarin.Forms;

namespace Facile
{
	public partial class DownloadImages : ContentPage
	{
		protected bool first;
		private LocalImpo lim;
		private readonly SQLiteAsyncConnection dbcon_;

		public DownloadImages()
		{
			first = true;
			lim = null;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			if (first)
			{
				var response = await DisplayAlert("Facile", "La ricezione dati potrebbe richiede una connessione internet e potrebbero essere necessari diversi minuti.\n\nVuoi proseguire?", "Si", "No");
				if (response)
					await Download();
				else
					await Navigation.PopModalAsync();
			}
			first = false;
		}

		private async Task Download()
		{
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

			string password = "";
			string remoteServer = "";

			if (lim.ftpServer == "Facile - 01")
				remoteServer = "ftp://www.facile2013.it/images/";

			if (lim.ftpServer == "Facile - 02")
				remoteServer = "ftp://www.rsaweb.com/images/";

			if (lim.ftpServer == "Facile - 03")
				remoteServer = "ftp://www.facilecloud.com/images/";

			if (remoteServer == "")
				throw new Exception("Server non impostato o non valido");

			if (lim.user != "demo2017")
				password = $"$_{lim.user}_$";
			else
				password = lim.user;
			
			try
			{
				IFolder rootFolder = FileSystem.Current.LocalStorage;
				IFolder imagesFolder = await rootFolder.CreateFolderAsync("images", CreationCollisionOption.OpenIfExists);

				m_desc.Text = "Otteniamo la lista dei files... ";
				var ftp = DependencyService.Get<IFtpWebRequest>();
				var files = await ftp.ListDirectory(lim.user, password, remoteServer);

				if (files != null && files.Count > 0)
				{
					int idx = 0;
					bool stop = false;

					foreach (var file in files)
					{
						String remoteFile = remoteServer + file;
						String localFile = imagesFolder.Path + "/" + file;

						if (stop) break;

						bool skip = true;
						var x = file.LastIndexOf('.');

						if (x >= 0)
						{
							var ext = file.Substring(x);
							if (ext.ToUpper() == ".JPG" || ext.ToUpper() == ".PNG" || ext.ToUpper() == ".PRN") skip = false;


							//
							// Controllare qui se si vuole la presenza dell'articolo
							//
							if (ext.ToUpper() == ".JPG" || ext.ToUpper() == ".PNG" )
							{
								x = file.LastIndexOf('_');
								if (x > 0)
								{
									var code = file.Substring(0, x).Trim().ToUpper();
									var sql = string.Format("SELECT COUNT(*) FROM artanag WHERE ana_codice = {0}", code.SqlQuote(false));
									var rec = await dbcon_.ExecuteScalarAsync<int>(sql);
									if (rec == 0) skip = true;
								}
							}
						}
						idx++;
						if (skip)
							m_desc.Text = string.Format("Salto {0} di {1} - {2}", idx, files.Count, file);
						else
						{
							m_desc.Text = string.Format("Scarico {0} di {1} - {2}", idx, files.Count, file);
							var result = await ftp.DownloadFile(lim.user, password, remoteFile, localFile);
							if (!result.StartsWith("221", StringComparison.CurrentCulture))
							{
								for (int retry = 0; retry < 5; retry++)
								{
									result = await ftp.DownloadFile(lim.user, password, remoteFile, localFile);
									if (result.StartsWith("221", StringComparison.CurrentCulture)) 
										break;
									else
									{
										if (idx == 4)
										{
											var str = "Impossibile scaricare il file : " + file + "\n\nVuoi continuare?";
											stop = !(await DisplayAlert("Errore", str, "SI", "NO"));
										}
									}
								}
							}
							m_image.Source = localFile;
						}
					}
				}
				m_desc.Text = "";
				await DisplayAlert("Facile", "Importazione dati conclusa!", "Ok");
			}
			catch (Exception ex)
			{
				await DisplayAlert("Download Error", ex.Message, "OK");
			}
			await Navigation.PopModalAsync();
		}


	}
}

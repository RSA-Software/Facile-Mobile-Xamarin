using System;
using System.Threading.Tasks;
using Facile.Extension;
using Facile.Interfaces;
using Facile.Models;
using PCLStorage;
using SQLite;
using Xamarin.Forms;
using Facile.Utils;

namespace Facile
{
	public partial class DownloadImages : ContentPage
	{
		protected bool first_;
		private int _max_download;
		private LocalImpo lim;
		private readonly SQLiteAsyncConnection dbcon_;

		public DownloadImages(int start_index, int stop_index)
		{
			first_ = true;
			lim = null;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			InitializeComponent();

			Random randomImages = new Random();
			if (Device.RuntimePlatform == Device.Android)
				_max_download = randomImages.Next(400,1000);
			else
				_max_download = randomImages.Next(250,400);
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			if (first_)
			{
				var response = await DisplayAlert("Facile", "La ricezione dati potrebbe richiede una connessione internet e potrebbero essere necessari diversi minuti.\n\nVuoi proseguire?", "Si", "No");
				if (response)
					await Download();
				else
					await Navigation.PopModalAsync();
			}
			first_ = false;
		}

		private async Task Download()
		{
			int num_download = 0;

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
						String remoteFile = remoteServer + file.Name;
						String localFile = imagesFolder.Path + "/" + file.Name;

						if (stop) break;

						if (num_download > _max_download)
						{
							throw new RsaException(RsaException.ImagesMsg, RsaException.ImageErr); 
						}

						bool skip = true;
						var x = file.Name.LastIndexOf('.');

						if (x >= 0)
						{
							var ext = file.Name.Substring(x);
							if (ext.ToUpper() == ".JPG" || ext.ToUpper() == ".PNG" || ext.ToUpper() == ".PRN") skip = false;

							//
							// Controllare qui se si vuole la presenza dell'articolo
							//
							if (ext.ToUpper() == ".JPG" || ext.ToUpper() == ".PNG")
							{
								x = file.Name.LastIndexOf('_');
								if (x > 0)
								{
									var code = file.Name.Substring(0, x).Trim().ToUpper();
									var sql = string.Format("SELECT COUNT(*) FROM artanag WHERE ana_codice = {0}", code.SqlQuote(false));
									var rec = await dbcon_.ExecuteScalarAsync<int>(sql);
									if (rec == 0) skip = true;
								}

								//
								// Controlliamo il timestamp
								//
								if (!skip && file.LastUpdate.HasValue)
								{
									try
									{
										var img = await dbcon_.GetAsync<Images>(file.Name.ToUpper());
										if (img.img_last_update.HasValue) 
										{
											if (DateTime.Compare(file.LastUpdate.Value, img.img_last_update.Value) <= 0)
											{
												skip = true;
											}
										}
									}
									catch
									{
										
									}
								}
							}
						}
						idx++;

						if (skip)
							m_desc.Text = string.Format("Salto {0} di {1} - {2}", idx, files.Count, file.Name);
						else
						{
							m_desc.Text = string.Format("Scarico {0} di {1} - {2}", idx, files.Count, file.Name);
							var result = await ftp.DownloadFile(lim.user, password, remoteFile, localFile);
							if (!result.StartsWith("221", StringComparison.CurrentCulture))
							{
								for (int retry = 0; retry < 5; retry++)
								{
									result = await ftp.DownloadFile(lim.user, password, remoteFile, localFile);
									if (result.StartsWith("221", StringComparison.CurrentCulture))
									{
										break;
									}
									else
									{
										if (idx == 4)
										{
											var str = "Impossibile scaricare il file : " + file.Name + "\n\nVuoi continuare?";
											stop = !(await DisplayAlert("Errore", str, "SI", "NO"));
										}
									}
								}
							}
							num_download++;
							Images image = new Images
							{
								img_name = file.Name.ToUpper(),
								img_last_update = file.LastUpdate
							};
							await dbcon_.InsertOrReplaceAsync(image);
							m_image.Source = localFile;
						}
					}
				}
				m_desc.Text = "";
				await DisplayAlert("Facile", "Importazione dati conclusa!", "Ok");
			}
			catch (RsaException ex)
			{
				await DisplayAlert("Download Error", ex.Message, "OK");
			}
			catch (Exception ex)
			{
				await DisplayAlert("Download Error", ex.Message, "OK");
			}
			await Navigation.PopModalAsync();
		}


	}
}

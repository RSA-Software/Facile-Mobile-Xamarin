using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facile.Interfaces;
using PCLStorage;
using Xamarin.Forms;

namespace Facile
{
	public partial class DownloadImages : ContentPage
	{
		protected bool first;
		public DownloadImages()
		{
			first = true;
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
			try
			{
				String remoteServer = "ftp://www.facile2013.it/images/";
				IFolder rootFolder = FileSystem.Current.LocalStorage;
				IFolder imagesFolder = await rootFolder.CreateFolderAsync("images", CreationCollisionOption.OpenIfExists);

				m_desc.Text = "Otteniamo la lista dei files... ";
				var ftp = DependencyService.Get<IFtpWebRequest>();
				var files = await ftp.ListDirectory("demo2017", "demo2017", remoteServer);

				if (files != null && files.Count > 0)
				{
					int idx = 0;
					foreach (var file in files)
					{
						String remoteFile = remoteServer + file;
						String localFile = imagesFolder.Path + "/" + file;

						idx++;
						m_desc.Text = string.Format("Scarico {0} di {1} - {2}", idx ,files.Count, file);

						var result = await ftp.DownloadFile("demo2017", "demo2017", remoteFile, localFile);
						if (!result.StartsWith("221", StringComparison.CurrentCulture))
						{
							for (int retry = 0; retry < 5; retry++)
							{
								result = await ftp.DownloadFile("demo2017", "demo2017", remoteFile, localFile);
								if (result.StartsWith("221", StringComparison.CurrentCulture)) break;
							}
						}
						m_image.Source = localFile;
					}
				}
				m_desc.Text = "";
				await DisplayAlert("Facile", "Importazione dati conclusa regolarmente!", "Ok");
			}
			catch (Exception ex)
			{
				await DisplayAlert("Download Error", ex.Message, "OK");
			}
			await Navigation.PopModalAsync();
		}


	}
}

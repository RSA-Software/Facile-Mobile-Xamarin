using System;
using System.Collections.Generic;
using Facile.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SincronizePage : ContentPage
	{
		public SincronizePage()
		{
			InitializeComponent();

			if (Device.RuntimePlatform == Device.iOS)
			{
				m_down_data.Text = "Ric. Dati";
				m_down_img.Text = "Immagini";
				m_upl_doc.Text = "Invio Doc.";
			}
		}

		async void OnDownloadDati(object sender, System.EventArgs e)
		{
			await Navigation.PushModalAsync(new DownloadPage());
		}


		async void OnDownloadImages(object sender, System.EventArgs e)
		{
			await Navigation.PushModalAsync(new DownloadImages());
		}


		async void OnUploadDocumentsAsync(object sender, System.EventArgs e)
		{
			await Navigation.PushModalAsync(new UploadPage());
		}

		async void OnInvioIncassiClicked(object sender, System.EventArgs e)
		{
			try
			{
				var dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();
				var num = await dbcon.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM scapaghe");
				if (num == 0)
				{
					await DisplayAlert("Attenzione!", "Nessun incasso presente in archivio", "OK");
					return;
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Errore", ex.Message, "OK");
				return;
			}
			await Navigation.PushModalAsync(new IncassiInvia());
		}
	}
}

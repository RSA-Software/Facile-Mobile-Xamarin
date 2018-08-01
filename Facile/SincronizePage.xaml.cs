using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
			int start = 0;
			//do
			//{
				var page = new DownloadImages(start, start + 25);
				await Navigation.PushModalAsync(page);
				//while(page.IsWorking)
				//{
				//	Task.Delay(100).Wait();
				//}
				//if (!page.Recall) break;
			//	start += 25 + 1;
			//	if (start > 3500) break;
			//} while (true);
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

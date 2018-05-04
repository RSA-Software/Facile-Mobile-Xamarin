using System;
using System.Collections.Generic;

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
	}
}

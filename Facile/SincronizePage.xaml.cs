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


		void OnDownloadImages(object sender, System.EventArgs e)
		{
			throw new NotImplementedException();
		}


		void OnUploadDocuments(object sender, System.EventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}

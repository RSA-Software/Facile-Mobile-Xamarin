using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class IncassiPage : ContentPage
	{
		public IncassiPage()
		{
			InitializeComponent();
		}

		async void OnAggiungiClicked(object sender, System.EventArgs e)
		{
			var page = new Incassi(null, null);
			await Navigation.PushAsync(page);
		}

		void OnModificaClicked(object sender, System.EventArgs e)
		{
			
		}

		async void OnElencoClicked(object sender, System.EventArgs e)
		{
			var page = new IncassiGrid();
			await Navigation.PushAsync(page);
		}
	}
}

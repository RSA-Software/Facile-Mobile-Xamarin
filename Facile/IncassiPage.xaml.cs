using System;
using System.Collections.Generic;
using Facile.Interfaces;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Facile.Models;

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

		async void OnModificaClicked(object sender, System.EventArgs e)
		{
			ScaPagHead dsp = null;
			SQLiteAsyncConnection dbcon;
			dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();

			busyIndicator.IsBusy = true;
			try
			{
				var sql = string.Format("SELECT * from scapaghe WHERE ORDER BY dsp_codice DESC LIMIT 1");
				var dspList = await dbcon.QueryAsync<ScaPagHead>("SELECT * from scapaghe ORDER BY dsp_codice DESC LIMIT 1");
				foreach (var x in dspList)
				{
					dsp = x;
					break;
				}
			}
			catch (Exception ex)
			{
				busyIndicator.IsBusy = false;
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				return;
			}
			if (dsp == null)
			{
				busyIndicator.IsBusy = false;
				await DisplayAlert("Attenzione!", "Non è stato trovato in archivio alcun incasso", "Ok");
				return;
			}
			var page = new IncassiModifica(ref dsp);
			await Navigation.PushAsync(page);
			busyIndicator.IsBusy = false;
		}

		async void OnElencoClicked(object sender, System.EventArgs e)
		{
			var page = new IncassiGrid();
			await Navigation.PushAsync(page);
		}
	}
}

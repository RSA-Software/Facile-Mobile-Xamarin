using Xamarin.Forms;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using System;
using static Facile.Extension.FattureExtensions;
using System.Diagnostics;

namespace Facile
{
	public partial class FacilePage : ContentPage
	{
		void OnClickedClienti(object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new ClientiSearch());
		}


		async void OnClickedOrdini(object sender, System.EventArgs e)
		{
			await Navigation.PushAsync(new FatturePage(DocTipo.TIPO_ORD));
		}



		async void OnClickedFatture(object sender, System.EventArgs e)
		{

			await Navigation.PushAsync(new FatturePage(DocTipo.TIPO_FAT));
		}

		async void OnClickedDdt(object sender, System.EventArgs e)
		{
			await Navigation.PushAsync(new FatturePage(DocTipo.TIPO_DDT));
		}

		async void OnClickedScadenze(object sender, System.EventArgs e)
		{
			var page = new ScadenzeElenco();
			await Navigation.PushAsync(page);
		}

		void OnClickedIncassi(object sender, System.EventArgs e)
		{
			DisplayAlert("Incassi", "La procedura sarà disponibile nelle prossime release!", "OK");
		}

		async void OnClickedSincronizza(object sender, System.EventArgs e)
		{

			//await Navigation.PushModalAsync(new DownloadPage());
			await Navigation.PushAsync(new SincronizePage());
		}

		async void OnClickedImpostazioni(object sender, System.EventArgs e) => await Navigation.PushAsync(new SetupPage());

		public FacilePage()
		{
			InitializeComponent();

		}

		protected override async void OnAppearing()
		{
			//DependencyService.Get<ISQLiteDb>().RemoveDB();
			var dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();

			await dbcon.CreateTableAsync<Ditte>();
			await dbcon.CreateTableAsync<Zone>();
			await dbcon.CreateTableAsync<Cateco>();
			await dbcon.CreateTableAsync<Pagamenti>();
			await dbcon.CreateTableAsync<Tabelle>();
			await dbcon.CreateTableAsync<Agenti>();
			await dbcon.CreateTableAsync<Misure>();
			await dbcon.CreateTableAsync<Clienti>();
			await dbcon.CreateTableAsync<Destinazioni>();
			await dbcon.CreateTableAsync<Scadenze>();
			await dbcon.CreateTableAsync<Codiva>();
			await dbcon.CreateTableAsync<Reparti>();
			await dbcon.CreateTableAsync<Catmerc>();
			await dbcon.CreateTableAsync<Fornitori>();
			await dbcon.CreateTableAsync<Depositi>();
			await dbcon.CreateTableAsync<Lotti>();
			await dbcon.CreateTableAsync<Artanag>();
			await dbcon.CreateTableAsync<Listini>();
			await dbcon.CreateTableAsync<Fatture>();
			await dbcon.CreateTableAsync<FatRow>();
			await dbcon.CreateTableAsync<Vettori>();
			await dbcon.CreateTableAsync<Banche>();
			await dbcon.CreateTableAsync<Canali>();
			await dbcon.CreateTableAsync<Stagioni>();
			await dbcon.CreateTableAsync<Marchi>();
			await dbcon.CreateTableAsync<Associazioni>();
			await dbcon.CreateTableAsync<Barcode>();
			await dbcon.CreateTableAsync<Trasporti>();
			await dbcon.CreateTableAsync<Agganci>();
			await dbcon.CreateTableAsync<Descrizioni>();
			await dbcon.CreateTableAsync<LocalImpo>();

			if (await dbcon.Table<LocalImpo>().CountAsync() == 0)
			{
				var imp = new LocalImpo();
				imp.id = 1;
				await dbcon.InsertAsync(imp);
			}

			try
			{
				var app = (App)Application.Current;
				var ditlist = await dbcon.QueryAsync<Ditte>("SELECT * FROM ditt2016 ORDER BY dit_codice LIMIT 1");
				if (ditlist.Count == 0) 
					app.facile_db_impo = null;
				else
					app.facile_db_impo = ditlist[0];
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}

			base.OnAppearing();
		}
	}
}

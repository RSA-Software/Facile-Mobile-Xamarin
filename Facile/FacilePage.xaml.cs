using Xamarin.Forms;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using System;
using static Facile.Extension.FattureExtensions;
using System.Diagnostics;
using Facile.Articoli;

namespace Facile
{
	public partial class FacilePage : ContentPage
	{
		private bool first_appearing_;

		async void OnClickedClienti(object sender, System.EventArgs e)
		{
			await Navigation.PushAsync(new ClientiSearch());

			//await Navigation.PushAsync(new MarchiFilter(false));
			//await Navigation.PushAsync(new StagioniFilter(false));
			//await Navigation.PushAsync(new RepartiFilter(false));
			//await Navigation.PushAsync(new CatMercFilter(false));
			//await Navigation.PushAsync(new FornitoriFilter(false));
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

		async void OnClickedIncassi(object sender, System.EventArgs e)
		{

			//var page = new Catalogo();
			//var page = new Incassi(null, null);
			var page = new IncassiPage();
			await Navigation.PushAsync(page);

			//DisplayAlert("Incassi", "La procedura sarà disponibile nelle prossime release!", "OK");
		}

		async void OnClickedSincronizza(object sender, System.EventArgs e)
		{

			//await Navigation.PushModalAsync(new DownloadPage());
			await Navigation.PushAsync(new SincronizePage());
		}

		async void OnClickedImpostazioni(object sender, System.EventArgs e) => await Navigation.PushAsync(new SetupPage());

		public FacilePage()
		{
			first_appearing_ = true;
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			if (first_appearing_)
			{
				first_appearing_ = false;

				//DependencyService.Get<ISQLiteDb>().RemoveDB();
				var dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();

				await dbcon.CreateTableAsync<FiltersDb>();
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
				await dbcon.CreateTableAsync<ScaPagHead>();
				await dbcon.CreateTableAsync<ScaPagRow>();
				await dbcon.CreateTableAsync<Images>();
				await dbcon.CreateTableAsync<ArtCounter>();

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
					{
						app.facile_db_impo = ditlist[0];

						//
						// Conversione campi ditta
						//
						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_sco) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_sco_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_sco = dit_reg_sco_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_sco_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_sco = app.facile_db_impo.dit_reg_sco_free;
							app.facile_db_impo.dit_reg_sco_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_car) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_car_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_car = dit_reg_car_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_car_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_car = app.facile_db_impo.dit_reg_car_free;
							app.facile_db_impo.dit_reg_car_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_fat) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_fat_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_fat = dit_reg_fat_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_fat_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_fat = app.facile_db_impo.dit_reg_fat_free;
							app.facile_db_impo.dit_reg_fat_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_ddt) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_ddt_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_ddt = dit_reg_ddt_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_ddt_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_ddt = app.facile_db_impo.dit_reg_ddt_free;
							app.facile_db_impo.dit_reg_ddt_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_bol) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_bol_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_bol = dit_reg_bol_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_bol_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_bol = app.facile_db_impo.dit_reg_bol_free;
							app.facile_db_impo.dit_reg_bol_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_buo) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_buo_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_buo = dit_reg_buo_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_buo_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_buo = app.facile_db_impo.dit_reg_buo_free;
							app.facile_db_impo.dit_reg_buo_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_acc) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_acc_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_acc = dit_reg_acc_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_acc_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_acc = app.facile_db_impo.dit_reg_acc_free;
							app.facile_db_impo.dit_reg_acc_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_ric) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_ric_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_ric = dit_reg_ric_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_ric_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_ric = app.facile_db_impo.dit_reg_ric_free;
							app.facile_db_impo.dit_reg_ric_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_ord) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_ord_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_ord = dit_reg_ord_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_ord_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_ord = app.facile_db_impo.dit_reg_ord_free;
							app.facile_db_impo.dit_reg_ord_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_pre) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_pre_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_pre = dit_reg_pre_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_pre_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_pre = app.facile_db_impo.dit_reg_pre_free;
							app.facile_db_impo.dit_reg_pre_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_ofo) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_ofo_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_ofo = dit_reg_ofo_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_ofo_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_ofo = app.facile_db_impo.dit_reg_ofo_free;
							app.facile_db_impo.dit_reg_ofo_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_auf) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_auf_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_auf = dit_reg_auf_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_auf_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_auf = app.facile_db_impo.dit_reg_auf_free;
							app.facile_db_impo.dit_reg_auf_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_fpf) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_fpf_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_fpf = dit_reg_fpf_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_fpf_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_fpf = app.facile_db_impo.dit_reg_fpf_free;
							app.facile_db_impo.dit_reg_fpf_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_oro) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_oro_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_oro = dit_reg_oro_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_oro_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_oro = app.facile_db_impo.dit_reg_oro_free;
							app.facile_db_impo.dit_reg_oro_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_fat_pa) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_fat_pa_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_fat_pa = dit_reg_fat_pa_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_fat_pa_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_fat_pa = app.facile_db_impo.dit_reg_fat_pa_free;
							app.facile_db_impo.dit_reg_fat_pa_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_cre_pa) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_cre_pa_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_cre_pa = dit_reg_cre_pa_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_cre_pa_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_cre_pa = app.facile_db_impo.dit_reg_cre_pa_free;
							app.facile_db_impo.dit_reg_cre_pa_free = string.Empty;
						}

						if (string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_ano) && !string.IsNullOrWhiteSpace(app.facile_db_impo.dit_reg_ano_free))
						{
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_ano = dit_reg_ano_free WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							await dbcon.ExecuteAsync($"UPDATE ditt2016 SET dit_reg_ano_free = '' WHERE dit_codice = {app.facile_db_impo.dit_codice}");
							app.facile_db_impo.dit_reg_ano = app.facile_db_impo.dit_reg_ano_free;
							app.facile_db_impo.dit_reg_ano_free = string.Empty;
						}

						//
						// Conversione Campi documenti
						//
						await dbcon.ExecuteAsync($"UPDATE fatture2 SET fat_registro = fat_registro_free WHERE TRIM(fat_registro) = '' AND TRIM(fat_registro_free) != ''");
						await dbcon.ExecuteAsync($"UPDATE fatture2 SET fat_registro_free = '' WHERE TRIM(fat_registro_free) != ''");
					}
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.Message);
				}
			}
			base.OnAppearing();}
	}
}

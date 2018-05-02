using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Facile.Extension;
using Facile.Interfaces;
using Facile.Models;
using PCLStorage;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DocumentRow : ContentPage
	{
		protected FatRow rig_;
		private bool change_;
		private readonly bool nuovo_;
		private bool first_;
		private bool editable_;
		private readonly SQLiteAsyncConnection dbcon_;

		public DocumentRow(ref FatRow rig, bool nuovo = true, bool editable = true)
		{
			rig_ = rig;
			change_ = false;
			first_ = true;
			nuovo_ = nuovo;
			editable_ = editable;
			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			m_image.Source = null;
			if (Device.RuntimePlatform == Device.iOS) Padding = new Thickness(0, 30, 0, 0);

			if (nuovo_)
			{ 
				m_elimina.IsEnabled = false;
				m_elimina.IsVisible = false;
			}

			//
			// Attenzione : da rimuovere
			//
			//editable_ = true;
			// fine

			if (!editable_)
			{
				m_art.IsEnabled = false;
				m_search.IsEnabled = false;

				m_salva.IsEnabled = false;
				m_elimina.IsEnabled = false;

				m_salva.IsVisible = false;
				m_elimina.IsVisible = false;

				m_quantita.IsEnabled = false;
				m_prezzo.IsEnabled = false;
				m_sco1.IsEnabled = false;
				m_sco2.IsEnabled = false;
				m_sco3.IsEnabled = false;
				m_sostituzione.IsEnabled = false;

				m_qta_down.IsEnabled = false;
				m_qta_up.IsEnabled = false;

				m_prezzo_down.IsEnabled = false;
				m_prezzo_up.IsEnabled = false;

				m_sco1_down.IsEnabled = false;
				m_sco1_up.IsEnabled = false;

				m_sco2_down.IsEnabled = false;
				m_sco2_up.IsEnabled = false;

				m_sco3_down.IsEnabled = false;
				m_sco3_up.IsEnabled = false;
			}

			if (Device.RuntimePlatform == Device.Android)
			{
				m_esci.IsEnabled = false;
				m_esci.IsVisible = false;
			}

			m_quantita.Culture = new CultureInfo("it-IT");
			m_prezzo.Culture = new CultureInfo("it-IT");
			m_sco1.Culture = new CultureInfo("it-IT");
			m_sco2.Culture = new CultureInfo("it-IT");
			m_sco3.Culture = new CultureInfo("it-IT");
			m_totale.Culture = new CultureInfo("it-IT");
			SetField();
		}

		protected async override void OnAppearing()
		{
			if (first_)
			{ 
				await LoadImage();
				first_ = false;
			}
		}

		protected async Task LoadImage ()
		{
			if (!string.IsNullOrWhiteSpace(rig_.rig_art))
			{
				IFolder rootFolder = FileSystem.Current.LocalStorage;
				IFolder imagesFolder = await rootFolder.CreateFolderAsync("images", CreationCollisionOption.OpenIfExists);

				String fileName = rig_.rig_art.Trim() + "_0.PNG";
				ExistenceCheckResult status = await imagesFolder.CheckExistsAsync(fileName);
				if (status == ExistenceCheckResult.FileExists)
				{
					m_image.Source = rootFolder.Path + "/images/" + fileName;
					m_image_box.IsVisible = true;
					return;
				}
				fileName = rig_.rig_art.Trim() + "_0.JPG";
				status = await imagesFolder.CheckExistsAsync(fileName);
				if (status == ExistenceCheckResult.FileExists)
				{
					m_image.Source = rootFolder.Path + "/images/" + fileName;
					m_image_box.IsVisible = true;
					return;
				}
			}
			//m_image.Source = "header_wallpaper.jpg";
			m_image.Source = null;
			m_image_box.IsVisible = false;
		}

		public void SetField()
		{
			m_art.Text = rig_.rig_art;
			m_desc.Text = rig_.rig_newdes;
			m_quantita.Value = rig_.rig_qta;
			m_prezzo.Value = rig_.rig_prezzo;
			m_sco1.Value = rig_.rig_sconto1;
			m_sco2.Value = rig_.rig_sconto2;
			m_sco3.Value = rig_.rig_sconto3;
			m_totale.Value = rig_.rig_importo;
			m_sostituzione.IsToggled = rig_.rig_sost != 0 ? true : false;
		}

		public void GetField()
		{
			rig_.rig_art = m_art.Text;
			rig_.rig_newdes = m_desc.Text;

			rig_.rig_qta = double.Parse(m_quantita.Value.ToString());
			rig_.rig_prezzo = double.Parse(m_prezzo.Value.ToString());
			rig_.rig_sconto1 = double.Parse(m_sco1.Value.ToString());
			rig_.rig_sconto2 = double.Parse(m_sco2.Value.ToString());
			rig_.rig_sconto3 = double.Parse(m_sco3.Value.ToString());
			rig_.rig_importo = double.Parse(m_totale.Value.ToString());
			if (m_sostituzione.IsToggled)
				rig_.rig_sost = 1;
			else
				rig_.rig_sost = 0;
		}

		async void OnValueChanged(object sender, Syncfusion.SfNumericTextBox.XForms.ValueEventArgs e)
		{
			if (!change_)
			{
				change_ = true;
				GetField();
				await rig_.RecalcAsync();
				SetField();
				change_ = false;
			}
		}

		async void OnSwitchToggled(object sender, Xamarin.Forms.ToggledEventArgs e)
		{
			if (!change_)
			{
				change_ = true;
				GetField();
				await rig_.RecalcAsync();
				SetField();
				change_ = false;
			}
		}

		async void OnSearchClicked(object sender, System.EventArgs e)
		{
			string old_art = rig_.rig_art;
			var page = new ArticoliSearch();
			page.AnaList.ItemDoubleTapped += async (source, args) =>
			{
				change_ = true;
				Artanag ana = (Artanag)args.ItemData;
				rig_.rig_art = ana.ana_codice;
				rig_.rig_newdes = (ana.ana_desc1 + " " + ana.ana_desc2).Trim();
				rig_.rig_iva = ana.ana_iva;
				rig_.rig_mis = ana.ana_mis;
				rig_.rig_peso = ana.ana_peso;
				rig_.rig_peso_mis = ana.ana_peso_mis;
				rig_.rig_qta = 1;

				var sql = string.Format("SELECT * FROM listini1 WHERE lis_codice = {0} AND lis_art = {1} LIMIT 1", 1, ana.ana_codice.SqlQuote(false));
				var listini = await dbcon_.QueryAsync<Listini>(sql);

				if (listini.Count > 0)
				{
					rig_.rig_prezzo = listini[0].lis_prezzo;
					rig_.rig_sconto1 = listini[0].lis_sco1;
					rig_.rig_sconto2 = listini[0].lis_sco2;
					rig_.rig_sconto3 = listini[0].lis_sco3;
					await rig_.RecalcAsync();
				}
				await Navigation.PopModalAsync();
				if (string.Compare(rig_.rig_art,old_art) != 0)
				{
					await LoadImage();
				}
				SetField();
				m_quantita.Focus();
				change_ = false;
			};
			await Navigation.PushModalAsync(page);
		}

		async void OnClickedSalva(object sender, System.EventArgs e)
		{
			GetField();
			if (String.IsNullOrWhiteSpace(rig_.rig_art))
			{
				m_art.Focus();
				return;
			}
			if (Math.Abs(rig_.rig_qta) < NumericExtensions.EPSILON)
			{
				m_quantita.Focus();
				return;
			}
			await rig_.RecalcAsync();

			if (nuovo_)
			{
				rig_.rig_d_ins = DateTime.Now;
				rig_.rig_t_ins = rig_.rig_d_ins.Hour * 3600 + rig_.rig_d_ins.Minute * 60 + rig_.rig_d_ins.Second;
				await dbcon_.InsertAsync(rig_);
			}
			else
				await dbcon_.UpdateAsync(rig_);


			await Navigation.PopModalAsync();
		}

		async void OnClickedElimina(object sender, System.EventArgs e)
		{
			if (editable_ && !nuovo_ && await DisplayAlert("Attenzione!", "Confermi la cancellazione della riga?", "Si", "No"))
			{

				if (await dbcon_.DeleteAsync(rig_) != 0)
					await Navigation.PopModalAsync();
				else
					await DisplayAlert("Attenzione!", "Non è stato possibile eliminare la riga", "Ok");
			}
		}

		void OnClickedEsci(object sender, System.EventArgs e)
		{
			Navigation.PopModalAsync();
		}

		async void OnArtUnfocused(object sender, Xamarin.Forms.FocusEventArgs e)
		{
			var ent = (Entry)sender;

			var codice = "";
			if (ent.Text != null) codice = ent.Text.Trim().ToUpper();
			if (string.Compare(codice, rig_.rig_art) == 0) return;

			var sql = string.Format("SELECT * FROM artanag WHERE ana_codice = {0} LIMIT 1", codice.SqlQuote(false));
			var anaList = await dbcon_.QueryAsync<Artanag>(sql);

			if (anaList.Count == 0)  // Cerchiamo tra i codici a Barre
			{
				//sql = string.Format("SELECT * FROM artanag WHERE ana_codice = {0} LIMIT 1", input.Text.SqlQuote(false));


			}
			if (anaList.Count > 0)
			{
				change_ = true;
				rig_.rig_art = anaList[0].ana_codice;
				rig_.rig_newdes = (anaList[0].ana_desc1 + " " + anaList[0].ana_desc2).Trim();
				rig_.rig_iva = anaList[0].ana_iva;
				rig_.rig_mis = anaList[0].ana_mis;
				rig_.rig_peso = anaList[0].ana_peso;
				rig_.rig_peso_mis = anaList[0].ana_peso_mis;
				if (Math.Abs(rig_.rig_qta) < NumericExtensions.EPSILON) rig_.rig_qta = 1;
				sql = string.Format("SELECT * FROM listini1 WHERE lis_codice = {0} AND lis_art = {1} LIMIT 1", 1, anaList[0].ana_codice.SqlQuote(false));
				var listini = await dbcon_.QueryAsync<Listini>(sql);

				if (listini.Count > 0)
				{
					rig_.rig_prezzo = listini[0].lis_prezzo;
					rig_.rig_sconto1 = listini[0].lis_sco1;
					rig_.rig_sconto2 = listini[0].lis_sco2;
					rig_.rig_sconto3 = listini[0].lis_sco3;
				}
				else
				{
					rig_.rig_prezzo = 0;
					rig_.rig_sconto1 = 0;
					rig_.rig_sconto2 = 0;
					rig_.rig_sconto3 = 0;
				}
				await rig_.RecalcAsync();
				await LoadImage();
				SetField();
				change_ = false;
			}
			else
			{
				change_ = true;
				rig_.rig_art = string.Empty;
				rig_.rig_newdes = string.Empty;
				rig_.rig_iva = 0;
				rig_.rig_mis = 0;
				rig_.rig_peso = 0;
				rig_.rig_peso_mis = string.Empty;
				rig_.rig_qta = 1;
				rig_.rig_prezzo = 0;
				rig_.rig_sconto1 = 0;
				rig_.rig_sconto2 = 0;
				rig_.rig_sconto3 = 0;
				await rig_.RecalcAsync();
				await LoadImage();
				SetField();
				change_ = false;
			}
		}

		async void OnQtaDownClicked(object sender, System.EventArgs e)
		{
			change_ = true;
			GetField();
			rig_.rig_qta -= 1;
			await rig_.RecalcAsync();
			SetField();
			change_ = false;
		}

		async void OnQtaUpClicked(object sender, System.EventArgs e)
		{
			change_ = true;
			GetField();
			rig_.rig_qta += 1;
			await rig_.RecalcAsync();
			SetField();
			change_ = false;
		}

		async void OnPriceDownClicked(object sender, System.EventArgs e)
		{
			change_ = true;
			GetField();
			if ((rig_.rig_prezzo - 0.10) >= 0.0) rig_.rig_prezzo -= 0.10;
			await rig_.RecalcAsync();
			SetField();
			change_ = false;
		}

		async void OnPriceUpClicked(object sender, System.EventArgs e)
		{
			change_ = true;
			GetField();
			rig_.rig_prezzo += 0.10;
			await rig_.RecalcAsync();
			SetField();
			change_ = false;
		}

		async void OnSco1DownClicked(object sender, System.EventArgs e)
		{
			change_ = true;
			GetField();
			if ((rig_.rig_sconto1 - 1) >= 0.0) rig_.rig_sconto1 -= 1;
			await rig_.RecalcAsync();
			SetField();
			change_ = false;
		}

		async void OnSco1UpClicked(object sender, System.EventArgs e)
		{
			change_ = true;
			GetField();
			rig_.rig_sconto1 += 1;
			await rig_.RecalcAsync();
			SetField();
			change_ = false;
		}

		async void OnSco2DownClicked(object sender, System.EventArgs e)
		{
			change_ = true;
			GetField();
			if ((rig_.rig_sconto2 - 1) >= 0.0) rig_.rig_sconto2 -= 1;
			await rig_.RecalcAsync();
			SetField();
			change_ = false;
		}

		async void OnSco2UpClicked(object sender, System.EventArgs e)
		{
			change_ = true;
			GetField();
			rig_.rig_sconto2 += 1;
			await rig_.RecalcAsync();
			SetField();
			change_ = false;
		}

		async void OnSco3DownClicked(object sender, System.EventArgs e)
		{
			change_ = true;
			GetField();
			if ((rig_.rig_sconto3 - 1) >= 0.0) rig_.rig_sconto3 -= 1;
			await rig_.RecalcAsync();
			SetField();
			change_ = false;
		}

		async void OnSco3UpClicked(object sender, System.EventArgs e)
		{
			change_ = true;
			GetField();
			rig_.rig_sconto3 += 1;
			await rig_.RecalcAsync();
			SetField();
			change_ = false;
		}

	}
}

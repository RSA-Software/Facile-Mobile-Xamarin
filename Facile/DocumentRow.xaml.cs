using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Facile.Extension;
using Facile.Interfaces;
using Facile.Models;
using Facile.ViewModels;
using PCLStorage;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DocumentRow : ContentPage
	{
		protected DocumentiBody par_;
		protected FatRow rig_;
		private bool change_;
		private readonly int index_;
		private bool first_;
		private bool editable_;
		private readonly SQLiteAsyncConnection dbcon_;

		public DocumentRow(DocumentiBody par, ref FatRow rig, int index = -1, bool editable = true)
		{
			par_ = par;
			rig_ = rig;
			change_ = false;
			first_ = true;
			index_ = index;
			editable_ = editable;
			InitializeComponent();

			//var culture = new CultureInfo("en-US");
			//m_quantita.Culture = culture; 
			//m_prezzo.Culture = culture; 
			//m_sco1.Culture = culture; 
			//m_sco2.Culture = culture; 
			//m_sco3.Culture = culture; 
			//m_totale.Culture = culture; 

			//
			// Rimuoviamo le righe per i lotti se non gestiti
			//
			if (!((App)Application.Current).facile_db_impo.dit_usa_lotti)
			{
				int x = 0;
				foreach (var row in m_grid.RowDefinitions)
				{
					if (x == 9 || x == 10)
					{
						row.Height = 0;
					}
					x++;
				}
			}

			NavigationPage.SetHasNavigationBar(this, false);
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			m_image.Source = null;
			if (Device.RuntimePlatform == Device.iOS) Padding = new Thickness(0, 30, 0, 0);

			if (Device.Idiom == TargetIdiom.Tablet)
			{
				if (((App)Application.Current).facile_db_impo.dit_usa_lotti)
					m_image_box.HeightRequest = 450;
				else
					m_image_box.HeightRequest = 500;
			}

			if (index_ == -1)
			{
				m_elimina.IsEnabled = false;
				m_elimina.IsVisible = false;
			}

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

				m_search_lotto.IsEnabled = false;
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

		protected async Task LoadImage()
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
			if (Device.Idiom == TargetIdiom.Phone)
			{
				m_image.Source = null;
				m_image_box.IsVisible = false;
			}
			else
			{
				m_image.Source = "header_wallpaper.jpg";
			}
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
			m_lotto.Text = rig_.rig_lotto;
			m_scadenza.Date = rig_.rig_scadenza != null ? rig_.rig_scadenza.Value : DateTime.Now;

			m_scadenza.IsVisible = rig_.rig_scadenza != null;
		}

		public void GetField()
		{
			rig_.rig_art = m_art.Text;
			rig_.rig_newdes = m_desc.Text;

			try
			{
				rig_.rig_qta = m_quantita.Value == null ? 0 : Convert.ToDouble(m_quantita.Value);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}

			try
			{
				rig_.rig_prezzo = m_prezzo.Value == null ? 0 : Convert.ToDouble(m_prezzo.Value);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}

			try
			{
				rig_.rig_sconto1 = m_sco1.Value == null ? 0 : Convert.ToDouble(m_sco1.Value);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}

			try
			{
				rig_.rig_sconto2 = m_sco2.Value == null ? 0 : Convert.ToDouble(m_sco2.Value);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}

			try
			{
				rig_.rig_sconto3 = m_sco3.Value == null ? 0 : Convert.ToDouble(m_sco3.Value);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}

			try
			{
				rig_.rig_importo = m_totale.Value == null ? 0 : Convert.ToDouble(m_totale.Value);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}

			if (m_sostituzione.IsToggled)
				rig_.rig_sost = 1;
			else
				rig_.rig_sost = 0;

			rig_.rig_lotto = m_lotto.Text;
			rig_.rig_scadenza = m_scadenza.Date;

			if (string.IsNullOrWhiteSpace(rig_.rig_lotto)) rig_.rig_scadenza = null;
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
				if (string.Compare(rig_.rig_art, old_art) != 0)
				{
					rig_.rig_gest_lotto = 0;
					rig_.rig_lotto = "";
					rig_.rig_scadenza = null;
					await LoadImage();
				}
				SetField();
				m_quantita.Focus();
				change_ = false;
			};
			await Navigation.PushModalAsync(page);
		}

		async void OnLottoSearchClicked(object sender, System.EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(rig_.rig_art)) return;

			//
			// Verifichiamo che ci  siano lotti per l'articolo
			//
			var cod_art = rig_.rig_art;
			string sql = "SELECT COUNT(*) FROM lotti1 WHERE lot_stop IS NULL AND lot_start IS NOT NULL AND lot_codice = " + cod_art.SqlQuote(false);
			var recTotal_ = await dbcon_.ExecuteScalarAsync<int>(sql);
			if (recTotal_ == 0)
			{
				await DisplayAlert("Attenzione!", "Non ci sono lotti per l'articolo indicato", "OK");
				return;
			}

			var page = new LottiSearch(rig_.rig_art);
			page.LotList.ItemDoubleTapped += async (source, args) =>
			{
				Lotti lotti = (Lotti)args.ItemData;
				change_ = true;
				rig_.rig_gest_lotto = 1;
				rig_.rig_lotto = lotti.lot_lotto;
				rig_.rig_scadenza = lotti.lot_scadenza;
				await Navigation.PopModalAsync();
				SetField();
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

			if (index_ == -1)
			{
				rig_.rig_d_ins = DateTime.Now;
				rig_.rig_t_ins = rig_.rig_d_ins.Hour * 3600 + rig_.rig_d_ins.Minute * 60 + rig_.rig_d_ins.Second;
				await dbcon_.InsertAsync(rig_);
				par_.rigCollection.Add(rig_);
			}
			else
			{
				await dbcon_.UpdateAsync(rig_);
				par_.rigCollection.RemoveAt(index_);
				par_.rigCollection.Insert(index_, rig_);
			}
			await Navigation.PopModalAsync();
		}

		async void OnClickedElimina(object sender, System.EventArgs e)
		{
			if (editable_ && index_ != -1 && await DisplayAlert("Attenzione!", "Confermi la cancellazione della riga?", "Si", "No"))
			{

				if (await dbcon_.DeleteAsync(rig_) != 0)
				{
					par_.rigCollection.RemoveAt(index_);
					await Navigation.PopModalAsync();
				}
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
			double quantita = 0.0;
			double totale = 0.0;

			var codice = "";
			if (ent.Text != null) codice = ent.Text.Trim().ToUpper();
			if (string.Compare(codice, rig_.rig_art) == 0) return;

			var sql = string.Format("SELECT * FROM artanag WHERE ana_codice = {0} LIMIT 1", codice.SqlQuote(false));
			var anaList = await dbcon_.QueryAsync<Artanag>(sql);

			if (anaList.Count == 0)  // Cerchiamo tra i codici a Barre
			{
				sql = string.Format("SELECT * FROM barcode WHERE bar_barcode = {0} LIMIT 1", codice.SqlQuote(false));
				var barList = await dbcon_.QueryAsync<Barcode>(sql);
				if (barList.Count != 0)
				{
					sql = string.Format("SELECT * FROM artanag WHERE ana_codice = {0} LIMIT 1", barList[0].bar_codart.Trim().SqlQuote(false));
					anaList = await dbcon_.QueryAsync<Artanag>(sql);
				}
				else
				{
					if (((codice.Length == 12 || codice.Length == 13)) && codice.AllDigits() && codice.StartsWith("2", StringComparison.CurrentCulture))
					{
						var codelen = 0;
						if (((App)Application.Current).facile_db_impo.dit_codbil == 0)
							codelen = 6;
						else
							codelen = 7;
						var code = codice.Substring(0, codelen);
						var data = codice.Substring(7, 5);

						while (code.Length < 13)
						{
							code += "0";
						}

						sql = string.Format("SELECT * FROM artanag WHERE ana_codice = {0} LIMIT 1", code.SqlQuote(false));
						anaList = await dbcon_.QueryAsync<Artanag>(sql);

						if (anaList.Count > 0)
						{
							if (anaList[0].ana_venapeso == (int)DatiEtichetta.ANA_VEN_COD_PESO)
							{
								quantita = Math.Round(Convert.ToDouble(data) / 1000, 3, MidpointRounding.AwayFromZero);
							}
							else if (anaList[0].ana_venapeso == (int)DatiEtichetta.ANA_VEN_COD_PREZZO || anaList[0].ana_venapeso == (int)DatiEtichetta.ANA_VEN_COD_PREZZO_Q1)
							{
								totale = Math.Round(Convert.ToDouble(data) / 100, 2, MidpointRounding.AwayFromZero);
							}
						}
						else
						{
							sql = string.Format("SELECT * FROM barcode WHERE bar_barcode = {0} LIMIT 1", code.SqlQuote(false));
							barList = await dbcon_.QueryAsync<Barcode>(sql);
							if (barList.Count != 0)
							{
								sql = string.Format("SELECT * FROM artanag WHERE ana_codice = {0} LIMIT 1", barList[0].bar_codart.Trim().SqlQuote(false));
								anaList = await dbcon_.QueryAsync<Artanag>(sql);
							}
							if (anaList.Count > 0)
							{
								if (anaList[0].ana_venapeso == (int)DatiEtichetta.ANA_VEN_COD_PESO)
								{
									quantita = Math.Round(Convert.ToDouble(data) / 1000, 3, MidpointRounding.AwayFromZero);
								}
								else if (anaList[0].ana_venapeso == (int)DatiEtichetta.ANA_VEN_COD_PREZZO || anaList[0].ana_venapeso == (int)DatiEtichetta.ANA_VEN_COD_PREZZO_Q1)
								{
									totale = Math.Round(Convert.ToDouble(data) / 100, 2, MidpointRounding.AwayFromZero);
								}
							}
						}
					}
				}
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

				if (quantita > 0.0) rig_.rig_qta = quantita;
				if (Math.Abs(rig_.rig_qta) < NumericExtensions.EPSILON) rig_.rig_qta = 1;

				sql = string.Format("SELECT * FROM listini1 WHERE lis_codice = {0} AND lis_art = {1} LIMIT 1", 1, anaList[0].ana_codice.SqlQuote(false));
				var listini = await dbcon_.QueryAsync<Listini>(sql);

				if (listini.Count > 0)
				{
					if ((anaList[0].ana_venapeso == (int)DatiEtichetta.ANA_VEN_COD_PREZZO_Q1) && (totale > 0.0))
					{
						rig_.rig_prezzo = totale;
						rig_.rig_sconto1 = 0;
						rig_.rig_sconto2 = 0;
						rig_.rig_sconto3 = 0;
					}
					else if ((anaList[0].ana_venapeso == (int)DatiEtichetta.ANA_VEN_COD_PREZZO) && (totale > 0.0))
					{
						rig_.rig_prezzo = listini[0].lis_prezzo;
						rig_.rig_sconto1 = listini[0].lis_sco1;
						rig_.rig_sconto2 = listini[0].lis_sco2;
						rig_.rig_sconto3 = listini[0].lis_sco3;

						var prezzo = listini[0].lis_prezzo;
						prezzo -= Math.Round(prezzo * (listini[0].lis_sco1 / 100), 4, MidpointRounding.AwayFromZero);
						prezzo -= Math.Round(prezzo * (listini[0].lis_sco2 / 100), 4, MidpointRounding.AwayFromZero);
						prezzo -= Math.Round(prezzo * (listini[0].lis_sco3 / 100), 4, MidpointRounding.AwayFromZero);

						if (!prezzo.TestIfZero(4))
						{
							rig_.rig_qta = Math.Round(totale / prezzo, 3, MidpointRounding.AwayFromZero);
						}
					}
					else
					{
						rig_.rig_prezzo = listini[0].lis_prezzo;
						rig_.rig_sconto1 = listini[0].lis_sco1;
						rig_.rig_sconto2 = listini[0].lis_sco2;
						rig_.rig_sconto3 = listini[0].lis_sco3;
					}
				}
				else
				{
					rig_.rig_prezzo = 0;
					rig_.rig_sconto1 = 0;
					rig_.rig_sconto2 = 0;
					rig_.rig_sconto3 = 0;
				}

				rig_.rig_gest_lotto = 0;
				rig_.rig_lotto = "";
				rig_.rig_scadenza = null;

				try
				{
					await rig_.RecalcAsync();
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
				await LoadImage();
				SetField();
				change_ = false;
			}
			else
			{
				change_ = true;
				rig_.rig_art = "";
				rig_.rig_newdes = "";
				rig_.rig_iva = 0;
				rig_.rig_mis = 0;
				rig_.rig_peso = 0;
				rig_.rig_peso_mis = "";
				rig_.rig_qta = 1;
				rig_.rig_prezzo = 0;
				rig_.rig_sconto1 = 0;
				rig_.rig_sconto2 = 0;
				rig_.rig_sconto3 = 0;
				rig_.rig_gest_lotto = 0;
				rig_.rig_lotto = "";
				rig_.rig_scadenza = null;
				try
				{
					await rig_.RecalcAsync();
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Facile.Extension;
using Facile.Interfaces;
using Facile.Models;
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
		private bool nuovo_;
		private bool first_;
		private readonly SQLiteAsyncConnection dbcon_;

		public DocumentRow(ref FatRow rig, bool nuovo = true)
		{
			rig_ = rig;
			change_ = false;
			nuovo_ = nuovo;
			first_ = nuovo;
			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			m_quantita.Culture = new CultureInfo("it-IT");
			m_prezzo.Culture = new CultureInfo("it-IT");
			m_sco1.Culture = new CultureInfo("it-IT");
			m_sco2.Culture = new CultureInfo("it-IT");
			m_sco3.Culture = new CultureInfo("it-IT");
			m_totale.Culture = new CultureInfo("it-IT");
			SetField();
		}

		protected override async void OnAppearing()
		{
			if (nuovo_ && first_)
			{
				await OnArticoloTapped(this, new EventArgs());
				first_ = false;
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

			/*
			if (m_totale.Value.GetType().Equals(typeof(decimal)))
				rig_.rig_importo = decimal.ToDouble((decimal)m_totale.Value);
			else
				rig_.rig_importo = (double)m_totale.Value;
			*/
			rig_.rig_sost = m_sostituzione.IsToggled ? 1 : 0;
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

		private async Task OnArticoloTapped(object sender, System.EventArgs e)
		{
			var page = new ArticoliSearch();
			page.CliList.ItemDoubleTapped += async (source, args) =>
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

		void OnClickedEsci(object sender, System.EventArgs e)
		{
			Navigation.PopModalAsync();
		}
	}
}

using System;
using System.Collections.Generic;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Xamarin.Forms;

namespace Facile
{
	public partial class DocumentiHeader : ContentPage
	{
		private Fatture doc_;
		private bool nuova_;
		private bool editable_;
		private bool first;
		private readonly SQLiteAsyncConnection dbcon_;
		private Clienti cli_ = null;
		private Destinazioni dst_ = null;

		public DocumentiHeader(ref Fatture f, ref bool nuova, ref bool editable)
		{
			doc_ = f;
			nuova_ = nuova;
			editable_ = editable;

			first = true;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			InitializeComponent();
	
			if (nuova == false)
			{
				if (!editable == true)
				{
					//section_num.IsEnabled = false;
				}
				else
				{
					//section_cli.IsEnabled = false;
					//section_dst.IsEnabled = false;
					//section_num.IsEnabled = false;
					//section_data.IsEnabled = false;
				}
			}

		}
		protected override async void OnAppearing()
		{
			if (first)
			{
				first = false;
				if (doc_.fat_inte != 0)
				{
					try
					{
						cli_ = await dbcon_.GetAsync<Clienti>(doc_.fat_inte);
					}
					catch (System.Exception)
					{
						await DisplayAlert("Attenzione", "Cliente non trovato", "OK");
					}
					if (doc_.fat_dest != 0)
					{
						try
						{
							dst_ = await dbcon_.GetAsync<Destinazioni>(doc_.fat_dest);
						}
						catch (System.Exception)
						{
							await DisplayAlert("Attenzione", "Destinazione non trovata", "OK");
						}
					}
				}
				SetField();
			}
			base.OnAppearing();
		}

		public void SetField()
		{
			m_cli_cod.Text = "0";
			m_dst_cod.Text = "0";
			if (cli_ != null)
			{
				m_cli_cod.Text = cli_.cli_codice.ToString();
				cli_desc.Text = cli_.cli_desc;
				cli_indirizzo.Text = cli_.cli_indirizzo;
				cli_citta.Text = cli_.cli_citta;
			}
			if (dst_ != null)
			{
				m_dst_cod.Text = dst_.dst_codice.ToString();
				dst_desc.Text = dst_.dst_desc;	
				dst_desc.Text = dst_.dst_desc;
				dst_indirizzo.Text = dst_.dst_indirizzo;
				dst_citta.Text = dst_.dst_citta;
			}
			fat_n_doc.Value = doc_.fat_n_doc % 700000000;
			fat_d_doc.Date = doc_.fat_d_doc;
			fat_registro.Text = doc_.fat_registro;

		}

		public void GetField()
		{
			doc_.fat_inte = Int32.Parse(m_cli_cod.Text);
			doc_.fat_dest = Int32.Parse(m_dst_cod.Text);
			//doc_.fat_n_doc = fat_n_doc.Value + () % 700000000 +;
			doc_.fat_registro = fat_registro.Text;
			doc_.fat_d_doc = fat_d_doc.Date;
		}

		void OnClienteTapped(object sender, System.EventArgs e)
		{
			var page = new ClientiSearch();
			page.CliList.ItemDoubleTapped += (source, args) =>
			{
				cli_ = (Clienti)args.ItemData;
				SetField();
				Navigation.PopAsync();
			};
			Navigation.PushAsync(page);
		}

		void OnDestinazioneTapped(object sender, System.EventArgs e)
		{
			var page = new DestinazioniSearch(cli_ != null ? cli_.cli_codice : 0);
			page.DstList.ItemDoubleTapped += (source, args) =>
			{
				dst_ = (Destinazioni)args.ItemData;
				SetField();
				Navigation.PopAsync();
			};
			Navigation.PushAsync(page);
		}

		async void OnCliCodUnfocused(object sender, Xamarin.Forms.FocusEventArgs e)
		{
			GetField();
			cli_ = null;
			dst_ = null;
			if (doc_.fat_inte != 0)
			{
				try
				{
					cli_ = await dbcon_.GetAsync<Clienti>(doc_.fat_inte);
				}
				catch (System.Exception)
				{
					
				}
				if (doc_.fat_dest != 0)
				{
					try
					{
						dst_ = await dbcon_.GetAsync<Destinazioni>(doc_.fat_dest);
						if (dst_.dst_cli_for != cli_.cli_codice || dst_.dst_rel != 0) dst_ = null;
					}
					catch (System.Exception)
					{
						//
					}
				}
			}
			SetField();
		}

		void OnDstCodUnfocused(object sender, Xamarin.Forms.FocusEventArgs e)
		{
			DisplayAlert("Attenzione!", "Da Implementare", "OK");
		}

		void OnClickPrec(object sender, System.EventArgs e)
		{
			if (nuova_) return;

			DisplayAlert("Attenzione!", "Da Implementare", "OK");

		}
	}
}

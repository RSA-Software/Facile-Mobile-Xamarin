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
			MessagingCenter.Subscribe<ClientiSearch, Clienti>(this, "ClienteChanged", OnClienteChanged);

			if (nuova == false)
			{
				if (editable == true)
				{
					section_num.IsEnabled = false;
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
				try
				{
					cli_ = await dbcon_.GetAsync<Clienti>(doc_.fat_inte);	
				}
				catch (System.Exception)
				{
					await DisplayAlert("Attenzione","Cliente non trovato","OK");
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
				SetField();
			}
			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			MessagingCenter.Unsubscribe<ClientiSearch>(this,"ClienteChanged");
			base.OnDisappearing();
		}

		public void SetField()
		{
			if (cli_ != null)
			{
				cli_desc.Text = cli_.cli_desc;
				cli_indirizzo.Text = cli_.cli_indirizzo;
				cli_citta.Text = cli_.cli_citta;
			}
			if (dst_ != null)
			{
				dst_desc.Text = dst_.dst_desc;	
			}
			fat_n_doc.Value = doc_.fat_n_doc % 700000000;
			fat_d_doc.Date = doc_.fat_d_doc;
			fat_registro.Text = doc_.fat_registro;

		}

		async void OnClienteTapped(object sender, System.EventArgs e)
		{
			await Navigation.PushAsync(new ClientiSearch());
		}

		void OnClienteChanged(ClientiSearch source, Clienti cli)
		{
			cli_ = cli;
			if (cli_ != null)
			{
				
			}
			SetField();
		}


		void OnDestinazioneTapped(object sender, System.EventArgs e)
		{
			DisplayAlert("Tapped", "Destinazione", "ok");
		}

	}
}

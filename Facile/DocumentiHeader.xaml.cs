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
		private bool first;
		private readonly SQLiteAsyncConnection dbcon_;
		private Clienti cli_ = null;
		private Destinazioni dst_ = null;

		public DocumentiHeader(ref Fatture f)
		{
			doc_ = f;
			first = true;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			InitializeComponent();		
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

		public void SetField()
		{
			if (cli_ != null)
			{
				cli_desc.Text = cli_.cli_desc;
			}
			if (dst_ != null)
			{
				dst_desc.Text = dst_.dst_desc;	
			}
			fat_d_doc.Date = doc_.fat_d_doc;
			fat_registro.Text = doc_.fat_registro;

		}

	}
}

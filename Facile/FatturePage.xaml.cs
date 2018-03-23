using System;
using System.Collections.Generic;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Facile.Extension.FattureExtensions;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FatturePage : ContentPage
	{
		private readonly TipoDocumento tipo_;
		public FatturePage(TipoDocumento t_doc)
		{
			tipo_ = t_doc;
			InitializeComponent();

			switch(tipo_)
			{
				case TipoDocumento.TIPO_FAT : 
					m_title.Text = "FATTURE";
					break;

				case TipoDocumento.TIPO_ORD:
					m_title.Text = "ORDINI";
					break;

				case TipoDocumento.TIPO_DDT:
					m_title.Text = "DOC. DI TRASPORTO";
					break;

				default:
					m_title.Text = " * * *  SCONOSCIUTO * * *";
					break;
			}

		}

		async void OnAggiungiClicked(object sender, System.EventArgs e)
		{
			var fat = new Fatture();
			SQLiteAsyncConnection dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			bool nuova = true;
			bool editable = true;

			//
			// Inizializziamo il documento
			//

			fat.fat_n_doc = 1;
			fat.fat_tipo = (short)tipo_;
			fat.fat_registro = "A";
			fat.fat_d_doc = DateTime.Now;
			fat.fat_editable = true;

			try
			{
				var sql = string.Format("SELECT * from FATTURE2 WHERE fat_tipo = {0} ORDER BY fat_n_doc DESC LIMIT 1", (short)tipo_);
				var docList = await dbcon_.QueryAsync<Fatture>(sql);
				foreach (var doc in docList)
				{
					fat.fat_n_doc = doc.fat_n_doc + 1;
					break;
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				return;
			}
			editable = fat.fat_editable;
			var page = new DocumentiEdit(ref fat, ref nuova, ref editable);
			await Navigation.PushAsync(page);
		}

		async void OnModificaClicked(object sender, System.EventArgs e)
		{
			Fatture fat = null;
			SQLiteAsyncConnection dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			bool nuova = false;
			bool editable = false;

			try
			{
				var sql = string.Format("SELECT * from FATTURE2 WHERE fat_tipo = {0} ORDER BY fat_n_doc DESC LIMIT 1",(short)tipo_);
				var docList = await dbcon_.QueryAsync<Fatture>(sql);
				foreach (var doc in docList)
				{
					fat = doc;
					break;
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				return;
			}
			if (fat == null) 
			{
				await DisplayAlert("Attenzione!", "Non è stato trovato in archivio alcun documento", "Ok");
				return;
			}
			editable = fat.fat_editable;
			var page = new DocumentiEdit(ref fat, ref nuova, ref editable);
			await Navigation.PushAsync(page);
		}

		async void OnElencoClicked(object sender, System.EventArgs e)
		{
			var page = new DocumentiGrid(tipo_);
			await Navigation.PushAsync(page);
		}
	}
}

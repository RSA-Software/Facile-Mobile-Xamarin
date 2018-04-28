using System;
using System.Collections.Generic;
using Facile.Interfaces;
using Facile.Models;
using Facile.Utils;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Facile.Extension.FattureExtensions;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FatturePage : ContentPage
	{
		private readonly DocTipo tipo_;
		public FatturePage(DocTipo t_doc)
		{
			tipo_ = t_doc;
			InitializeComponent();

			switch(tipo_)
			{
				case DocTipo.TIPO_FAT : 
					m_title.Text = "FATTURE";
					break;

				case DocTipo.TIPO_ORD:
					m_title.Text = "ORDINI";
					break;

				case DocTipo.TIPO_DDT:
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
			LocalImpo lim = null;
			Agenti age = null;

			SQLiteAsyncConnection dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			//
			// Leggiamo le impostazioni
			//
			try
			{
				lim = await dbcon_.GetAsync<LocalImpo>(1);
			}
			catch
			{
				await DisplayAlert("Attenzione!", "Impostazioni locali non trovate!\nRiavviare l'App.", "OK");
				return;
			}

			if (!lim.data_download)
			{
				await DisplayAlert("Attenzione!", "Dati non presenti sul dispositivo!\nPer procedere è necessario scaricare i dati dal server.", "OK");
				return;
			}
			if (string.IsNullOrWhiteSpace(lim.registro))
			{
				await DisplayAlert("Attenzione!", "Registro non impostato!\nPer inserire documenti è necessario fare le impostazioni iniziali.", "OK");
				return;
			}
			if (lim.age == 0)
			{
				await DisplayAlert("Attenzione!", "Agente non impostato!\nPer inserire documenti è necessario fare le impostazioni iniziali.", "OK");
				return;
			}

			//
			// Leggiamo i dati dell' agente
			//
			try
			{
				age = await dbcon_.GetAsync<Agenti>(lim.age);
			}
			catch
			{
				await DisplayAlert("Attenzione!", "L' Agente impostato non è presente in archivio!", "OK");
				return;
			}




			bool nuova = true;

			//
			// Inizializziamo il documento
			//
			fat.fat_n_doc = 1;
			fat.fat_tipo = (short)tipo_;
			fat.fat_registro = lim.registro;
			fat.fat_d_doc = DateTime.Now;
			fat.fat_editable = true;
			fat.fat_local_doc = true;
			fat.fat_age = lim.age;

			try
			{
				fat.fat_n_doc = 1;
				var sql = string.Format("SELECT * FROM fatture2 WHERE fat_tipo = {0} AND fat_n_doc > {1} AND fat_n_doc <= {2} ORDER BY fat_n_doc DESC LIMIT 1", (short)tipo_, RsaUtils.GetFirstRegNumber(lim.registro), RsaUtils.GetLastRegNumber(lim.registro));
				var list = await dbcon_.QueryAsync<Fatture>(sql);
				foreach (var doc in list)
				{
					fat.fat_n_doc += doc.fat_n_doc;
					break;
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				return;
			}
			var page = new DocumentiEdit(ref fat, ref nuova);
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
				var sql = string.Format("SELECT * from FATTURE2 WHERE fat_tipo = {0} ORDER BY fat_tipo, fat_n_doc DESC LIMIT 1",(short)tipo_);
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
			var page = new DocumentiEdit(ref fat, ref nuova);
			await Navigation.PushAsync(page);
		}

		async void OnElencoClicked(object sender, System.EventArgs e)
		{
			var page = new DocumentiGrid(tipo_);
			await Navigation.PushAsync(page);
		}
	}
}

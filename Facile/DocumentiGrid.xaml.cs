using System;
using System.Collections.ObjectModel;
using Facile.Interfaces;
using Facile.Models;
using Facile.ViewModels;
using SQLite;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;
using static Facile.Extension.FattureExtensions;

namespace Facile
{
	//[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DocumentiGrid : ContentPage
	{
		private readonly DocTipo tipo_;
		private readonly SQLiteAsyncConnection dbcon_;
		private string query_;
		private string filter_;
		private int swipeIndex;
		private Documents swipeDoc;
		private Image leftImage;
		private int cliCodice_;

		ObservableCollection<Documents> docCollection = null;

		public DocumentiGrid(DocTipo t_doc)
		{
			InitializeComponent();
			tipo_ = t_doc;
			cliCodice_ = 0;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			busyIndicator.IsBusy = true;
			leftImage = null;
			swipeDoc = null;
			swipeIndex = 0;
			switch(tipo_)
			{
				case DocTipo.TIPO_DDT :
					Title = "Documenti di Trasporto";
					break;

				case DocTipo.TIPO_FAT:
					Title = "Fatture";
					break;

				case DocTipo.TIPO_PRE:
					Title = "Preventivi";
					break;

				case DocTipo.TIPO_ORD:
					Title = "Ordini";
					break;

				default:
					Title = "*** Documento Sconosciuto ***";
					break;
			}
			//
			// Inserire Massimo e minimo per le date dopo aver impostato l'anno nelle impostazioni ditta
			// 
			dStart.Date = new DateTime(2016, 1, 1); // DateTime.Now;
			dStop.Date = DateTime.Now;

			query_ = "SELECT fat_tipo, fat_n_doc, fat_d_doc, fat_tot_fattura, fat_registro, cli_desc " +
				"FROM fatture2 " +
				"LEFT JOIN clienti1 on fat_inte = cli_codice";
			
			filter_ = String.Format(" WHERE fat_tipo = {0}", (int)tipo_);

			dataGrid.ColumnSizer = Syncfusion.SfDataGrid.XForms.ColumnSizer.LastColumnFill;
			dataGrid.GridLongPressed += DataGrid_GridLongPressed; 

			GridTableSummaryRow summaryRow1 = new GridTableSummaryRow();
			summaryRow1.Title = "Totale {Totale} - Numero Doc. : {DocCount}";
			summaryRow1.ShowSummaryInRow = true;
			summaryRow1.Position = Position.Bottom;
			summaryRow1.SummaryColumns.Add(new GridSummaryColumn()
			{
				Name = "Totale",
				MappingName = "fat_tot_fattura",
				Format = "{Sum:c}",
				SummaryType = SummaryType.DoubleAggregate
			});
			summaryRow1.SummaryColumns.Add(new GridSummaryColumn()
			{
				Name = "DocCount",
				MappingName = "fat_tot_fattura",
				Format = "{Count:#,#}",
				SummaryType = SummaryType.CountAggregate
			});
			dataGrid.TableSummaryRows.Add(summaryRow1);

		}

		async protected override void OnAppearing()
		{
			string sql = query_ + filter_;
			var docList = await dbcon_.QueryAsync<Documents>(sql);
			foreach(Documents doc in docList)
			{
				if (doc.fat_tipo == (int)DocTipo.TIPO_FAT && doc.fat_credito != 0)
					doc.fat_tot_fattura = -doc.fat_tot_fattura;
			}
			if (docList.Count == 0)
			{
				await DisplayAlert("Attenzione!", "Dati non trovati", "OK");
				await Navigation.PopAsync();
			}
			else
			{
				docCollection = new ObservableCollection<Documents>(docList);
				dataGrid.ItemsSource = docCollection;
			}
			busyIndicator.IsBusy = false;
			base.OnAppearing();
		}

		async void OnDateSelected(object sender, Xamarin.Forms.DateChangedEventArgs e)
		{
			if (!String.IsNullOrEmpty(query_))
			{
				busyIndicator.IsBusy = true;
				string where;
				if (cliCodice_ != 0)
					where = String.Format(" AND fat_d_doc BETWEEN {0} AND {1} AND fat_inte = {2}", dStart.Date.Ticks,dStop.Date.Ticks, cliCodice_);
				else 
					where = String.Format(" AND fat_d_doc BETWEEN {0} AND {1}", dStart.Date.Ticks, dStop.Date.Ticks);
				string sql = query_ + filter_ + where;
				var docList = await dbcon_.QueryAsync<Documents>(sql);
				foreach (Documents doc in docList)
				{
					if (doc.fat_tipo == (int)DocTipo.TIPO_FAT && doc.fat_credito != 0)
						doc.fat_tot_fattura = -doc.fat_tot_fattura;
				}
				docCollection = new ObservableCollection<Documents>(docList);
				dataGrid.ItemsSource = docCollection;
				busyIndicator.IsBusy = false;
			}
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);

			var cols = dataGrid.Columns;
			if (width > height)
			{
				foreach (var col in cols)
				{
					if (col.HeaderText == "Tipo") col.IsHidden = false;
				}
			}
			else
			{
				foreach (var col in cols)
				{
					if (col.HeaderText == "Tipo") col.IsHidden = true;
				}
			}
		}

		async void DataGrid_GridLongPressed(object sender, Syncfusion.SfDataGrid.XForms.GridLongPressedEventArgs e)
		{
			var doc = e.RowData as Documents;
			busyIndicator.IsBusy = true;

			Fatture fat = null;
			bool nuova = false;
			try
			{
				string sql = String.Format("SELECT * from FATTURE2 WHERE fat_tipo = {0} AND fat_n_doc = {1} LIMIT 1", doc.fat_tipo, doc.fat_n_doc);
				var docList = await dbcon_.QueryAsync<Fatture>(sql);
				if (docList.Count > 0)
					fat = docList[0];
			}
			catch (Exception ex)
			{
				busyIndicator.IsBusy = false;
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				return;
			}
			if (fat == null) 
			{
				busyIndicator.IsBusy = false;
				return;
			}
			var page = new DocumentiEdit(ref fat, ref nuova);
			await Navigation.PushAsync(page);
			busyIndicator.IsBusy = false;
		}

		void OnSwipeStarted(object sender, Syncfusion.SfDataGrid.XForms.SwipeStartedEventArgs e)
		{
			swipeIndex = e.RowIndex;
			swipeDoc = (Documents)e.RowData;
		}


		void OnLeftBindingContextChanged(object sender, System.EventArgs e)
		{
			if (leftImage == null)
			{
				leftImage = sender as Image;
				(leftImage.Parent as View).GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(Edit) });
			}
		}

		private async void Edit ()
		{
			dataGrid.ResetSwipeOffset();
			if (swipeIndex == 0 || swipeDoc == null)
			{
				return;
			}

			Fatture fat = null;
			bool nuova = false;

			swipeIndex = 0;
			try
			{
				string sql = String.Format("SELECT * from FATTURE2 WHERE fat_tipo = {0} AND fat_n_doc = {1} LIMIT 1", swipeDoc.fat_tipo, swipeDoc.fat_n_doc);
				var docList = await dbcon_.QueryAsync<Fatture>(sql);
				if (docList.Count > 0)
					fat = docList[0];
			}
			catch (Exception ex)
			{
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				return;
			}
			if (fat == null) return;
			var page = new DocumentiEdit(ref fat, ref nuova);
			await Navigation.PushAsync(page);
		}
	}
}

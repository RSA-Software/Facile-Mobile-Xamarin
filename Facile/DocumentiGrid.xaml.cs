using System;
using System.Collections.ObjectModel;
using Facile.Interfaces;
using Facile.ViewModels;
using SQLite;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;

namespace Facile
{
	public enum TipoDocumento
	{
		TIPO_FAT = 0,
		TIPO_BOL = 1,
		TIPO_DDT = 2,
		TIPO_BUO = 3,
		TIPO_ACC = 4,
		TIPO_RIC = 5,
		TIPO_PRE = 6,
		TIPO_ORD = 7,
		TIPO_FAR = 8,
		TIPO_OFO = 9,
		TIPO_AUF = 10,
		TIPO_RIO = 11,
		TIPO_DRI = 12,
		TIPO_FPF = 13,
	}

	//[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DocumentiGrid : ContentPage
	{
		private readonly TipoDocumento tipo_;
		private readonly SQLiteAsyncConnection dbcon_;
		private string query_;
		private string filter_;

		ObservableCollection<Documents> docCollection = null;

		public DocumentiGrid(TipoDocumento t_doc)
		{
			InitializeComponent();
			tipo_ = t_doc;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			switch(tipo_)
			{
				case TipoDocumento.TIPO_DDT :
					Title = "Documenti di Trasporto";
					break;

				case TipoDocumento.TIPO_FAT:
					Title = "Fatture";
					break;

				case TipoDocumento.TIPO_PRE:
					Title = "Preventivi";
					break;

				case TipoDocumento.TIPO_ORD:
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
				if (doc.fat_tipo == (int)TipoDocumento.TIPO_FAT && doc.fat_credito != 0)
					doc.fat_tot_fattura = -doc.fat_tot_fattura;
			}
			docCollection =  new ObservableCollection<Documents>(docList);
			dataGrid.ItemsSource = docCollection;

			base.OnAppearing();
		}

		async void OnDateSelected(object sender, Xamarin.Forms.DateChangedEventArgs e)
		{
			if (!String.IsNullOrEmpty(query_))
			{
				string where = String.Format(" AND fat_d_doc BETWEEN {0} AND {1}", dStart.Date.Ticks,dStop.Date.Ticks);

				string sql = query_ + filter_ + where;
				var docList = await dbcon_.QueryAsync<Documents>(sql);
				foreach (Documents doc in docList)
				{
					if (doc.fat_tipo == (int)TipoDocumento.TIPO_FAT && doc.fat_credito != 0)
						doc.fat_tot_fattura = -doc.fat_tot_fattura;
				}
				docCollection = new ObservableCollection<Documents>(docList);
				dataGrid.ItemsSource = docCollection;
			}
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);
					    
			if (Device.Idiom != TargetIdiom.Phone) return;
			var cols = dataGrid.Columns;
			if (width > height)
			{
				foreach (var col in cols)
				{
					if (col.HeaderText == "Tipo")
						col.IsHidden = false;
				}
			}
			else
			{
				foreach (var col in cols)
				{
					if (col.HeaderText == "Tipo")
						col.IsHidden = true;
				}
			}
		}
	}
}

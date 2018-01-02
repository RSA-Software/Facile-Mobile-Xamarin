using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Facile.Extension;
using Facile.Interfaces;
using Facile.ViewModels;
using SQLite;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScadenzeElenco : ContentPage
	{
		private readonly SQLiteAsyncConnection dbcon_;

		public ScadenzeElenco()
		{
			InitializeComponent();
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			dataGrid.ColumnSizer = Syncfusion.SfDataGrid.XForms.ColumnSizer.LastColumnFill;


			GridTableSummaryRow summaryRow1 = new GridTableSummaryRow();
			summaryRow1.Title = "Totale {Totale} - Numero Cli. : {CliCount}";
			summaryRow1.ShowSummaryInRow = true;
			summaryRow1.Position = Position.Bottom;
			summaryRow1.SummaryColumns.Add(new GridSummaryColumn()
			{
				Name = "Totale",
				MappingName = "CliTotale",
				Format = "{Sum:c}",
				SummaryType = SummaryType.DoubleAggregate
			});
			summaryRow1.SummaryColumns.Add(new GridSummaryColumn()
			{
				Name = "CliCount",
				MappingName = "CliDesc",
				Format = "{Count:#,#}",
				SummaryType = SummaryType.CountAggregate
			});
			dataGrid.TableSummaryRows.Add(summaryRow1);
		}

		protected async override void OnAppearing()
		{
			var sql = @"SELECT sca_cli_for as CliId, cli_desc as CliDesc, cli_tel as CliTel,  SUM(sca_importo) as CliTotale
				FROM scadenze 
				INNER JOIN clienti1 on cli_codice = sca_cli_for
				WHERE sca_relaz = 0 AND sca_pagato = 0
				GROUP BY cli_desc, cli_tel, sca_cli_for";

			busyIndicator.IsBusy = true;
			dataGrid.IsBusy = true;
			var scaList = await dbcon_.QueryAsync<ScadenzeInfo>(sql);
			var scaCollection= new ObservableCollection<ScadenzeInfo>(scaList);
			dataGrid.ItemsSource = scaCollection;
			dataGrid.IsBusy = false;
			busyIndicator.IsBusy = false;
			base.OnAppearing();
		}

		async void OnTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			if (e.OldTextValue == e.NewTextValue) return;

			string sql = String.Empty;
			busyIndicator.IsBusy = true;
			dataGrid.IsBusy = true;
			if (String.IsNullOrWhiteSpace(e.NewTextValue))
			{
				sql = @"SELECT sca_cli_for as CliId, cli_desc as CliDesc, cli_tel as CliTel,  SUM(sca_importo) as CliTotale
				FROM scadenze 
				INNER JOIN clienti1 on cli_codice = sca_cli_for
				WHERE sca_relaz = 0 AND sca_pagato = 0
				GROUP BY cli_desc, cli_tel, sca_cli_for";
			}
			else
			{
				sql = @"SELECT sca_cli_for as CliId, cli_desc as CliDesc, cli_tel as CliTel,  SUM(sca_importo) as CliTotale
				FROM scadenze 
				INNER JOIN clienti1 on cli_codice = sca_cli_for
				WHERE sca_relaz = 0 AND sca_pagato = 0" + " AND cli_desc LIKE(" + e.NewTextValue.Trim().SqlQuote(true) + ")"  + " GROUP BY cli_desc, cli_tel, sca_cli_for";
			}

			var scaList = await dbcon_.QueryAsync<ScadenzeInfo>(sql);
			var scaCollection = new ObservableCollection<ScadenzeInfo>(scaList);

			dataGrid.ItemsSource = scaCollection;
			dataGrid.IsBusy = false;
			busyIndicator.IsBusy = false;
		}

		void OnSwipeStarted(object sender, Syncfusion.SfDataGrid.XForms.SwipeStartedEventArgs e)
		{
			throw new NotImplementedException();
		}

		void OnLeftBindingContextChanged(object sender, System.EventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}

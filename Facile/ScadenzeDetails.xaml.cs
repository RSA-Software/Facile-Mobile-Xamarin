using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Facile.Interfaces;
using Facile.Models;
using Facile.ViewModels;
using SQLite;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScadenzeDetails : ContentPage
	{
		private readonly ScadenzeInfo info_;
		private readonly SQLiteAsyncConnection dbcon_;

		public ScadenzeDetails(ScadenzeInfo info)
		{
			info_ = info;
			InitializeComponent();
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			Title = info.CliDesc;

			GridTableSummaryRow summaryRow1 = new GridTableSummaryRow();
			summaryRow1.Title = "Totale {Totale} - Num. : {ScaCount}";
			summaryRow1.ShowSummaryInRow = true;
			summaryRow1.Position = Position.Bottom;
			summaryRow1.SummaryColumns.Add(new GridSummaryColumn()
			{
				Name = "Totale",
				MappingName = "sca_importo",
				Format = "{Sum:c}",
				SummaryType = SummaryType.DoubleAggregate
			});
			summaryRow1.SummaryColumns.Add(new GridSummaryColumn()
			{
				Name = "ScaCount",
				MappingName = "sca_num",
				Format = "{Count:#,#}",
				SummaryType = SummaryType.CountAggregate
			});
			dataGrid.TableSummaryRows.Add(summaryRow1);
		}

		protected async override void OnAppearing()
		{
			dataGrid.IsBusy = true;
			string sql = String.Format("SELECT * FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} ORDER BY sca_data, sca_num", info_.CliId);
			var scaList = await dbcon_.QueryAsync<Scadenze>(sql);
			var scaCollection = new ObservableCollection<Scadenze>(scaList);
			dataGrid.ItemsSource = scaCollection;
			dataGrid.IsBusy = false;
			base.OnAppearing();
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);
			if (Device.Idiom == TargetIdiom.Phone)
			{
				if (width > height)
				{
					foreach (var col in dataGrid.Columns)
					{
						if (col.HeaderText == "Descrizione") col.IsHidden = false;
					}
				}
				else
				{
					foreach (var col in dataGrid.Columns)
					{
						if (col.HeaderText == "Descrizione") col.IsHidden = true;
					}
				}
			}
		}

	}
}

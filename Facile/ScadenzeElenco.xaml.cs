using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Facile.Extension;
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
	public partial class ScadenzeElenco : ContentPage
	{
		private readonly SQLiteAsyncConnection dbcon_;
		private int swipeIndex_;
		private ScadenzeInfo swipeData_;
		private Image leftImage;
		private bool first;

		public ScadenzeElenco()
		{
			InitializeComponent();
			swipeIndex_ = 0;
			swipeData_ = null;
			leftImage = null;
			first = true;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			dataGrid.ColumnSizer = Syncfusion.SfDataGrid.XForms.ColumnSizer.LastColumnFill;
			dataGrid.GridLongPressed += DataGrid_GridLongPressed;

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

			if (Device.Idiom == TargetIdiom.Phone && Device.RuntimePlatform == Device.Android)
			{
				searchBar.HeightRequest = 40;
			}
		}

		protected async override void OnAppearing()
		{
			if (first)
			{
				var sql = @"SELECT sca_cli_for as CliId, cli_desc as CliDesc, cli_tel as CliTel,  SUM(sca_importo) as CliTotale
				FROM scadenze 
				INNER JOIN clienti1 on cli_codice = sca_cli_for
				WHERE sca_relaz = 0 AND sca_pagato = 0
				GROUP BY cli_desc, cli_tel, sca_cli_for";

				busyIndicator.IsBusy = true;
				dataGrid.IsBusy = true;
				var scaList = await dbcon_.QueryAsync<ScadenzeInfo>(sql);
				var scaCollection = new ObservableCollection<ScadenzeInfo>(scaList);
				dataGrid.ItemsSource = scaCollection;
				dataGrid.IsBusy = false;
				busyIndicator.IsBusy = false;
				first = false;
			}
			base.OnAppearing();		
		}

		async void DataGrid_GridLongPressed(object sender, GridLongPressedEventArgs e)
		{
			var sca = (ScadenzeInfo)e.RowData;
			var page = new ScadenzeDetails(sca);
			await Navigation.PushAsync(page);
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
			swipeIndex_ = e.RowIndex;
			swipeData_ = (ScadenzeInfo)e.RowData;
		}

		void OnLeftBindingContextChanged(object sender, System.EventArgs e)
		{
			if (leftImage == null)
			{
				leftImage = sender as Image;
				(leftImage.Parent as View).GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(ShowDetails) });
			}
		}

		private async void ShowDetails()
		{
			dataGrid.ResetSwipeOffset();
			if (swipeIndex_ == 0) return; 
			if (swipeData_ == null) return;

			try
			{
				string sql = String.Format("SELECT * FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0}", swipeData_.CliId);
				var scaList = await dbcon_.QueryAsync<Scadenze>(sql);
			}
			catch (Exception ex)
			{
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				return;
			}

			var page = new ScadenzeDetails(swipeData_);
			await Navigation.PushAsync(page);
		}


	}
}

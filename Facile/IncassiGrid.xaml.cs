using System;
using System.Collections.Generic;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Facile.Models;
using Facile.ViewModels;
using Facile.Interfaces;
using Syncfusion.SfDataGrid.XForms;
using Syncfusion.Data;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class IncassiGrid : ContentPage
	{
		private readonly SQLiteAsyncConnection dbcon_;
		private string query_;
		private int swipeIndex;
		private IncassiGridModel swipeDsp;
		private Image leftImage;

		ObservableCollection<IncassiGridModel> dspCollection = null;

		public IncassiGrid()
		{
			InitializeComponent();

			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			busyIndicator.IsBusy = true;
			leftImage = null;
			swipeDsp = null;
			swipeIndex = 0;

			//
			// Inserire Massimo e minimo per le date dopo aver impostato l'anno nelle impostazioni ditta
			// 
			dStart.Date = new DateTime(2016, 1, 1); // DateTime.Now;
			dStop.Date = DateTime.Now;

			query_ = "SELECT dsp_codice, dsp_clifor, dsp_data, dsp_totale, cli_desc " +
				"FROM scapaghe " +
				"LEFT JOIN clienti1 ON cli_codice = dsp_clifor";

			dataGrid.ColumnSizer = Syncfusion.SfDataGrid.XForms.ColumnSizer.LastColumnFill;
			dataGrid.GridLongPressed += DataGrid_GridLongPressed;

			GridTableSummaryRow summaryRow1 = new GridTableSummaryRow();
			summaryRow1.Title = "Totale {Totale} - Numero Doc. : {DocCount}";
			summaryRow1.ShowSummaryInRow = true;
			summaryRow1.Position = Position.Bottom;
			summaryRow1.SummaryColumns.Add(new GridSummaryColumn()
			{
				Name = "Totale",
				MappingName = "dsp_totale",
				Format = "{Sum:c}",
				SummaryType = SummaryType.DoubleAggregate
			});
			summaryRow1.SummaryColumns.Add(new GridSummaryColumn()
			{
				Name = "DocCount",
				MappingName = "dsp_totale",
				Format = "{Count:#,#}",
				SummaryType = SummaryType.CountAggregate
			});
			dataGrid.TableSummaryRows.Add(summaryRow1);
		}

		protected async override void OnAppearing()
		{
			//
			// Prendiamo la data della prima Distinta
			//
			try
			{
				var d_start = await dbcon_.ExecuteScalarAsync<DateTime>("SELECT MIN(dsp_data) FROM scapaghe WHERE dsp_data IS NOT NULL");
				var d_stop = await dbcon_.ExecuteScalarAsync<DateTime>("SELECT MAX(dsp_data) FROM scapaghe WHERE dsp_data IS NOT NULL");

				dStart.Date = d_start;
				dStop.Date = d_stop;

				var dspList = await dbcon_.QueryAsync<IncassiGridModel>(query_);

				if (dspList.Count == 0)
				{
					await DisplayAlert("Attenzione!", "Dati non trovati", "OK");
					await Navigation.PopAsync();
				}
				else
				{
					dspCollection = new ObservableCollection<IncassiGridModel>(dspList);
					dataGrid.ItemsSource = dspCollection;
				}
			}
			catch
			{
				await DisplayAlert("Errore!", "Dati non trovati.", "OK");
				await Navigation.PopAsync();
			}
			busyIndicator.IsBusy = false;
			base.OnAppearing();
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);

			if (Device.Idiom == TargetIdiom.Phone)
			{
				var cols = dataGrid.Columns;
				if (width > height)
				{
					foreach (var col in cols)
					{
						if (col.HeaderText == "Cod.Cli") col.IsHidden = false;
					}
				}
				else
				{
					foreach (var col in cols)
					{
						if (col.HeaderText == "Cod.Cli") col.IsHidden = true;
					}
				}
			}
		}

		async void OnDateSelected(object sender, Xamarin.Forms.DateChangedEventArgs e)
		{
			if (!String.IsNullOrEmpty(query_))
			{
				busyIndicator.IsBusy = true;
				string where;
				where = String.Format(" WHERE dsp_data BETWEEN {0} AND {1}", dStart.Date.Ticks, dStop.Date.Ticks);
				string sql = query_ + where;
				var dspList = await dbcon_.QueryAsync<IncassiGridModel>(sql);
				dspCollection = new ObservableCollection<IncassiGridModel>(dspList);
				dataGrid.ItemsSource = dspCollection;
				busyIndicator.IsBusy = false;
			}
		}

		async void DataGrid_GridLongPressed(object sender, Syncfusion.SfDataGrid.XForms.GridLongPressedEventArgs e)
		{
			var igm = e.RowData as IncassiGridModel;
			busyIndicator.IsBusy = true;

			ScaPagHead dsp = null;
			try
			{
				dsp = await dbcon_.GetAsync<ScaPagHead>(igm.dsp_codice);
			}
			catch (Exception ex)
			{
				busyIndicator.IsBusy = false;
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				return;
			}
			if (dsp == null)
			{
				busyIndicator.IsBusy = false;
				return;
			}
			await DisplayAlert("Attenzione", "Guardiamo il dettaglio della distinta", "OK");
			//var page = new DocumentiEdit(ref fat, ref nuova);
			//await Navigation.PushAsync(page);
			busyIndicator.IsBusy = false;
		}


		void OnSwipeStarted(object sender, Syncfusion.SfDataGrid.XForms.SwipeStartedEventArgs e)
		{
			swipeIndex = e.RowIndex;
			swipeDsp = (IncassiGridModel)e.RowData;
		}


		void OnLeftBindingContextChanged(object sender, System.EventArgs e)
		{
			if (leftImage == null)
			{
				leftImage = sender as Image;
				(leftImage.Parent as View).GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(Edit) });
			}
		}

		private async void Edit()
		{
			dataGrid.ResetSwipeOffset();
			if (swipeIndex == 0 || swipeDsp == null)
			{
				return;
			}

			ScaPagHead dsp = null;

			swipeIndex = 0;
			try
			{
				dsp = await dbcon_.GetAsync<ScaPagHead>(swipeDsp.dsp_codice);
			}
			catch (Exception ex)
			{
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				return;
			}
			if (dsp == null) return;
			await DisplayAlert("Attenzione", "Guardiamo il dettaglio della distinta", "OK");
			//var page = new DocumentiEdit(ref fat, ref nuova);
			//await Navigation.PushAsync(page);
		}

	}
}

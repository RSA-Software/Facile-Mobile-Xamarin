using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Incassi : ContentPage
	{
		private Clienti _cli;
		private Destinazioni _dst;
		private readonly SQLiteAsyncConnection _dbcon;

		public Incassi(Clienti cli, Destinazioni dst)
		{
			_dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();

			_cli = cli;
			_dst = dst;
			InitializeComponent();

			m_cli_cod.Text = _cli != null ? _cli.cli_codice.ToString() : "";
			m_cli_des.Text = _cli != null ? _cli.cli_desc : "";

			m_dst_cod.Text = _dst != null ? _dst.dst_codice.ToString() : "";
			m_dst_des.Text = _dst != null ? _dst.dst_desc : "";

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
			List<Scadenze> scaList = new List<Scadenze>();
			dataGrid.IsBusy = true;
			if (_cli != null)
			{
				string sql = "";
				if (_dst == null)
					sql = String.Format("SELECT * FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} ORDER BY sca_data, sca_num", _cli.cli_codice);
				else
					sql = String.Format("SELECT * FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} AND sca_dst = {1} ORDER BY sca_data, sca_num", _cli.cli_codice, _dst.dst_codice);
				scaList = await _dbcon.QueryAsync<Scadenze>(sql);
			}
			var scaCollection = new ObservableCollection<Scadenze>(scaList);
			dataGrid.ItemsSource = scaCollection;
			dataGrid.IsBusy = false;
			base.OnAppearing();
		}

		async void OnClientiTapped(object sender, System.EventArgs e)
		{
			var page = new ClientiSearch();
			page.CliList.ItemDoubleTapped += async (source, args) =>
			{
				var cli = (Clienti)args.ItemData;
				m_cli_cod.Text = cli.cli_codice.ToString();
				m_cli_des.Text = cli.cli_desc;
				await  Navigation.PopAsync();

				if ((_cli == null) || (_cli.cli_codice != cli.cli_codice))
				{
					m_dst_cod.Text = "";
					m_dst_des.Text = "";
					_cli = cli;
					_dst = null;
					dataGrid.IsBusy = true;
					string sql = String.Format("SELECT * FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} ORDER BY sca_data, sca_num", cli.cli_codice);
					var scaList = await _dbcon.QueryAsync<Scadenze>(sql);
					var scaCollection = new ObservableCollection<Scadenze>(scaList);
					dataGrid.ItemsSource = scaCollection;
					dataGrid.IsBusy = false;
				}
			};
			await Navigation.PushAsync(page);
		}

		void Handle_Clicked(object sender, System.EventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}

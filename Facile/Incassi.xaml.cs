using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Incassi : ContentPage
	{
		private Clienti _cli;
		private Destinazioni _dst;
		private bool _cli_changed;
		private bool _dst_changed;
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

			_cli_changed = false;
			_dst_changed = false;
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

		void OnClientiTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			_cli_changed = true;
		}

		void OnDestinazioniTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			_dst_changed = true;
		}

		async void OnClientiClicked(object sender, System.EventArgs e)
		{
			var page = new ClientiSearch();
			page.CliList.ItemDoubleTapped += async (source, args) =>
			{
				var cli = (Clienti)args.ItemData;
				m_cli_cod.Text = cli.cli_codice.ToString();
				m_cli_des.Text = cli.cli_desc;
				await Navigation.PopAsync();

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
				_cli_changed = false;
				_dst_changed = false;
			};
			await Navigation.PushAsync(page);
		}

		async void OnDestinazioniClicked(object sender, System.EventArgs e)
		{
			if ((_cli == null) || (_cli.cli_codice == 0)) return;

			var recTotal_ = 0;
			string sql = "SELECT COUNT(*) FROM destina1 WHERE dst_rel = 0 AND dst_cli_for = " + _cli.cli_codice;
			recTotal_ = await _dbcon.ExecuteScalarAsync<int>(sql);
			if (recTotal_ == 0)
			{
				await DisplayAlert("Attenzione!", "Non ci sono destinazioni per il Cliente selezionato", "OK");
				return;
			}
			var page = new DestinazioniSearch(_cli != null ? _cli.cli_codice : 0);
			page.DstList.ItemDoubleTapped += async (source, args) =>
			{
				var dst = (Destinazioni)args.ItemData;
				m_dst_cod.Text = dst.dst_codice.ToString();
				m_dst_des.Text = dst.dst_desc;
				await Navigation.PopAsync();
				if ((_dst == null) || (_dst.dst_codice != dst.dst_codice))
				{
					_dst = dst;
					dataGrid.IsBusy = true;
					sql = String.Format("SELECT * FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} AND sca_dst = {1} ORDER BY sca_data, sca_num", _cli.cli_codice, _dst.dst_codice);
					var scaList = await _dbcon.QueryAsync<Scadenze>(sql);
					var scaCollection = new ObservableCollection<Scadenze>(scaList);
					dataGrid.ItemsSource = scaCollection;
					dataGrid.IsBusy = false;
				}
				_cli_changed = false;
				_dst_changed = false;
			};
			await Navigation.PushAsync(page);
		}

		async void OnClientiUnfocused(object sender, Xamarin.Forms.FocusEventArgs e)
		{
			if (!_cli_changed) return;
			try
			{
				_cli = await _dbcon.GetAsync<Clienti>(Convert.ToInt32(m_cli_cod.Text));
			}
			catch (Exception ex)
			{
				_cli = null;
				Debug.WriteLine(ex.Message);
			}
			if (_dst != null)
			{
				if ((_cli == null) || (_dst.dst_cli_for != _cli.cli_codice) || (_dst.dst_rel != 0)) _dst = null;
			}

			m_cli_cod.Text = _cli != null ? _cli.cli_codice.ToString() : "";
			m_cli_des.Text = _cli != null ? _cli.cli_desc : "";

			m_dst_cod.Text = _dst != null ? _dst.dst_codice.ToString() : "";
			m_dst_des.Text = _dst != null ? _dst.dst_desc : "";

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
		}

		async void OnDestinazioniUnfocused(object sender, Xamarin.Forms.FocusEventArgs e)
		{
			if (!_dst_changed) return;
			try
			{
				if (_cli != null)
				{
					_dst = await _dbcon.GetAsync<Destinazioni>(Convert.ToInt32(m_dst_cod.Text));
					if (_dst.dst_cli_for != _cli.cli_codice || _dst.dst_rel != 0) _dst = null;
				}
				else _dst = null;
			}
			catch (Exception ex)
			{
				_dst = null;
				Debug.WriteLine(ex.Message);
			}
			m_cli_cod.Text = _cli != null ? _cli.cli_codice.ToString() : "";
			m_cli_des.Text = _cli != null ? _cli.cli_desc : "";

			m_dst_cod.Text = _dst != null ? _dst.dst_codice.ToString() : "";
			m_dst_des.Text = _dst != null ? _dst.dst_desc : "";

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
		}

		async void OnSalvaClicked(object sender, System.EventArgs e)
		{
			var recipients = new List<string>();
			recipients.Add("capizz.filippo.rsa@gmail.com");


			try
			{
				var message = new EmailMessage
				{
					Subject = "Incasso",
					Body = "Ho incassato 100.000,00 €",
					To = recipients,
					//Cc = ccRecipients,
					//Bcc = bccRecipients
				};

				await Email.ComposeAsync(message);
			}
			catch (FeatureNotSupportedException fbsEx)
			{
				// Sms is not supported on this device
			}
			catch (Exception ex)
			{
				// Some other exception occured
			}

		}
	}
}


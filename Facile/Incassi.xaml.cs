﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Facile.Interfaces;
using Facile.Models;
using Facile.ViewModels;
using SQLite;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Facile.Extension;

namespace Facile
{

	public class GridTableSummaryCellRendererExt : GridTableSummaryCellRenderer
	{
		public GridTableSummaryCellRendererExt()
		{
		}

		public override void OnInitializeDisplayView(DataColumnBase dataColumn, SfLabel view)
		{
			base.OnInitializeDisplayView(dataColumn, view);
			view.HorizontalTextAlignment = TextAlignment.Center;
			view.BackgroundColor = Color.DarkCyan;
			view.FontSize = 16;
			view.TextColor = Color.White;

		}
	}


	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Incassi : ContentPage
	{
		private Clienti _cli;
		private Destinazioni _dst;
		private bool _cli_changed;
		private bool _dst_changed;
		private bool _on_edit;
		private readonly SQLiteAsyncConnection _dbcon;

		public Incassi(Clienti cli, Destinazioni dst)
		{
			_dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();

			_cli = cli;
			_dst = dst;
			_on_edit = false;
			InitializeComponent();

			m_cli_cod.Text = _cli != null ? _cli.cli_codice.ToString() : "";
			m_cli_des.Text = _cli != null ? _cli.cli_desc : "";

			m_dst_cod.Text = _dst != null ? _dst.dst_codice.ToString() : "";
			m_dst_des.Text = _dst != null ? _dst.dst_desc : "";

			GridTableSummaryRow summaryRow1 = new GridTableSummaryRow();
			summaryRow1.Title = "Totale da Incassare {Totale}  -  Incasso : {Incasso}";
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
				Name = "Incasso",
				MappingName = "sca_incasso",
				Format = "{Sum:c}",
				SummaryType = SummaryType.DoubleAggregate
			});

			dataGrid.TableSummaryRows.Add(summaryRow1);
			dataGrid.LiveDataUpdateMode = LiveDataUpdateMode.AllowSummaryUpdate;

			_cli_changed = false;
			_dst_changed = false;
		}

		protected async override void OnAppearing()
		{
			List<IncassiInfo> scaList = new List<IncassiInfo>();

			dataGrid.IsBusy = true;
			if (_cli != null)
			{
				string sql = "";
				if (_dst == null)
					sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} ORDER BY sca_data, sca_num", _cli.cli_codice);
				else
					sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} AND sca_dst = {1} ORDER BY sca_data, sca_num", _cli.cli_codice, _dst.dst_codice);
				scaList = await _dbcon.QueryAsync<IncassiInfo>(sql);
			}
			var scaCollection = new ObservableCollection<IncassiInfo>(scaList);
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
					string sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} ORDER BY sca_data, sca_num", cli.cli_codice);
					var scaList = await _dbcon.QueryAsync<IncassiInfo>(sql);
					var scaCollection = new ObservableCollection<IncassiInfo>(scaList);
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
					sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} AND sca_dst = {1} ORDER BY sca_data, sca_num", _cli.cli_codice, _dst.dst_codice);
					var scaList = await _dbcon.QueryAsync<IncassiInfo>(sql);
					var scaCollection = new ObservableCollection<IncassiInfo>(scaList);
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

			List<IncassiInfo> scaList = new List<IncassiInfo>();
			dataGrid.IsBusy = true;
			if (_cli != null)
			{
				string sql = "";
				if (_dst == null)
					sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} ORDER BY sca_data, sca_num", _cli.cli_codice);
				else
					sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} AND sca_dst = {1} ORDER BY sca_data, sca_num", _cli.cli_codice, _dst.dst_codice);
				scaList = await _dbcon.QueryAsync<IncassiInfo>(sql);
			}
			var scaCollection = new ObservableCollection<IncassiInfo>(scaList);
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

			var scaList = new List<IncassiInfo>();
			dataGrid.IsBusy = true;
			if (_cli != null)
			{
				string sql = "";
				if (_dst == null)
					sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} ORDER BY sca_data, sca_num", _cli.cli_codice);
				else
					sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} AND sca_dst = {1} ORDER BY sca_data, sca_num", _cli.cli_codice, _dst.dst_codice);
				scaList = await _dbcon.QueryAsync<IncassiInfo>(sql);
			}
			var scaCollection = new ObservableCollection<IncassiInfo>(scaList);
			dataGrid.ItemsSource = scaCollection;
			dataGrid.IsBusy = false;
		}

		void OnGridLongPressed(object sender, Syncfusion.SfDataGrid.XForms.GridLongPressedEventArgs e)
		{
			if (_on_edit) return;
			if (((IncassiInfo)e.RowData).sca_incasso.TestIfZero(2))
				((IncassiInfo)e.RowData).sca_incasso = ((IncassiInfo)e.RowData).sca_importo;
			else
				((IncassiInfo)e.RowData).sca_incasso = 0.0;
		}

		void OnCurrentCellBeginEdit(object sender, Syncfusion.SfDataGrid.XForms.GridCurrentCellBeginEditEventArgs e)
		{
			_on_edit = true;
		}

		void OnCurrentCellEndEdit(object sender, Syncfusion.SfDataGrid.XForms.GridCurrentCellEndEditEventArgs e)
		{
			if (e.NewValue == null) return;

			double val = Convert.ToDouble(e.NewValue);
			var rec = (IncassiInfo)dataGrid.GetRecordAtRowIndex(e.RowColumnIndex.RowIndex);

			if (rec.sca_importo > 0.0)
			{
				if ((val < 0.0) || (val > rec.sca_importo)) e.Cancel = true;
			}
			else
			{
				if ((val > 0.0) || (Math.Abs(val) < Math.Abs(rec.sca_importo))) e.Cancel = true;
			}
			_on_edit = false;
		}

		void Handle_QueryCellStyle(object sender, Syncfusion.SfDataGrid.XForms.QueryCellStyleEventArgs e)
		{
			if (e.ColumnIndex == 2)
			{
				e.Style.BackgroundColor = Color.FromHex("#ffff00");
				e.Style.ForegroundColor = Color.FromHex("#2e7d32");
				e.Style.FontAttribute = FontAttributes.Bold;
				e.Handled = true;
			}
		}

		async void OnSalvaClicked(object sender, System.EventArgs e)
		{
			/*
			var recipients = new List<string>();
			recipients.Add("capizz.filippo.rsa@gmail.com");
			recipients.Add("mariorifici@gmail.com");


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
				await DisplayAlert("Attenzione", "Non supportato : " + fbsEx.Message, "OK");
			}
			catch (Exception ex)
			{
				await DisplayAlert("Attenzione", ex.Message, "OK");
			}*/


			double incasso = 0.0;
			int num_sca = 0;
			foreach(var item in dataGrid.View.Records) 
			{     			
				var rowData = (IncassiInfo)item.Data;

				if (!rowData.sca_incasso.TestIfZero(2))
				{
					incasso += rowData.sca_incasso;
					num_sca++;
				}
			} 
			if (num_sca == 0)
			{
				await DisplayAlert("Attenzione", "Non è stato inserito alcun importo da incassare!", "OK");
				return;
			}
			else
			{
				bool confirm = await DisplayAlert("Attenzione", "Confermi l'incasso di €" + incasso + " ?", "Si", "No");
				if (!confirm) return;
			}

			//
			// Inseriamo la distinta di Incasso
			//



			//
			// Inseriamo le righe incassate
			//


			//
			// Marchiamo le righe come incassate
			//
		}

	}
}


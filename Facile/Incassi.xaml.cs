using System;
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
using Xamarin.Forms.Internals;

namespace Facile
{
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

			if (Device.Idiom != TargetIdiom.Phone)
			{
				m_cli_cod.WidthRequest = 110;
				m_dst_cod.WidthRequest = 110;
			}

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
					sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc, sca_locked FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} ORDER BY sca_data, sca_num", _cli.cli_codice);
				else
					sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc, sca_locked FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} AND sca_dst = {1} ORDER BY sca_data, sca_num", _cli.cli_codice, _dst.dst_codice);
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
					string sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc, sca_locked FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} ORDER BY sca_data, sca_num", cli.cli_codice);
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
					sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc, sca_locked FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} AND sca_dst = {1} ORDER BY sca_data, sca_num", _cli.cli_codice, _dst.dst_codice);
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
					sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc, sca_locked FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} ORDER BY sca_data, sca_num", _cli.cli_codice);
				else
					sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc, sca_locked FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} AND sca_dst = {1} ORDER BY sca_data, sca_num", _cli.cli_codice, _dst.dst_codice);
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
					sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc, sca_locked FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} ORDER BY sca_data, sca_num", _cli.cli_codice);
				else
					sql = String.Format("SELECT sca_id, sca_data, sca_importo, (sca_importo * 0) AS sca_incasso, sca_fattura, sca_tot_fat, sca_desc, sca_locked FROM scadenze WHERE sca_relaz = 0 AND sca_pagato = 0 AND sca_cli_for = {0} AND sca_dst = {1} ORDER BY sca_data, sca_num", _cli.cli_codice, _dst.dst_codice);
				scaList = await _dbcon.QueryAsync<IncassiInfo>(sql);
			}
			var scaCollection = new ObservableCollection<IncassiInfo>(scaList);
			dataGrid.ItemsSource = scaCollection;
			dataGrid.IsBusy = false;
		}

		async void OnGridLongPressed(object sender, Syncfusion.SfDataGrid.XForms.GridLongPressedEventArgs e)
		{
			if (_on_edit) return;

			if (((IncassiInfo)e.RowData).sca_locked != 0)
			{
				await DisplayAlert("Attenzione!", "L scadenza è bloccata e non può essere incassata!", "OK");
				return;
			}
			if (((IncassiInfo)e.RowData).sca_incasso.TestIfZero(2))
				((IncassiInfo)e.RowData).sca_incasso = ((IncassiInfo)e.RowData).sca_importo;
			else
				((IncassiInfo)e.RowData).sca_incasso = 0.0;
		}

		void OnCurrentCellBeginEdit(object sender, Syncfusion.SfDataGrid.XForms.GridCurrentCellBeginEditEventArgs e)
		{
			_on_edit = true;
		}

		async void OnCurrentCellEndEdit(object sender, Syncfusion.SfDataGrid.XForms.GridCurrentCellEndEditEventArgs e)
		{
			if (e.NewValue == null) return;

			double val = Convert.ToDouble(e.NewValue);
			var rec = (IncassiInfo)dataGrid.GetRecordAtRowIndex(e.RowColumnIndex.RowIndex);

			if (rec.sca_locked != 0)
			{
				e.Cancel = true;
				await DisplayAlert("Attenzione!", "L scadenza è bloccata e non può essere incassata!", "OK");
				return;
			}

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

			//if (await DisplayAlert("Attenzione", "Vuoi inviare un messaggio alla sede?", "Si", "No"))
			//{
			//	var recipients = new List<string>();
			//	recipients.Add("capizz.filippo.rsa@gmail.com");
			//	recipients.Add("mariorifici@gmail.com");


			//	try
			//	{
			//		var message = new EmailMessage
			//		{
			//			Subject = "Incasso",
			//			To = recipients,
			//			//Cc = ccRecipients,
			//			//Bcc = bccRecipients
			//		};

			//		message.BodyFormat = EmailBodyFormat.Html;

			//		message.Body = string.Format("<body><h1>Incasso</h1><p>Cliente {0}</p>", m_cli_des.Text);

			//		string str = "<table><tr><td>Data</td><td>Num. Doc.</td><td>Importo Doc</td><td>Importo Scadenza</td><td>Incasso</td></tr>";


			//		message.Body = message.Body + str + "</table></body>";

			//		await Email.ComposeAsync(message);
			//	}
			//	catch (FeatureNotSupportedException fbsEx)
			//	{
			//		await DisplayAlert("Attenzione", "Non supportato : " + fbsEx.Message, "OK");
			//	}
			//	catch (Exception ex)
			//	{
			//		await DisplayAlert("Attenzione", ex.Message, "OK");
			//	}

			//}
			//return;

			double incasso = 0.0;
			int num_sca = 0;
			foreach (var item in dataGrid.View.Records)
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
			// Leggiamo le impostazioni
			//
			var app = (App)Application.Current;
			LocalImpo lim = null;
			try
			{
				lim = await _dbcon.GetAsync<LocalImpo>(1);
			}
			catch
			{
				await DisplayAlert("Attenzione!", "Impostazioni locali non trovate!\nRiavviare l'App.", "OK");
				return;
			}
			if (lim.age == 0)
			{
				await DisplayAlert("Attenzione!", "Codice Agente non impostato!\nEffettuare le impostazioni iniziali.", "OK");
				return;
			}

			//
			// Inseriamo la testata della distinta di Incasso
			//
			var dsp = new ScaPagHead
			{
				dsp_relaz = 0,
				dsp_codice = 0,
				dsp_clifor = Convert.ToInt32(m_cli_cod.Text),
				dsp_data = DateTime.Now,
				dsp_data_sel = null,
				dsp_mas = 0,
				dsp_con = 0,
				dsp_sot = 0,
				dsp_totale = incasso,
				dsp_des_con = "",
				dsp_mezzo = 0, // Cassa
				dsp_codass = 0,
				dsp_oldass = 0,
				dsp_pnota = 0,
				dsp_dst = string.IsNullOrWhiteSpace(m_dst_cod.Text) ? 0 : Convert.ToInt32(m_dst_cod.Text),
				dsp_abbuoni = 0.0,
				dsp_age = lim.age,
				dsp_sez = 1,
				dsp_ass = 0,
				dsp_sez_sca = 0,
				dsp_nonconf = 0,
				dsp_timeid = DateTime.Now,
				dsp_codppc = 0,
				dsp_parked = 0,
				dsp_data_ass = null,
				dsp_user = "",
				dsp_last_update = DateTime.Now,
				dsp_immediato = false,
				dsp_inviato = false
			};

			try
			{
				await _dbcon.InsertAsync(dsp);
			}
			catch (Exception ex)
			{
				await DisplayAlert("Errore", "Impossibile inserire testata : " + ex.Message, "OK");
				return;
			}

			//
			// Inseriamo le righe incassate
			//
			int idx = 0;
			foreach (var item in dataGrid.View.Records)
			{
				idx++;
				var rowData = (IncassiInfo)item.Data;
				if (!rowData.sca_incasso.TestIfZero(2))
				{
					try
					{
						var sca = await _dbcon.GetAsync<Scadenze>(rowData.sca_id);

						var dsr = new ScaPagRow
						{
							dsr_id = 0,
							dsr_relaz = dsp.dsp_relaz,
							dsr_codice = dsp.dsp_codice,
							dsr_rel_sca = sca.sca_relaz,
							dsr_num_sca = sca.sca_num,
							dsr_data = sca.sca_data,
							dsr_old_num_doc = "",
							dsr_d_doc = sca.sca_data_fattura,
							dsr_tot_doc = sca.sca_tot_fat,
							dsr_importo = sca.sca_importo,
							dsr_paginc = rowData.sca_incasso,
							dsr_idx = idx,
							dsr_sez = sca.sca_sez,
							dsr_user = "",
							dsr_last_update = DateTime.Now,
							dsr_num_doc = sca.sca_fattura
						};

						try
						{
							await _dbcon.InsertAsync(dsr);

							var resto = sca.sca_importo - rowData.sca_incasso;
							if (!resto.TestIfZero(2))
							{
								try
								{
									sca.sca_importo = rowData.sca_incasso;
									sca.sca_pagato = 1;
									sca.sca_cont = 1;
									await _dbcon.UpdateAsync(sca);

									sca.sca_id = 0;
									sca.sca_num = 1 + await _dbcon.ExecuteScalarAsync<int>("SELECT MAX(sca_num) FROM scadenze WHERE sca_relaz = 0");
									sca.sca_pagato = 0;
									sca.sca_cont = 0;
									sca.sca_importo = resto;
									sca.sca_locked = 1;
									await _dbcon.InsertAsync(sca);
								}
								catch (Exception ex)
								{
									await DisplayAlert("Errore", "Impossibile aggiornare la scadenza : " + ex.Message, "OK");
								}
							}
							else
							{
								try
								{
									sca.sca_pagato = 1;
									sca.sca_cont = 1;
									await _dbcon.UpdateAsync(sca);
								}
								catch (Exception ex)
								{
									await DisplayAlert("Errore", "Impossibile aggiornare la scadenza : " + ex.Message, "OK");
								}
							}
						}
						catch (Exception ex)
						{
							await DisplayAlert("Errore", "Impossibile inserire riga : " + ex.Message, "OK");
						}
					}
					catch
					{
						await DisplayAlert("Attenzione!", "Scadenza non trovata in archivio!", "OK");
					}
				}
			}


			m_cli_cod.Text = "";
			m_cli_des.Text = "";

			m_dst_cod.Text = "";
			m_dst_des.Text = "";
			dataGrid.ItemsSource = null;

			m_search_cli.Focus();
		}

	}
}


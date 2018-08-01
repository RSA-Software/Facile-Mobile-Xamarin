using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Facile.Models;
using Facile.Interfaces;
using SQLite;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class IncassiModifica : ContentPage
	{
		private bool _first;
		private ScaPagHead _dsp;
		private List<ScaPagRow> _dsrList;
		private Clienti _cli;
		private Destinazioni _dst;
		private readonly SQLiteAsyncConnection _dbcon;

		public IncassiModifica(ref ScaPagHead d)
		{
			InitializeComponent();

			_dsp = d;
			_cli = null;
			_dst = null;
			_dsrList = null;
			_first = true;
			_dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();
		}

		protected async override void OnAppearing()
		{
			if (_first)
			{
				_first = false;
				await LoadRelAsync();
				SetField();
			}
			base.OnAppearing();
		}

		protected async Task LoadRelAsync()
		{
			_cli = null;
			_dst = null;

			if (_dsp.dsp_clifor != 0)
			{
				try
				{
					_cli = await _dbcon.GetAsync<Clienti>(_dsp.dsp_clifor);
				}
				catch (System.Exception)
				{
					await DisplayAlert("Attenzione", "Cliente non trovato", "OK");
				}
				if (_dsp.dsp_dst != 0)
				{
					try
					{
						_dst = await _dbcon.GetAsync<Destinazioni>(_dsp.dsp_dst);
					}
					catch (System.Exception)
					{
						await DisplayAlert("Attenzione", "Destinazione non trovata", "OK");
					}
				}
				_dsrList = await _dbcon.QueryAsync<ScaPagRow>("SELECT * FROM scapagro WHERE dsr_relaz = ? AND dsr_codice = ?", _dsp.dsp_relaz, _dsp.dsp_codice);
			}
		}

		public void SetField()
		{
			m_codice.Value = _dsp.dsp_codice;

			//m_data.Date = _dsp.dsp_data.Value;

			m_data.Text = _dsp.dsp_data.Value.ToString("dd/mm/yyy");

			if (_cli != null)
			{
				m_cli_cod.Value = _cli.cli_codice;
				m_cli_des.Text = _cli.cli_desc;
			}
			else
			{
				m_cli_cod.Value = 0;
				m_cli_des.Text = "";
			}
			if (_dst != null)
			{
				m_dst_cod.Value = _dst.dst_codice;
				m_dst_des.Text = _dst.dst_desc;
			}
			else
			{
				m_dst_cod.Value = 0;
				m_dst_des.Text = "";
			}

			m_abbuoni.Value = _dsp.dsp_abbuoni;
			m_totale.Value = _dsp.dsp_totale;

			if (_dsrList != null)
				dataGrid.ItemsSource = new ObservableCollection<ScaPagRow>(_dsrList);
			else
				dataGrid.ItemsSource = null;

		}

		public void GetField()
		{

		}

		async void OnClickPrec(object sender, System.EventArgs e)
		{
			busyIndicator.IsBusy = true;
			try
			{
				var dspList = await _dbcon.QueryAsync<ScaPagHead>("SELECT * FROM scapaghe WHERE  dsp_codice < ? ORDER BY dsp_codice DESC LIMIT 1", _dsp.dsp_codice);
				if (dspList.Count > 0)
				{
					foreach (var dsp in dspList)
					{
						_dsp = dsp;
						await LoadRelAsync();
						SetField();
						break;
					}
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				await Navigation.PopAsync();
				return;
			}
			finally
			{
				busyIndicator.IsBusy = false;
			}
		}

		async void OnClickSucc(object sender, System.EventArgs e)
		{
			busyIndicator.IsBusy = true;
			try
			{
				var dspList = await _dbcon.QueryAsync<ScaPagHead>("SELECT * FROM scapaghe WHERE dsp_codice > ? ORDER BY dsp_codice LIMIT 1", _dsp.dsp_codice);
				if (dspList.Count > 0)
				{
					foreach (var dsp in dspList)
					{
						_dsp = dsp;
						await LoadRelAsync();
						SetField();
						break;
					}
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				await Navigation.PopAsync();
				return;
			}
			finally
			{
				busyIndicator.IsBusy = false;
			}
		}

		async void OnClickElimina(object sender, System.EventArgs e)
		{
			var test = await DisplayAlert("Attenzione!", "Confermi la cancellazione del documento?", "Si", "No");
			if (!test) return;

			//
			// Cancelliamo le righe
			//
			try
			{
				await _dbcon.ExecuteAsync("DELETE FROM scapagro WHERE dsr_relaz = ? AND dsr_codice = ?", _dsp.dsp_relaz, _dsp.dsp_codice);
				await _dbcon.ExecuteAsync("DELETE FROM scapaghe WHERE dsp_relaz = ? AND dsp_codice = ?", _dsp.dsp_relaz, _dsp.dsp_codice);
			}
			catch (Exception ex)
			{
				await DisplayAlert("Errore!", "Impossibile cancellare : " + ex.Message, "OK");
				return;
			}

			busyIndicator.IsBusy = true;
			try
			{
				var dspList = await _dbcon.QueryAsync<ScaPagHead>("SELECT * FROM scapaghe WHERE dsp_codice > ? ORDER BY dsp_codice LIMIT 1", _dsp.dsp_codice);
				if (dspList.Count > 0)
				{
					foreach (var dsp in dspList)
					{
						_dsp = dsp;
						await LoadRelAsync();
						SetField();
						break;
					}
				}
				else
				{
					dspList = await _dbcon.QueryAsync<ScaPagHead>("SELECT * FROM scapaghe WHERE dsp_codice < ? ORDER BY dsp_codice DESC LIMIT 1", _dsp.dsp_codice);
					if (dspList.Count > 0)
					{
						foreach (var dsp in dspList)
						{
							_dsp = dsp;
							await LoadRelAsync();
							SetField();
							break;
						}
					}
					else await Navigation.PopAsync();
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				await Navigation.PopAsync();
				return;
			}
			finally
			{
				busyIndicator.IsBusy = false;
			}
		}

	}
}

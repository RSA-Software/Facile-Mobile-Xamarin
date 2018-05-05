using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Facile.Extension;
using Facile.Interfaces;
using Facile.Models;
using Facile.Utils;
//using LinkOS.Plugin;
//using LinkOS.Plugin.Abstractions;
using SQLite;
using Xamarin.Forms;

namespace Facile
{
	public enum ConnectionType
	{
		Bluetooth,
		USB,
		Network
	}

	public partial class DocumentiHeader : ContentPage
	{
		private DocumentiEdit _parent;
		private bool _first;
		private readonly SQLiteAsyncConnection _dbcon;
		private Clienti _cli;
		private Destinazioni _dst;

		public DocumentiHeader(DocumentiEdit par)
		{
			_parent = par;
			_first = true;
			_dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();

			InitializeComponent();
			SetProtection();
		}
		protected override async void OnAppearing()
		{
			if (_first)
			{
				_first = false;
				await LoadRel();
				SetField();
			}
			base.OnAppearing();
		}

		protected void SetProtection()
		{
			if (_parent.nuova)
			{
				m_prec.IsEnabled = false;
				m_succ.IsEnabled = false;
				m_salva.IsEnabled = true;
				m_elimina.IsEnabled = false;
				m_stampa.IsEnabled = false;
				m_email.IsEnabled = false;

				m_prec.IsVisible = false;
				m_succ.IsVisible = false;
				m_salva.IsVisible = true;
				m_elimina.IsVisible = false;
				m_stampa.IsVisible = false;
				m_email.IsVisible = false;

				m_cli_cod.IsEnabled = true;
				m_search_cli.IsEnabled = true;  
				m_dst_cod.IsEnabled = true;
				m_search_dst.IsEnabled = true;

				m_n_doc.IsEnabled = true;
				m_d_doc.IsEnabled = true;
			}
			else
			{
				m_n_doc.IsEnabled = false;

				if (_parent.doc.fat_editable)
				{
					m_prec.IsVisible = true;
					m_succ.IsVisible = true;
					m_salva.IsVisible = true;
					m_elimina.IsVisible = true;
					m_stampa.IsVisible = true;
					m_email.IsVisible = true;

					m_prec.IsEnabled = true;
					m_succ.IsEnabled = true;
					m_salva.IsEnabled = true;
					m_elimina.IsEnabled = true;
					m_stampa.IsEnabled = true;
					m_email.IsEnabled = true;

					m_cli_cod.IsEnabled = true;
					m_search_cli.IsEnabled = true;
					m_dst_cod.IsEnabled = true;
					m_search_dst.IsEnabled = true;
					m_d_doc.IsEnabled = true;
				}
				else
				{
					m_prec.IsVisible = true;
					m_succ.IsVisible = true;
					m_salva.IsVisible = false;
					m_elimina.IsVisible = false;
					m_stampa.IsVisible = true;
					m_email.IsVisible = true;

					m_prec.IsEnabled = true;
					m_succ.IsEnabled = true;
					m_salva.IsEnabled = false;
					m_elimina.IsEnabled = false;
					m_stampa.IsEnabled = true;
					m_email.IsEnabled = true;

					m_cli_cod.IsEnabled = false;
					m_search_cli.IsEnabled = false;
					m_dst_cod.IsEnabled = false;
					m_search_dst.IsEnabled = false;
					m_d_doc.IsEnabled = false;
				}
			}
		}


		protected async Task LoadRel()
		{
			_cli = null;
			_dst = null;
			if (_parent.doc.fat_inte != 0)
			{
				try
				{
					_cli = await _dbcon.GetAsync<Clienti>(_parent.doc.fat_inte);
				}
				catch (System.Exception)
				{
					await DisplayAlert("Attenzione", "Cliente non trovato", "OK");
				}
				if (_parent.doc.fat_dest != 0)
				{
					try
					{
						_dst = await _dbcon.GetAsync<Destinazioni>(_parent.doc.fat_dest);
					}
					catch (System.Exception)
					{
						await DisplayAlert("Attenzione", "Destinazione non trovata", "OK");
					}
				}
			}
		}

		public void SetField()
		{
			m_cli_cod.Text = "0";
			m_dst_cod.Text = "0";
			if (_cli != null)
			{
				m_cli_cod.Text = _cli.cli_codice.ToString();
				cli_desc.Text = _cli.cli_desc;
				cli_indirizzo.Text = _cli.cli_indirizzo;
				cli_citta.Text = _cli.cli_citta;
			}
			else
			{
				m_cli_cod.Text = "0";
				cli_desc.Text = "";
				cli_indirizzo.Text = "";
				cli_citta.Text = "";
			}
			if (_dst != null)
			{
				m_dst_cod.Text = _dst.dst_codice.ToString();
				dst_desc.Text = _dst.dst_desc;
				dst_indirizzo.Text = _dst.dst_indirizzo;
				dst_citta.Text = _dst.dst_citta;
			}
			else
			{
				m_dst_cod.Text = "0";
				dst_desc.Text = "";
				dst_indirizzo.Text = "";
				dst_citta.Text = "";
			}

			m_n_doc.Value = RsaUtils.GetShowedNumDoc(_parent.doc.fat_n_doc);
			m_d_doc.Date = _parent.doc.fat_d_doc;
			fat_registro.Text = _parent.doc.fat_registro;

		}

		public void GetField()
		{
			_parent.doc.fat_inte = Int32.Parse(m_cli_cod.Text);
			_parent.doc.fat_dest = Int32.Parse(m_dst_cod.Text);
			_parent.doc.fat_n_doc = RsaUtils.GetStoredNumDoc((int)m_n_doc.Value,fat_registro.Text); 
			_parent.doc.fat_registro = fat_registro.Text;
			_parent.doc.fat_d_doc = m_d_doc.Date;
		}

		void OnClienteTapped(object sender, System.EventArgs e)
		{
			var page = new ClientiSearch();
			page.CliList.ItemDoubleTapped += (source, args) =>
			{
				_cli = (Clienti)args.ItemData;
				if (_parent.doc.fat_tipo_ven == (short)DocTipoVen.VEN_TRASFERT)
					_parent.doc.fat_listino = _cli.cli_listino_tra;
				else
					_parent.doc.fat_listino = _cli.cli_listino;
				_parent.doc.fat_pag = _cli.cli_pag;
				if (_dst != null) 
				{
					if (_dst.dst_cli_for != _cli.cli_codice)
					{
						_dst = null;
						_parent.doc.fat_dest = 0;
					}
				}
				SetField();
				Navigation.PopAsync();
			};
			Navigation.PushAsync(page);
		}

		void OnDestinazioneTapped(object sender, System.EventArgs e)
		{
			var page = new DestinazioniSearch(_cli != null ? _cli.cli_codice : 0);
			page.DstList.ItemDoubleTapped += (source, args) =>
			{
				_dst = (Destinazioni)args.ItemData;
				SetField();
				Navigation.PopAsync();
			};
			Navigation.PushAsync(page);
		}

		async void OnCliCodUnfocused(object sender, Xamarin.Forms.FocusEventArgs e)
		{
			GetField();
			_cli = null;
			_dst = null;
			if (_parent.doc.fat_inte != 0)
			{
				try
				{
					_cli = await _dbcon.GetAsync<Clienti>(_parent.doc.fat_inte);
					if (_parent.doc.fat_tipo_ven == (short)DocTipoVen.VEN_TRASFERT)
						_parent.doc.fat_listino = _cli.cli_listino_tra;
					else
						_parent.doc.fat_listino = _cli.cli_listino;
					_parent.doc.fat_pag = _cli.cli_pag;
				}
				catch (System.Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);
				}
				if (_parent.doc.fat_dest != 0)
				{
					try
					{
						_dst = await _dbcon.GetAsync<Destinazioni>(_parent.doc.fat_dest);
						if (_dst.dst_cli_for != _cli.cli_codice || _dst.dst_rel != 0) _dst = null;
					}
					catch (System.Exception ex)
					{
						System.Diagnostics.Debug.WriteLine(ex.Message);
					}
				}
			}
			if (_cli == null) _parent.doc.fat_inte = 0;
			if (_dst == null) _parent.doc.fat_dest = 0;
			SetField();
		}

		async void OnDstCodUnfocused(object sender, Xamarin.Forms.FocusEventArgs e)
		{
			GetField();
			_dst = null;
			if (_parent.doc.fat_inte != 0)
			{
				if (_parent.doc.fat_dest != 0)
				{
					try
					{
						_dst = await _dbcon.GetAsync<Destinazioni>(_parent.doc.fat_dest);
						if (_dst.dst_cli_for != _cli.cli_codice || _dst.dst_rel != 0) _dst = null;
					}
					catch (System.Exception ex)
					{
						System.Diagnostics.Debug.WriteLine(ex.Message);
					}
				}
			}
			if (_dst == null) _parent.doc.fat_dest = 0;
			SetField();
		}

		async void OnClickPrec(object sender, System.EventArgs e)
		{
			if (_parent.nuova) return;

			busyIndicator.IsBusy = true;
			var sql = string.Format("SELECT * FROM fatture2 WHERE fat_tipo = {0} AND fat_n_doc < {1} ORDER BY  fat_tipo, fat_n_doc  DESC LIMIT 1", _parent.doc.fat_tipo, _parent.doc.fat_n_doc); 

			try
			{
				var docList = await _dbcon.QueryAsync<Fatture>(sql);

				if (docList.Count > 0)
				{
					foreach (var doc in docList)
					{
						_parent.doc = doc;
						SetProtection();
						await LoadRel();
						SetField();
						break;
					}
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				return;
			}
			finally
			{
				busyIndicator.IsBusy = false;
			}
		}

		async void OnClickSucc(object sender, System.EventArgs e)
		{
			if (_parent.nuova) return;

			busyIndicator.IsBusy = true;
			var sql = string.Format("SELECT * FROM fatture2 WHERE fat_tipo = {0} AND fat_n_doc > {1} ORDER BY fat_tipo, fat_n_doc LIMIT 1", _parent.doc.fat_tipo, _parent.doc.fat_n_doc);
			try
			{
				var docList = await _dbcon.QueryAsync<Fatture>(sql);
				if (docList.Count > 0)
				{
					foreach (var doc in docList)
					{
						_parent.doc = doc;
						SetProtection();
						await LoadRel();
						SetField();
						break;
					}
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				return;
			}
			finally
			{
				busyIndicator.IsBusy = false; 
			}
		}

		async void OnRecordSalva(object sender, System.EventArgs e)
		{
			if (!m_salva.IsEnabled) return;
			GetField();
			if (_parent.doc.fat_inte == 0L)
			{
				m_cli_cod.Focus();
				return;
			}

			try
			{
				await _parent.doc.RecalcAsync();
			}
			catch (Exception ex)
			{
				await DisplayAlert("Errore", ex.Message, "OK");
				return;
			}

			//
			// Controlliamo che l'anno sia coincidente con le impostazioni
			//
			//if (_parent.doc.fat_d_doc.Year != ) 

			//
			// Inseriamo l'agente 
			//

			if (_parent.nuova)
			{
				do
				{
					try
					{
						await _dbcon.InsertAsync(_parent.doc);
						_parent.nuova = false;
						SetProtection();
						SetField();
						return;
					}
					catch (SQLiteException ex)
					{
						if (string.Compare(ex.Message.ToUpper(),"CONSTRAINT") == 0)
						{
							_parent.doc.fat_n_doc++;
							continue;
						}
						else
						{
							await DisplayAlert("Attenzione!", ex.Message, "OK");
							return;
						}
					}
				} while (true);
			}
			else
			{
				try
				{
					await _dbcon.UpdateAsync(_parent.doc);
					return;
				}
				catch (SQLiteException ex)
				{
					await DisplayAlert("Attenzione!", ex.Message, "OK");
					return;
				}
			}
		}

		async void OnRecordStampa(object sender, System.EventArgs e)
		{
			GetField();
			if (_parent.doc.fat_inte == 0L)
			{
				m_cli_cod.Focus();
				return;
			}

			try
			{
				await _parent.doc.RecalcAsync();
			}
			catch (Exception ex)
			{
				await DisplayAlert("Errore", ex.Message, "OK");
				return;
			}

			try
			{
				await _dbcon.UpdateAsync(_parent.doc);
			}
			catch (SQLiteException ex)
			{
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				return;
			}

			var animation = busyIndicator.AnimationType;
			busyIndicator.IsBusy = true;
			busyIndicator.AnimationType = Syncfusion.SfBusyIndicator.XForms.AnimationTypes.Print;
			var prn = new ZebraPrn(this);
			await prn.PrintDoc(_parent.doc);
			busyIndicator.IsBusy = false;
			busyIndicator.AnimationType = animation;
		}
	}
}

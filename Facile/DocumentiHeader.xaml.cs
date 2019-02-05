using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

			if (Device.Idiom == TargetIdiom.Tablet)
			{
				m_cli_cod.WidthRequest = 150;
				m_dst_cod.WidthRequest = 150;
				m_n_doc.WidthRequest = 150;
				m_d_doc.WidthRequest = 150;
			}


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
					m_elimina.IsVisible = true;
					m_stampa.IsVisible = true;
					m_email.IsVisible = true;

					m_prec.IsEnabled = true;
					m_succ.IsEnabled = true;
					m_salva.IsEnabled = false;
					m_elimina.IsEnabled = true;
					m_stampa.IsEnabled = true;
					m_email.IsEnabled = true;

					m_cli_cod.IsEnabled = false;
					m_search_cli.IsEnabled = false;
					m_dst_cod.IsEnabled = false;
					m_search_dst.IsEnabled = false;
					m_d_doc.IsEnabled = false;
				}
			}


			m_email.IsEnabled = false;
			m_email.IsVisible = false;
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

			try
			{
				int num = m_n_doc.Value == null ? 0 : Convert.ToInt32(m_n_doc.Value);
				_parent.doc.fat_n_doc = RsaUtils.GetStoredNumDoc(num, fat_registro.Text);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}

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
				if (_parent.doc.fat_tipo != (short)DocTipo.TIPO_FAT && _parent.doc.fat_tipo != (short)DocTipo.TIPO_ORD && _parent.doc.fat_tipo != (short)DocTipo.TIPO_PRE && _parent.doc.fat_tipo != (short)DocTipo.TIPO_BUO)
					_parent.doc.fat_rag = _cli.cli_ragg;
				_parent.doc.fat_spese = _cli.cli_spese;
				_parent.doc.fat_bolli = _cli.cli_bolli;
				_parent.doc.fat_iva_cli = _cli.cli_iva;
				_parent.doc.fat_ban = _cli.cli_ban;
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

		async void OnDestinazioneTapped(object sender, System.EventArgs e)
		{
			//
			// Verifichiamo che ci destinazioni per il cliente
			//
			var recTotal_ = 0;
			if (_cli != null)
			{
				string sql = "SELECT COUNT(*) FROM destina1 WHERE dst_rel = 0 AND dst_cli_for = " + _cli.cli_codice;
				recTotal_ = await _dbcon.ExecuteScalarAsync<int>(sql);
			}
			if (recTotal_ == 0)
			{
				await DisplayAlert("Attenzione!", "Non ci sono destinazioni per il Cliente selezionato", "OK");
				return;
			}
			var page = new DestinazioniSearch(_cli != null ? _cli.cli_codice : 0);
			page.DstList.ItemDoubleTapped += (source, args) =>
			{
				_dst = (Destinazioni)args.ItemData;
				SetField();
				Navigation.PopAsync();
			};
			await Navigation.PushAsync(page);
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
					if (_parent.doc.fat_tipo != (short)DocTipo.TIPO_FAT && _parent.doc.fat_tipo != (short)DocTipo.TIPO_ORD && _parent.doc.fat_tipo != (short)DocTipo.TIPO_PRE && _parent.doc.fat_tipo != (short)DocTipo.TIPO_BUO) 
						_parent.doc.fat_rag = _cli.cli_ragg;
					_parent.doc.fat_spese = _cli.cli_spese;
					_parent.doc.fat_bolli = _cli.cli_bolli;
					_parent.doc.fat_iva_cli = _cli.cli_iva;
					_parent.doc.fat_ban = _cli.cli_ban;
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
				await Navigation.PopAsync();
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
			var anno = ((App)Application.Current).facile_db_impo.dit_anno;
			if (_parent.doc.fat_d_doc.Year != anno)
			{
				m_d_doc.Focus();
				await DisplayAlert("Attenzione!", $"Anno documento diverso anno di lavoro ({anno})\n\nImpossibile continuare", "OK");
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
						if (string.Compare(ex.Message.ToUpper(), "CONSTRAINT") == 0)
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
			bool stprice = true;
			LocalImpo imp = null;
			short num_copy = 1;
			short max_copy = 1;
			bool req_copy = false;

			m_stampa.IsEnabled = false;
			GetField();
			var animation = busyIndicator.AnimationType;
			busyIndicator.IsBusy = true;
			busyIndicator.AnimationType = Syncfusion.SfBusyIndicator.XForms.AnimationTypes.Print;

			if (_parent.doc.fat_inte == 0L)
			{
				busyIndicator.IsBusy = true;
				m_cli_cod.Focus();
				m_stampa.IsEnabled = true;
				return;
			}

			//
			// Ricalcoliamo il documento
			//
			try
			{
				await _parent.doc.RecalcAsync();
				await _dbcon.UpdateAsync(_parent.doc);
			}
			catch (SQLiteException ex)
			{
				busyIndicator.IsBusy = true;
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				m_stampa.IsEnabled = true;
				return;
			}
			catch (Exception ex)
			{
				busyIndicator.IsBusy = true;
				await DisplayAlert("Errore", ex.Message, "OK");
				m_stampa.IsEnabled = true;
				return;
			}

			//
			// Leggiamo le impostazioni della stampante
			//
			try
			{
				imp = await _dbcon.GetAsync<LocalImpo>(1);
				 
				switch(imp.seconda_copia)
				{
					case 0 :  // A Rchiesta
						max_copy = 2;
						num_copy = 1;
						req_copy = true;
						break;

					case 1 :  // Stampa automatica
						max_copy = 1;
						num_copy = 2;
						req_copy = false;
						break;

					default : // Non Stampare la seconda copia
						max_copy = 1;
						num_copy = 1;
						req_copy = false;
						break;
				}
			}
			catch
			{
				await DisplayAlert("Attenzione!", "Impossibile leggere le impostazioni della stampante", "OK");
				busyIndicator.IsBusy = true;
				m_stampa.IsEnabled = true;
				return;
			}
			if (string.IsNullOrWhiteSpace(imp.printer))
			{
				await DisplayAlert("Attenzione!", "Nessuna stampante configurata.", "OK");
				busyIndicator.IsBusy = true;
				m_stampa.IsEnabled = true;
				return;
			}

			if (_parent.doc.fat_tipo == (short)DocTipo.TIPO_DDT)
			{
				stprice = await DisplayAlert("Facile", "Vuoi Stampare i Prezzi ?", "SI", "NO");
				await Task.Delay(100);
			}

			try
			{
				if (Device.RuntimePlatform == Device.iOS)
				{
					for (var idx = 0; idx < max_copy; idx++)
					{
						if (idx != 0 && req_copy)
						{
							var test = await DisplayAlert("Attenzione...", "Vuoi stampare un'altra copia?", "Si", "No");
							if (test == false) break;
							await Task.Delay(100);
						}
						var t = Task.Run(async () =>
						{
							var prn = new ZebraPrn(imp.printer);
							await prn.PrintDoc(_parent.doc, stprice, num_copy);
						});
						t.Wait();
					}
				}
				else if (Device.RuntimePlatform == Device.Android)
				{
					var prn = new ZebraPrn(imp.printer);
					for (var idx = 0; idx < max_copy; idx++)
					{
						if (idx != 0 && req_copy)
						{
							var test = await DisplayAlert("Attenzione...", "Vuoi stampare un'altra copia?", "Si", "No");
							if (test == false) break;
							await Task.Delay(100);
						}
						await prn.PrintDoc(_parent.doc, stprice, num_copy);
					}
				}
			}
			catch (ZebraExceptions ex)
			{
				await DisplayAlert("Errore", ex.Message, "ok");
			}
			catch (Exception ex)
			{
				await DisplayAlert("Errore", ex.Message, "ok");
			}

			busyIndicator.IsBusy = false;
			busyIndicator.AnimationType = animation;
			m_stampa.IsEnabled = true;
		}

		async void OnRecordElimina(object sender, System.EventArgs e)
		{
			var test = await DisplayAlert("Attenzione!", "Confermi la cancellazione del documento?", "Si", "No");
			if (!test) return;

			//
			// Cancelliamo le righe
			//
			try
			{
				await _dbcon.ExecuteAsync("DELETE FROM fatrow2 WHERE rig_tipo = ? AND rig_n_doc = ?", _parent.doc.fat_tipo, _parent.doc.fat_n_doc);
				await _dbcon.ExecuteAsync("DELETE FROM fatture2 WHERE fat_tipo = ? AND fat_n_doc = ?", _parent.doc.fat_tipo, _parent.doc.fat_n_doc);
			}
			catch (Exception ex)
			{
				await DisplayAlert("Errore!", "Impossibile cancellare : " + ex.Message, "OK");
				return;
			}

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
				else
				{
					sql = string.Format("SELECT * FROM fatture2 WHERE fat_tipo = {0} AND fat_n_doc < {1} ORDER BY fat_tipo, fat_n_doc DESC LIMIT 1", _parent.doc.fat_tipo, _parent.doc.fat_n_doc);
					docList = await _dbcon.QueryAsync<Fatture>(sql);
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

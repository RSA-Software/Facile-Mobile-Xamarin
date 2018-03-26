using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Xamarin.Forms;

namespace Facile
{
	public partial class DocumentiHeader : ContentPage
	{
		private DocumentiEdit _parent;
		private bool first;
		private readonly SQLiteAsyncConnection dbcon_;
		private Clienti cli_ = null;
		private Destinazioni dst_ = null;

		public DocumentiHeader(DocumentiEdit par)
		{
			_parent = par;
			first = true;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			InitializeComponent();
			SetProtection();
		}
		protected override async void OnAppearing()
		{
			if (first)
			{
				first = false;
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
			if (_parent.doc.fat_inte != 0)
			{
				try
				{
					cli_ = await dbcon_.GetAsync<Clienti>(_parent.doc.fat_inte);
				}
				catch (System.Exception)
				{
					await DisplayAlert("Attenzione", "Cliente non trovato", "OK");
				}
				if (_parent.doc.fat_dest != 0)
				{
					try
					{
						dst_ = await dbcon_.GetAsync<Destinazioni>(_parent.doc.fat_dest);
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
			if (cli_ != null)
			{
				m_cli_cod.Text = cli_.cli_codice.ToString();
				cli_desc.Text = cli_.cli_desc;
				cli_indirizzo.Text = cli_.cli_indirizzo;
				cli_citta.Text = cli_.cli_citta;
			}
			if (dst_ != null)
			{
				m_dst_cod.Text = dst_.dst_codice.ToString();
				dst_desc.Text = dst_.dst_desc;	
				dst_desc.Text = dst_.dst_desc;
				dst_indirizzo.Text = dst_.dst_indirizzo;
				dst_citta.Text = dst_.dst_citta;
			}
			m_n_doc.Value = _parent.doc.fat_n_doc % 700000000;
			m_d_doc.Date = _parent.doc.fat_d_doc;
			fat_registro.Text = _parent.doc.fat_registro;

		}

		public void GetField()
		{
			_parent.doc.fat_inte = Int32.Parse(m_cli_cod.Text);
			_parent.doc.fat_dest = Int32.Parse(m_dst_cod.Text);
			//doc_.fat_n_doc = fat_n_doc.Value + () % 700000000 +;
			_parent.doc.fat_registro = fat_registro.Text;
			_parent.doc.fat_d_doc = m_d_doc.Date;
		}

		void OnClienteTapped(object sender, System.EventArgs e)
		{
			var page = new ClientiSearch();
			page.CliList.ItemDoubleTapped += (source, args) =>
			{
				cli_ = (Clienti)args.ItemData;
				SetField();
				Navigation.PopAsync();
			};
			Navigation.PushAsync(page);
		}

		void OnDestinazioneTapped(object sender, System.EventArgs e)
		{
			var page = new DestinazioniSearch(cli_ != null ? cli_.cli_codice : 0);
			page.DstList.ItemDoubleTapped += (source, args) =>
			{
				dst_ = (Destinazioni)args.ItemData;
				SetField();
				Navigation.PopAsync();
			};
			Navigation.PushAsync(page);
		}

		async void OnCliCodUnfocused(object sender, Xamarin.Forms.FocusEventArgs e)
		{
			GetField();
			cli_ = null;
			dst_ = null;
			if (_parent.doc.fat_inte != 0)
			{
				try
				{
					cli_ = await dbcon_.GetAsync<Clienti>(_parent.doc.fat_inte);
				}
				catch (System.Exception)
				{
					
				}
				if (_parent.doc.fat_dest != 0)
				{
					try
					{
						dst_ = await dbcon_.GetAsync<Destinazioni>(_parent.doc.fat_dest);
						if (dst_.dst_cli_for != cli_.cli_codice || dst_.dst_rel != 0) dst_ = null;
					}
					catch (System.Exception)
					{
						//
					}
				}
			}
			if (cli_ == null) _parent.doc.fat_inte = 0;
			if (dst_ == null) _parent.doc.fat_inte = 0;
			SetField();
		}

		void OnDstCodUnfocused(object sender, Xamarin.Forms.FocusEventArgs e)
		{
			DisplayAlert("Attenzione!", "Da Implementare", "OK");
		}

		async void OnClickPrec(object sender, System.EventArgs e)
		{
			if (_parent.nuova) return;

			var sql = string.Format("SELECT * FROM fatture2 WHERE fat_tipo = {0} AND fat_n_doc < {1} ORDER BY  fat_tipo, fat_n_doc  DESC LIMIT 1", _parent.doc.fat_tipo, _parent.doc.fat_n_doc); 

			try
			{
				var docList = await dbcon_.QueryAsync<Fatture>(sql);

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
		}

		async void OnClickSucc(object sender, System.EventArgs e)
		{
			if (_parent.nuova) return;

			var sql = string.Format("SELECT * FROM fatture2 WHERE fat_tipo = {0} AND fat_n_doc > {1} ORDER BY fat_tipo, fat_n_doc LIMIT 1", _parent.doc.fat_tipo, _parent.doc.fat_n_doc);

			try
			{
				var docList = await dbcon_.QueryAsync<Fatture>(sql);
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
						await dbcon_.InsertAsync(_parent.doc);
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
					await dbcon_.UpdateAsync(_parent.doc);
					return;
				}
				catch (SQLiteException ex)
				{
					await DisplayAlert("Attenzione!", ex.Message, "OK");
					return;
				}
			}
		}
	}
}

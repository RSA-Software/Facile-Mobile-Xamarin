using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Facile.Interfaces;
using Facile.Models;
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

		//ObservableCollection<IDiscoveredPrinter> printerList;
		//ListView printerLv;
		//ConnectionType connetionType;

		public DocumentiHeader(DocumentiEdit par)
		{
			_parent = par;
			_first = true;
			_dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();

			//printerList = new ObservableCollection<IDiscoveredPrinter>();
			//printerLv = new ListView();

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
			if (_dst != null)
			{
				m_dst_cod.Text = _dst.dst_codice.ToString();
				dst_desc.Text = _dst.dst_desc;	
				dst_desc.Text = _dst.dst_desc;
				dst_indirizzo.Text = _dst.dst_indirizzo;
				dst_citta.Text = _dst.dst_citta;
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
				_cli = (Clienti)args.ItemData;
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
			if (_dst == null) _parent.doc.fat_inte = 0;
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
		}

		async void OnClickSucc(object sender, System.EventArgs e)
		{
			if (_parent.nuova) return;

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

		void Handle_Clicked(object sender, System.EventArgs e)
		{
			/*
			new Task(new Action(() => {
				StartUSBDiscovery();
			})).Start();
		*/		
		}
		/*
		private void StartUSBDiscovery()
		{
			OnStatusMessage("Discovering USB Printers");
			try
			{
				IDiscoveryEventHandler usbhandler = DiscoveryHandlerFactory.Current.GetInstance();
				usbhandler.OnDiscoveryError += DiscoveryHandler_OnDiscoveryError;
				usbhandler.OnDiscoveryFinished += DiscoveryHandler_OnDiscoveryFinished;
				usbhandler.OnFoundPrinter += DiscoveryHandler_OnFoundPrinter;
				connetionType = ConnectionType.USB;
				System.Diagnostics.Debug.WriteLine("Starting USB Discovery");
				DependencyService.Get<IPrinterDiscovery>().FindUSBPrinters(usbhandler);
			}
			catch (NotImplementedException)
			{
				//  USB not availible on iOS, so handle the exeption and move on to Bluetooth discovery
				StartBluetoothDiscovery();
			}
		}

		private void StartNetworkDiscovery()
		{
			OnStatusMessage("Discovering Network Printers");
			try
			{
				IDiscoveryEventHandler nwhandler = DiscoveryHandlerFactory.Current.GetInstance();
				nwhandler.OnDiscoveryError += DiscoveryHandler_OnDiscoveryError;
				nwhandler.OnDiscoveryFinished += DiscoveryHandler_OnDiscoveryFinished;
				nwhandler.OnFoundPrinter += DiscoveryHandler_OnFoundPrinter;
				connetionType = ConnectionType.Network;
				System.Diagnostics.Debug.WriteLine("Starting Network Discovery");
				NetworkDiscoverer.Current.LocalBroadcast(nwhandler);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("Network Exception: " + e.Message);
			}
		}

		private void StartBluetoothDiscovery()
		{
			OnStatusMessage("Discovering Bluetooth Printers");
			IDiscoveryEventHandler bthandler = DiscoveryHandlerFactory.Current.GetInstance();
			bthandler.OnDiscoveryError += DiscoveryHandler_OnDiscoveryError;
			bthandler.OnDiscoveryFinished += DiscoveryHandler_OnDiscoveryFinished;
			bthandler.OnFoundPrinter += DiscoveryHandler_OnFoundPrinter;
			connetionType = ConnectionType.Bluetooth;
			System.Diagnostics.Debug.WriteLine("Starting Bluetooth Discovery");
			DependencyService.Get<IPrinterDiscovery>().FindBluetoothPrinters(bthandler);
		}


		private void DiscoveryHandler_OnFoundPrinter(object sender, IDiscoveredPrinter discoveredPrinter)
		{

			System.Diagnostics.Debug.WriteLine("Found Printer:" + discoveredPrinter.ToString());
			Device.BeginInvokeOnMainThread(() => {
				printerLv.BatchBegin();

				if (!printerList.Contains(discoveredPrinter))
				{
					printerList.Add(discoveredPrinter);
				}
				printerLv.BatchCommit();
			});
		}

		private void DiscoveryHandler_OnDiscoveryFinished(object sender)
		{
			System.Diagnostics.Debug.WriteLine("On Discovery Finished:" + connetionType.ToString());

			if (connetionType == ConnectionType.USB)
			{
				StartBluetoothDiscovery();
			}
			else if (connetionType == ConnectionType.Bluetooth)
			{
				StartNetworkDiscovery();
			}
			else
				OnStatusMessage("Discovery Finished");
		}

		private void DiscoveryHandler_OnDiscoveryError(object sender, string message)
		{
			System.Diagnostics.Debug.WriteLine("On Discovery Error: " + connetionType.ToString());
			OnError(message);

			if (connetionType == ConnectionType.USB)
			{
				StartBluetoothDiscovery();
			}
			else if (connetionType == ConnectionType.Bluetooth)
			{
				StartNetworkDiscovery();
			}
			else
				OnStatusMessage("Discovery Finished");
		}

		private void OnError(string message)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				DisplayAlert("Error", message, "OK");
			});
		}

		private void OnStatusMessage(string message)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				System.Diagnostics.Debug.WriteLine("On Discovery : " + message);
				//statusLbl.Text = message;
			});
		}
		*/
	}
}

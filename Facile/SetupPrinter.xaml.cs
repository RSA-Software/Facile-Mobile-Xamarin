using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Facile.Interfaces;
using LinkOS.Plugin;
using LinkOS.Plugin.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SetupPrinter : ContentPage
	{
		ObservableCollection<IDiscoveredPrinter> printers;

		public SetupPrinter()
		{
			printers = new ObservableCollection<IDiscoveredPrinter>();
			InitializeComponent();
			elenco.FontSize += 4;

			var app = (App)Application.Current;
			if (app.printer != null) printers.Add(app.printer);

			lstDevices.ItemsSource = printers;
			if (printers.Count > 0)
			{
				lstDevices.SelectedItem = app.printer;
			}

			btnPrint.IsEnabled = false;
			btnScan.Clicked += (sender, e) =>
			{
				busyIndicator.IsBusy = true;
				btnScan.Text = "Ricerca in Corso";
				btnScan.IsEnabled = false;
				IsBusy = true;
				Task.Run(() =>
				{
					StartBluetoothDiscovery();
				});
			};
			btnPrint.Clicked += BtnPrint_Clicked; ;


		}

		void Handle_ItemTapped(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
		{
			DependencyService.Get<IPrinterDiscovery>().CancelDiscovery();
			var app = (App)Application.Current;
			app.printer = e.ItemData as IDiscoveredPrinter;
			btnPrint.IsEnabled = true;
		}

		async void BtnPrint_Clicked(object sender, System.EventArgs e)
		{
			busyIndicator.IsBusy = true;
			var prn = new ZebraPrn(this);
			await prn.PrintTest();
			busyIndicator.IsBusy = false;
		}

		//Start searching for printers
		private void StartBluetoothDiscovery()
		{
			Debug.WriteLine("Discovering Bluetooth Printers");
			IDiscoveryEventHandler bthandler = DiscoveryHandlerFactory.Current.GetInstance();
			bthandler.OnDiscoveryError += DiscoveryHandler_OnDiscoveryError;
			bthandler.OnDiscoveryFinished += DiscoveryHandler_OnDiscoveryFinished;
			bthandler.OnFoundPrinter += DiscoveryHandler_OnFoundPrinter;
			Debug.WriteLine("Starting Bluetooth Discovery");
			DependencyService.Get<IPrinterDiscovery>().FindBluetoothPrinters(bthandler);
		}

		private void StartUSBDiscovery()
		{
			try
			{
				IDiscoveryEventHandler usbhandler = DiscoveryHandlerFactory.Current.GetInstance();
				usbhandler.OnDiscoveryError += DiscoveryHandler_OnDiscoveryError;
				usbhandler.OnDiscoveryFinished += DiscoveryHandler_OnDiscoveryFinished;
				usbhandler.OnFoundPrinter += DiscoveryHandler_OnFoundPrinter;
				Debug.WriteLine("Starting USB Discovery");
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

			try
			{
				IDiscoveryEventHandler nwhandler = DiscoveryHandlerFactory.Current.GetInstance();
				nwhandler.OnDiscoveryError += DiscoveryHandler_OnDiscoveryError;
				nwhandler.OnDiscoveryFinished += DiscoveryHandler_OnDiscoveryFinished;
				nwhandler.OnFoundPrinter += DiscoveryHandler_OnFoundPrinter;
				Debug.WriteLine("Starting Network Discovery");
				NetworkDiscoverer.Current.LocalBroadcast(nwhandler);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("Network Exception: " + e.Message);
			}
		}

		private void DiscoveryHandler_OnFoundPrinter(object sender, IDiscoveredPrinter discoveredPrinter)
		{
			Debug.WriteLine("Found Printer:" + discoveredPrinter.ToString());
			Device.BeginInvokeOnMainThread(() =>
			{
				lstDevices.BatchBegin();
				if (!printers.Contains(discoveredPrinter))
				{
					printers.Add(discoveredPrinter);
				}
				lstDevices.BatchCommit();
			});
		}

		private void DiscoveryHandler_OnDiscoveryFinished(object sender)
		{
			Debug.WriteLine("Discovery Finished");
			Device.BeginInvokeOnMainThread(() =>
			{
				IsBusy = false;
				btnScan.Text = "Cerca Stampanti";
				btnScan.TextColor = Color.Black;
				btnScan.IsEnabled = true;
				busyIndicator.IsBusy = false;
			});
		}

		private void DiscoveryHandler_OnDiscoveryError(object sender, string message)
		{
			Debug.WriteLine("On Discovery Error");
			Debug.WriteLine(message);
			Device.BeginInvokeOnMainThread(() =>
			{

				btnScan.Text = "Cerca Stampanti";
				btnScan.TextColor = Color.Black;
				btnScan.IsEnabled = true;
				busyIndicator.IsBusy = false;
			});
		}

	}
}

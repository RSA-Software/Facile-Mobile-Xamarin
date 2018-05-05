using System;
using Facile.Interfaces;
using Facile.iOS.Interfaces;
using LinkOS.Plugin;
using LinkOS.Plugin.Abstractions;

[assembly: Xamarin.Forms.Dependency(typeof(PrinterDiscovery))]
namespace Facile.iOS.Interfaces
{
	public class PrinterDiscovery : IPrinterDiscovery
	{
		public PrinterDiscovery() { }

		public void CancelDiscovery()
		{
		}

		public void FindBluetoothPrinters(IDiscoveryHandler handler)
		{
			BluetoothDiscoverer.Current.FindPrinters(null, handler);
		}

		public void FindUSBPrinters(IDiscoveryHandler handler)
		{
			throw new NotImplementedException();
		}

		public void RequestUSBPermission(IDiscoveredPrinterUsb printer)
		{
			throw new NotImplementedException();
		}
	}
}

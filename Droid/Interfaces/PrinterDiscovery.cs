using System;
using Facile.Interfaces;
//using LinkOS.Plugin;
//using LinkOS.Plugin.Abstractions;
using Android;
using Android.Bluetooth;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.Design.Widget;
using Facile.Droid.Interfaces;

//[assembly: Xamarin.Forms.Dependency(typeof(PrinterDiscovery))] 
namespace Facile.Droid.Interfaces
{
	/*
	public class PrinterDiscovery : IPrinterDiscovery
	{
		public PrinterDiscovery() { }

		public void CancelDiscovery()
		{
			if (BluetoothAdapter.DefaultAdapter.IsDiscovering)
			{
				BluetoothAdapter.DefaultAdapter.CancelDiscovery();
				System.Diagnostics.Debug.WriteLine("Cancelling Discovery");
			}
		}

		public void FindBluetoothPrinters(IDiscoveryHandler handler)
		{
			const string permission = Manifest.Permission.AccessCoarseLocation;
			if (ContextCompat.CheckSelfPermission(Android.App.Application.Context , permission) == (int)Permission.Granted)
			{
				BluetoothDiscoverer.Current.FindPrinters(Android.App.Application.Context , handler: handler);
				return;
			}
			TempHandler = handler;
			//Finally request permissions with the list of permissions and Id
			//ActivityCompat.RequestPermissions(, PermissionsLocation, RequestLocationId);
		}
		public static IDiscoveryHandler TempHandler { get; set; }

		public readonly string[] PermissionsLocation =
		{
		  Manifest.Permission.AccessCoarseLocation
		};
		public const int RequestLocationId = 0;



		public void FindUSBPrinters(IDiscoveryHandler handler)
		{
			Android.Content.Context context = Android.App.Application.Context; 	//Xamarin.Forms.Forms.Context;
			UsbDiscoverer.Current.FindPrinters(context, handler);
		}

		public void RequestUSBPermission(IDiscoveredPrinterUsb printer)
		{
			if (!printer.HasPermissionToCommunicate)
			{
				Android.Content.Context context = Android.App.Application.Context; //Xamarin.Forms.Forms.Context;
				printer.RequestPermission(context);
			}
		}
	}
*/
}

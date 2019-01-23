using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Facile.Models;
using LinkOS.Plugin;
using LinkOS.Plugin.Abstractions;
using Newtonsoft.Json;
using PCLStorage;
using Xamarin.Forms;

namespace Facile
{
    public partial class App : Application
    {
		public IDiscoveredPrinter printer;
		public Ditte facile_db_impo;

        public App()
        {
			printer = null;
			facile_db_impo = null;

			// 16.2.x.x
			//Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzUyQDMxMzYyZTMyMmUzMEZpOGZMSHMvU3hFMU5Dazc2QVNJYjlPdWhMWlZOSHhFRjFLZ1RNbE55RTg9");

			// 16.3.x.x
			//Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mjg3MTVAMzEzNjJlMzMyZTMwWjJMQ1RRTlFXNkdnaWpzS0l6MGcyTFlWQzhJSnZvK01mNyt0Y2hiTzZzVT0=");

			// 16.4.x.x.
			Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjI2MTVAMzEzNjJlMzQyZTMwYVBMVTR1U08rNUQrTXl4L0JDeTIxRkVIenQ4S3ZMdWVTK1Jrc0pDQ2xPOD0=");

			InitializeComponent();
			MainPage = new NavigationPage(new FacilePage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

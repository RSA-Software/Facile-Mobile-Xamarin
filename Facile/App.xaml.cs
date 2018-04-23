using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using LinkOS.Plugin;
using LinkOS.Plugin.Abstractions;
using Newtonsoft.Json;
using PCLStorage;
using Xamarin.Forms;

namespace Facile
{
    public partial class App : Application
    {
        private const string FtpServerKey = "FtpServer";
        private const string FtpUserKey = "FtpUser";
        private const string FtpPasswordKey = "FtpPassword";
        private const string FtpSslKey = "FtpSsl";
		private const string PrinterKey  = "Printer";

		public IDiscoveredPrinter printer;

        public App()
        {
			printer = null;
			InitializeComponent();
			MainPage = new NavigationPage(new FacilePage());
        }

        protected override void OnStart()
        {
			
			//IFolder rootFolder = FileSystem.Current.LocalStorage;
			//string path = rootFolder.Path + "/" + "PRINTER.JSON";

			//IFile file = await rootFolder.CreateFileAsync(path, CreationCollisionOption.OpenIfExists);
			//if (file != null)
			//{
			//	var settings = new JsonSerializerSettings();
			//	settings.DateFormatString = "dd/MM/yyyy HH:mm:ss";
			//	settings.NullValueHandling = NullValueHandling.Ignore;

			//	string str = await file.ReadAllTextAsync();
			//	printer = JsonConvert.DeserializeObject<IDiscoveredPrinter>(str, settings);

			//	int x = 1;
			//}
        }

        protected async override void OnSleep()
        {
			//if (printer != null)
			//{
			//	IFolder rootFolder = FileSystem.Current.LocalStorage;
			//	string path = rootFolder.Path + "/" + "PRINTER.JSON";

			//	var settings = new JsonSerializerSettings();
			//	settings.DateFormatString = "dd/MM/yyyy HH:mm:ss";
			//	settings.NullValueHandling = NullValueHandling.Ignore;


			//	string output = JsonConvert.SerializeObject(printer, settings);
			//	IFile json_file = await rootFolder.CreateFileAsync(path,CreationCollisionOption.ReplaceExisting);
			//	await json_file.WriteAllTextAsync(output); 
			//}

			await SavePropertiesAsync();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

		/*
		public IDiscoveredPrinter Printer
		{
			get
			{
				if (Properties.ContainsKey(PrinterKey))
					return ((IDiscoveredPrinter)Properties[PrinterKey]);
				return (null);
			}

			set
			{
				if (Properties.ContainsKey(PrinterKey))
					Properties[PrinterKey] = value;
				else
					Properties.Add(PrinterKey, value);
			}

		}
*/

        public string FtpServer 
        {
            get 
            {
                if (Properties.ContainsKey(FtpServerKey))
                    return (Properties[FtpServerKey].ToString());
                return ("");
            }

            set
            {
				if (Properties.ContainsKey(FtpServerKey))
					Properties[FtpServerKey] = value;
				else
	                Properties[FtpServerKey] = value;
            }
        }

        public string FtpUser
        {
            get
            {
                if (Properties.ContainsKey(FtpUserKey))
                    return (Properties[FtpUserKey].ToString());
                return ("");
            }

            set
            {
                Properties[FtpUserKey]= value;
            }
        }

        public string FtpPassword
        {
            get
            {
                if (Properties.ContainsKey(FtpPasswordKey))
                    return (Properties[FtpPasswordKey].ToString());
                return ("");
            }

            set
            {
                Properties[FtpPasswordKey] = value;
            }
        }

        public bool FtpSsl
        {
            get 
            {
                if (Properties.ContainsKey(FtpSslKey))
                    return ((bool)Properties[FtpSslKey]);
                return (false);
            }

            set
            {
                Properties[FtpSslKey] = value;
            }
        }
    }
}

using LinkOS.Plugin.Abstractions;
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


        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new FacilePage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected async override void OnSleep()
        {
			IDiscoveredPrinter printer = Printer;

			await SavePropertiesAsync();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

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

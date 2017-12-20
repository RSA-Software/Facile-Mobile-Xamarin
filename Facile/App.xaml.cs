using Xamarin.Forms;

namespace Facile
{
    public partial class App : Application
    {
        private const string FtpServerKey = "FtpServer";
        private const string FtpUserKey = "FtpUser";
        private const string FtpPasswordKey = "FtpPassword";
        private const string FtpSslKey = "FtpSsl";

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new FacilePage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            SavePropertiesAsync();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
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

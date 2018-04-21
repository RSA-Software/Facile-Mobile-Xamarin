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

namespace Facile
{
    public partial class SetupPage : ContentPage
    {
        public SetupPage()
        {

            InitializeComponent();
            BindingContext = Application.Current;
        }

    }
}

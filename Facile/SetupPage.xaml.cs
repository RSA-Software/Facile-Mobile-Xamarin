using System;
using System.Collections.Generic;

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

        async protected override void OnDisappearing()
        {
            base.OnDisappearing();
            await Application.Current.SavePropertiesAsync();
        }
    }
}
